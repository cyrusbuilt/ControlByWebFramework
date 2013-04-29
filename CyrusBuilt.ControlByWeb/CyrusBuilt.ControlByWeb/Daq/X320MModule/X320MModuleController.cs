using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.Security;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Used to monitor and control a ControlByWeb X-320M module.
    /// </summary>
    public class X320MModuleController : IDisposable
    {
        #region Fields
        private Boolean _disposed = false;
        private Boolean _authEnabled = false;
        private Boolean _isConnected = false;
        private SecureString _password = null;
        private IPAddress _ip = null;
        private Int32 _port = Common.DEFAULT_PORT;
        private TcpClient _tcpClient = null;
        private NetworkStream _netStream = null;
        private Thread _pollThread = null;
        private Int32 _pollInterval = Common.DEFAULT_POLL_INTERVAL;
        private Boolean _isPolling = false;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the device is successfully polled for status.
        /// </summary>
        public event PollStatusEventHandler Polled;

        /// <summary>
        /// Occurs when a status poll fails.
        /// </summary>
        public event PollFailEventHandler PollFailed;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320MModule.X320MModuleController</b>
        /// class. This is the default constructor.
        /// </summary>
        public X320MModuleController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320MModule.X320MModuleController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ip">
        /// The IP address of the module to control.
        /// </param>
        public X320MModuleController(IPAddress ip) {
            this._ip = ip;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320Module.X320MModuleController</b>
        /// class with the IP address of the module to control and the port to
        /// communicate with it on.
        /// </summary>
        /// <param name="ip">
        /// The IP address of the module to control.
        /// </param>
        /// <param name="port">
        /// The port to communicate with the device on.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The port number assignment is invalid. Must be 1 - 65535.
        /// </exception>
        public X320MModuleController(IPAddress ip, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ip;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320Module.X320MModuleController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public X320MModuleController(IPEndPoint endpoint) {
            if (endpoint != null) {
                this._ip = endpoint.Address;
                this._port = endpoint.Port;
            }
        }

        /// <summary>
        /// Releases all resources that are used by this component.
        /// </summary>
        /// <param name="disposing">
        /// Set <code>true</code> to dispose both managed and unmanaged resources.
        /// </param>
        protected virtual void Dispose(Boolean disposing) {
            if (this._disposed) {
                return;
            }

            if (disposing) {
                if (this._netStream != null) {
                    this._netStream.Close();
                    this._netStream.Dispose();
                }

                if (this._tcpClient != null) {
                    this._tcpClient.Close();
                    this._tcpClient = null;
                }
                this._password = null;
                this._authEnabled = false;
                this._ip = null;
                this._port = 0;
            }
            this._disposed = true;
        }

        /// <summary>
        /// Releases all the resources that are used by this component.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether or not this component instance has been disposed.
        /// </summary>
        public Boolean IsDisposed {
            get { return this._disposed; }
        }

        /// <summary>
        /// Gets whether or not this component is connected to the device.
        /// </summary>
        public Boolean IsConnected {
            get { return this._isConnected; }
        }

        /// <summary>
        /// Gets or sets the IP address of the module to communicate with.
        /// </summary>
        public IPAddress ModuleAddress {
            get { return this._ip; }
            set { this._ip = value; }
        }

        /// <summary>
        /// Gets or sets the port to communicate with the device on. The default
        /// is <see cref="Common.DEFAULT_PORT"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The port number assignment is invalid. Must be 1 - 65535.
        /// </exception>
        public Int32 Port {
            get { return this._port; }
            set {
                if ((value < 1) || (value > 65535)) {
                    throw new ArgumentOutOfRangeException("Port", "Port number must 1 - 65535.");
                }
                this._port = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not Basic Authentication is enabled.
        /// </summary>
        public Boolean BasicAuthEnabled {
            get { return this._authEnabled; }
            set { this._authEnabled = value; }
        }

        /// <summary>
        /// The password to use when Basic Authentication is enabled.
        /// </summary>
        public SecureString Password {
            get { return this._password; }
            set { this._password = value; }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Establishes a connection to the device.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The IP address of the device is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        private void Connect() {
            if (this._isConnected) {
                return;
            }

            if (this._ip == null) {
                throw new InvalidOperationException("Device IP address undefined.");
            }

            // Create a new TCP client. If we already have one,
            // make sure we break any existing connection.
            if (this._tcpClient != null) {
                lock (this._tcpClient) {
                    if (this._tcpClient.Connected) {
                        this._tcpClient.Close();
                    }
                }
            }

            // Create a new endpoint and connect to it.
            this._tcpClient = new TcpClient();
            IPEndPoint endpoint = new IPEndPoint(this._ip, this._port);
            try {
                lock (this._tcpClient) {
                    this._tcpClient.Connect(endpoint);
                }
            }
            catch (SocketException) {
                throw;
            }

            // Get the network stream. This is what we will read status from
            // and write commands to.
            lock (this._tcpClient) {
                if (this._tcpClient.Connected) {
                    this._netStream = this._tcpClient.GetStream();
                    this._isConnected = true;
                }
            }
        }

        /// <summary>
        /// Disconnects from the remote host.
        /// </summary>
        private void Disconnect() {
            if (!this._isConnected) {
                return;
            }

            lock (this) {
                // Kill the underlying stream.
                if (this._netStream != null) {
                    this._netStream.Close();
                }

                // Kill the underlying TCP connection.
                if (this._tcpClient != null) {
                    this._tcpClient.Close();
                }
                this._isConnected = false;
            }
        }

        /// <summary>
        /// Gets device diagnostics from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The node to parse the diagnostic info from.
        /// </param>
        /// <returns>
        /// The diagnostic info as parsed from the specified XML node.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> cannot be null.
        /// </exception>
        private X320MDiagnostics GetDiagsFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X320MDiagnostics diags = new X320MDiagnostics();
            String itemText = String.Empty;
            Int32 value = 0;
            Double temp = 0.0;
            Double volts = 0.0;

            // Get the internal temperature.
            itemText = Common.GetNamedChildNode(node, "internalTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    diags.SetInternalTemp(temp);
                }
            }

            // Get the input voltage.
            itemText = Common.GetNamedChildNode(node, "vin").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out volts)) {
                    diags.SetVoltageIn(volts);
                }
            }

            // Get internal 6 volt supply reading.
            volts = 0.0;
            itemText = Common.GetNamedChildNode(node, "internal6Volt").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out volts)) {
                    diags.SetInternalVoltage(volts);
                }
            }

            // Get mem power up flag.
            itemText = Common.GetNamedChildNode(node, "memoryPowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    diags.SetMemoryPowerUpFlag(Common.GetPowerUpFlag(value));
                }
            }

            // Get device power up flag.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "devicePowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    diags.SetDevicePowerUpFlag(Common.GetPowerUpFlag(value));
                }
            }

            // Get power loss count.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "powerLossCounter").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    diags.SetPowerLossCount(value);
                }
            }
            return diags;
        }

        /// <summary>
        /// Gets the state of the device from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse the state information from.
        /// </param>
        /// <returns>
        /// The state of the device as parsed from the specified node.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> cannot be null.
        /// </exception>
        private X320MState GetStateFromXmlNode(XmlNode node) {
            // TODO I hate, hate, HATE this method. I really need to find a
            // more efficient way... Right now its more like a fucking mess
            // of spaghetti code.
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X320MState state = new X320MState();
            String itemText = String.Empty;
            String attrib = String.Empty;
            Double dValue = 0.0;
            Int32 iValue = 0;
            Epoch unixTime = null;
            AlarmConditions condition = AlarmConditions.Normal;

            // Get wind speed.
            itemText = Common.GetNamedChildNode(node, "windSpd").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeWindSpeed(dValue);
                }
            }

            // Get wind direction.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "windDir").InnerText;
            if (String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeWindDirection(dValue);
                }
            }

            // Get total rainfall.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "rainTot").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeRainTotal(dValue);
                }
            }

            // Get current temperature.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "temp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    SensorInput sensor = new SensorInput(dValue);
                    state.ChangeTemperatureSensor(sensor);
                }
            }

            // Get relative humdidity.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "humidity").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeHumidty(dValue);
                }
            }

            // Get solar radiation.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "solarRad").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeSolarRadition(dValue);
                }
            }

            // Get barometric pressure.
            dValue = 0.0;
            itemText = Common.GetNamedChildNode(node, "barPressure").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeBarometricPressure(dValue);
                }
            }

            // Get auxiliary input readings.
            for (Int32 i = 1; i <= X320MConstants.TOTAL_AUX_INPUTS; i++) {
                dValue = 0.0;
                attrib = String.Format("aux{0}", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out dValue)) {
                        state.ChangeAuxiliarySensorValue(i, dValue);
                    }
                }
            }
            
            // Get rainfall histories.
            dValue = 0.0;
            attrib = X320MUtils.GetRainfallHistoryName(RainfallReadings.LastHour);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeRainfallHistory(RainfallReadings.LastHour, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetRainfallHistoryName(RainfallReadings.TotalSevenDays);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeRainfallHistory(RainfallReadings.TotalSevenDays, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetRainfallHistoryName(RainfallReadings.TotalToday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeRainfallHistory(RainfallReadings.TotalToday, dValue);
                }
            }

            // Get rainfall reset time.
            itemText = Common.GetNamedChildNode(node, "rainRst").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Epoch.TryParse(itemText, out unixTime)) {
                    state.ChangeRainTotalResetTime(unixTime);
                }
            }

            // Gets the rain alarm state.
            itemText = Common.GetNamedChildNode(node, "rainAlrm").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out iValue)) {
                    condition = Common.GetAlarmCondition(iValue);
                    state.ChangeRainAlarmCondition(condition);
                }
            }

            // Get temperature histories.
            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.DewPoint);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.DewPoint, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.HeatIndex);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.HeatIndex, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.HighTempToday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.HighTempToday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.HighTempYesterday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.HighTempYesterday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.LowTempToday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.LowTempToday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.LowTempYesterday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.LowTempYesterday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetTemperatureHistoryName(TemperatureReadings.WindChill);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeTemperatureHistory(TemperatureReadings.WindChill, dValue);
                }
            }

            // Get temperature alarm state.
            iValue = 0;
            itemText = Common.GetNamedChildNode(node, "tempAlarm").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out iValue)) {
                    condition = Common.GetAlarmCondition(iValue);
                    state.ChangeTemperatureAlarmCondition(condition);
                }
            }

            // Get humidity histories.
            dValue = 0.0;
            attrib = X320MUtils.GetHumidityHistoryName(HumidityReadings.HighToday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeHumidityHistory(HumidityReadings.HighToday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetHumidityHistoryName(HumidityReadings.HighYesterday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeHumidityHistory(HumidityReadings.LowToday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetHumidityHistoryName(HumidityReadings.LowToday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeHumidityHistory(HumidityReadings.LowToday, dValue);
                }
            }

            dValue = 0.0;
            attrib = X320MUtils.GetHumidityHistoryName(HumidityReadings.LowYesterday);
            itemText = Common.GetNamedChildNode(node, attrib).InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out dValue)) {
                    state.ChangeHumidityHistory(HumidityReadings.LowYesterday, dValue);
                }
            }

            // Get humidity alarm.
            iValue = 0;
            itemText = Common.GetNamedChildNode(node, "humidityAlrm").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out iValue)) {
                    condition = Common.GetAlarmCondition(iValue);
                    state.ChangeHumidityAlarmCondition(condition);
                }
            }

            // Get barometer histories.
			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastFifteenHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastFifteenHours, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastHour);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastHour, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastNineHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastNineHours, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastSixHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastSixHours, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastThreeHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastThreeHours, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastTwelveHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastTwelveHours, dValue);
				}
			}

			dValue = 0.0;
			attrib = X320MUtils.GetBarometerHistoryName(BarometerReadings.LastTwentyFourHours);
			itemText = Common.GetNamedChildNode(node, attrib).InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeBarometerHistory(BarometerReadings.LastTwentyFourHours, dValue);
				}
			}

			// Get barometer alarm.
			iValue = 0;
			itemText = Common.GetNamedChildNode(node, "presAlrm").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Int32.TryParse(itemText, out iValue)) {
					condition = Common.GetAlarmCondition(iValue);
					state.ChangeBarometerAlarmCondition(condition);
				}
			}

			// Gets the wind gust speed.
			dValue = 0.0;
			itemText = Common.GetNamedChildNode(node, "windGust").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeWindGustSpeed(dValue);
				}
			}

			// Change wind gust direction.
			dValue = 0.0;
			itemText = Common.GetNamedChildNode(node, "windGustDir").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Double.TryParse(itemText, out dValue)) {
					state.ChangeWindGustDirection(dValue);
				}
			}

			// Get wind alarm.
			iValue = 0;
			itemText = Common.GetNamedChildNode(node, "windAlrm").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Int32.TryParse(itemText, out iValue)) {
                    condition = Common.GetAlarmCondition(iValue);
					state.ChangeWindGustAlarmCondition(condition);
				}
			}

			// Get power up time.
			unixTime = null;
			itemText = Common.GetNamedChildNode(node, "powerUp").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Epoch.TryParse(itemText, out unixTime)) {
					state.ChangePowerUpTime(unixTime);
				}
			}

			// Get serial.
			itemText = Common.GetNamedChildNode(node, "serialNumber").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				state.ChangeSerial(itemText);
			}

			// Get device time.
			unixTime = null;
			itemText = Common.GetNamedChildNode(node, "time").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Epoch.TryParse(itemText, out unixTime)) {
					state.ChangeTime(unixTime);
				}
			}

            return state;
        }

        /// <param name="command">
        /// The command to send to the device. The command should end with
        /// "\r\n\r\n".
        /// </param>
        /// <returns>
        /// If successful, an <see cref="XmlDocument"/> containing the response
        /// message. If no response is recieved from the device, null is
        /// returned.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address is
        /// not defined.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// command cannot be null or empty.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        private XmlDocument SendCommand(String command) {
            if (this._disposed) {
                throw new InvalidOperationException("Cannot perform operation after the object has been disposed.");
            }

            if (String.IsNullOrEmpty(command)) {
                throw new ArgumentNullException("command");
            }

            // Connect to the device.
            try {
                this.Connect();
            }
            catch (InvalidOperationException) {
                throw;
            }
            catch (SocketException) {
                throw;
            }

            // Unable to connect to device. Fail.
            if (!this._isConnected) {
                throw new ConnectionFailedException(this._ip, this._port);
            }

            // We can't read/write to the underlying stream. Get to the choppa!
            if ((!this._netStream.CanWrite) || (!this._netStream.CanRead)) {
                // The Connect() method is responsible for setting _netStream.
                // I really can't think of anything that could cause this that
                // isn't already handled by throwing any of the previous exceptions.
                // But just in case...
                throw new IOException("Cannot read/write the underlying NetworkStream.");
            }

            // Convert our command into an ASCII byte array and send the
            // the command to the device.
            Byte[] sendBytes = { };
            lock (this._netStream) {
                sendBytes = Encoding.ASCII.GetBytes(command);
                this._netStream.Write(sendBytes, 0, sendBytes.Length);
            }

            // Get the response from the device.
            Byte[] recvBytes = { };
            lock (this._netStream) {
                recvBytes = new Byte[this._tcpClient.ReceiveBufferSize];
                this._netStream.Read(recvBytes, 0, this._tcpClient.ReceiveBufferSize);
            }

            // We don't need the connection anymore.
            this.Disconnect();
            if (recvBytes.Length == 0) {
                // No response from device. This could happen if &noReply=1 in
                // the command we sent.
                return null;
            }

            // Check to see if our request was rejected due to auth error.
            String responseMsg = Encoding.ASCII.GetString(recvBytes);
            if (responseMsg.Contains("401 Authorization Required")) {
                throw new UnauthorizedAccessException();
            }

            // For some reason, the device returns a shit ton of "\0"
            // after the XML response. We have to chop all that off here.
            ASCIIEncoding enc = new ASCIIEncoding();
            recvBytes = enc.GetBytes(Common.TrimResponse(responseMsg));

            // If we got a response, then we convert the recieve buffer into an
            // XML document so we can parse it.
            MemoryStream memStream = new MemoryStream(recvBytes);
            memStream.Seek(0, SeekOrigin.Begin);
            XmlDocument doc = new XmlDocument();
            try {
                doc.Load(memStream);
            }
            catch (XmlException) {
                throw new BadResponseFromDeviceException(recvBytes);
            }

            // Dump the send and recieve buffers, then return the state.
            Array.Clear(sendBytes, 0, sendBytes.Length);
            Array.Clear(recvBytes, 0, recvBytes.Length);
            return doc;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="Polled"/> event if a handler is attached.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnPolled(PollStatusEventArgs e) {
            if (this.Polled != null) {
                this.Polled(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PollFailed"/> event if a handler is attached.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnPollFailed(PollFailedEventArgs e) {
            if (this.PollFailed != null) {
                this.PollFailed(this, e);
            }
        }

        /// <summary>
        /// Gets the state of the X-320M device.
        /// </summary>
        /// <returns>
        /// If successful, the state of the device; Otherwise, null.
        /// </returns>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public X320MState GetState() {
            // Construct the command.
            String command = "GET /state.xml?noReply=0 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }

            // Send the command and get the response. Then run the response
            // through the appropriate state parser.
            XmlDocument response = this.SendCommand(command);
            if (response != null) {
                try {
                    XmlNode node = response.SelectSingleNode("/datavalues");
                    return this.GetStateFromXmlNode(node);
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets diagnostic information from the device.
        /// </summary>
        /// <returns>
        /// If successful, a <see cref="X320MDiagnostics"/> state object
        /// containing diagnostic information specific to the device;
        /// Otherwise, null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public X320MDiagnostics GetDiagnostics() {
            // Construct the command. Note: Authentication is not required for
            // diagnostics! We ignore any provided auth creds for this.
            String command = "GET /diagnostics.xml?noReply=0 HTTP/1.1\r\n\r\n";

            // Send the command and get the response. Then run the response
            // through the appropriate state parser.
            XmlDocument response = this.SendCommand(command);
            if (response != null) {
                try {
                    XmlNode node = response.SelectSingleNode("/datavalues");
                    return this.GetDiagsFromXmlNode(node);
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }
            return null;
        }

        /// <summary>
        /// Ends the poll cycle.
        /// </summary>
        public void EndPollCycle() {
            if (this._isPolling) {
                lock (this) {
                    this._isPolling = false;
                }
            }
        }

        /// <summary>
        /// Polls the device state.
        /// </summary>
        private void Poll() {
            Exception fail = null;
            X320MState state = null;
            while (this._isPolling) {
                try {
                    state = this.GetState();
                    this.OnPolled(new PollStatusEventArgs(state));
                    Thread.Sleep(this._pollInterval * 1000);
                }
                catch (InvalidOperationException ioEx) {
                    fail = ioEx;
                }
                catch (SocketException socEx) {
                    fail = socEx;
                }
                catch (ConnectionFailedException cfEx) {
                    fail = cfEx;
                }
                catch (IOException iEx) {
                    fail = iEx;
                }
                catch (UnauthorizedAccessException uaEx) {
                    fail = uaEx;
                }
                catch (BadResponseFromDeviceException brfdEx) {
                    fail = brfdEx;
                }
                finally {
                    if (fail != null) {
                        this.OnPollFailed(new PollFailedEventArgs(fail));
                        this.EndPollCycle();
                    }
                }
            }
            fail = null;
            state = null;
        }

        /// <summary>
        /// Begins polling the device for status.
        /// </summary>
        public void BeginPollCycle() {
            if (this._isPolling) {
                return;
            }
            this._pollThread = new Thread(new ThreadStart(this.Poll));
            this._pollThread.IsBackground = true;
            this._pollThread.Name = "X320MStatePoller";
            this._isPolling = true;
            this._pollThread.Start();
        }

        /// <summary>
        /// Convenience method for switching the specified relay on if it is
        /// not already.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to switch on.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified relay number is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void SwitchRelayOn(Int32 relayNum) {
            if ((relayNum < 1) || (relayNum > X320MConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X320MConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Build the command and send it.
            Int32 on = Common.GetRelayStateCode(RelayState.On);
            String command = String.Format("GET /state.xml?relay{0}state={1}&noReply=1 HTTP/1.1\r\n\r\n",
                                            relayNum.ToString(),
                                            on.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        ///  Convenience method for switching the specified relay off if it is
        ///  not already.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to switch off.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified relay number is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void SwitchRelayOff(Int32 relayNum) {
            if ((relayNum < 1) || (relayNum > X320MConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X320MConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Build the command and send it.
            Int32 on = Common.GetRelayStateCode(RelayState.Off);
            String command = String.Format("GET /state.xml?relay{0}state={1}&noReply=1 HTTP/1.1\r\n\r\n",
                                            relayNum.ToString(),
                                            on.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Pulse the specified relay.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to pulse.
        /// </param>
        /// <param name="pulseTime">
        /// The amount of time (in seconds) to pulse the relay.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified relay number is invalid.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// pulseTime cannot be greater than <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void PulseRelay(Int32 relayNum, Double pulseTime) {
            if ((relayNum < 1) || (relayNum > X320MConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X320MConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            if (pulseTime > Common.MAX_PULSE_DURATION) {
                String err = String.Format("pulseTime cannot be greater than {0}.",
                                            Common.MAX_PULSE_DURATION.ToString());
                throw new ArgumentException(err, "pulseTime");
            }

            // This is essentially a no-op.
            if (pulseTime < 0.1) {
                return;
            }

            Int32 pulse = Common.GetRelayStateCode(RelayState.Pulse);
            String command = String.Format("GET /state.xml?relay{0}state={1}&pulseTime={2}&noReply=1 HTTP/1.1\r\n\r\n",
                                            relayNum.ToString(),
                                            pulse.ToString(),
                                            pulseTime.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Pulse the specified relay.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to pulse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified relay number is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Authentication is enabled and the password is either undefined or
        /// incorrect.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void PulseRelay(Int32 relayNum) {
            this.PulseRelay(relayNum, Common.DEFAULT_PULSE_TIME);
        }

        /// <summary>
        /// Clears the power loss counter.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void ClearPowerLossCounter() {
            String command = "GET /diagnostics.xml?powerLossCounter=0&noReply=1 HTTP/1.1\r\n\r\n";
            this.SendCommand(command);
        }

        /// <summary>
        /// Clears the internal memory power up flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void ClearMemPowerUpFlag() {
            Int32 pUpFlag = Common.GetPowerUpFlagCode(PowerUpFlag.Off);
            String command = String.Format("GET /diagnostics.xml?memoryPowerUpFlag={0}&noReply=1 HTTP/1.1\r\n\r\n",
                                            pUpFlag.ToString());
            this.SendCommand(command);
        }

        /// <summary>
        /// Clears the device power up flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void ClearDevicePowerUpFlag() {
            Int32 pUpFlag = Common.GetPowerUpFlagCode(PowerUpFlag.Off);
            String command = String.Format("GET /diagnostics.xml?devicePowerUpFlag={0}&nReply=1 HTTP/1.1\r\n\r\n",
                                            pUpFlag.ToString());
            this.SendCommand(command);
        }

        /// <summary>
        /// Clears both the memory power up flag and the device power up flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address
        /// is not defined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Could not connect to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read or write the underlying <see cref="NetworkStream"/>.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void ClearPowerUpFlags() {
            Int32 pUpFlag = Common.GetPowerUpFlagCode(PowerUpFlag.Off);
            String command = String.Format("GET /diagnostics.xml?memoryPowerUpFlag={0}&" +
                                            "devicePowerUpFlag={1}&noReply=1 HTTP/1.1 \r\n\r\n",
                                            pUpFlag.ToString(),
                                            pUpFlag.ToString());
            this.SendCommand(command);
        }
        #endregion
    }
}
