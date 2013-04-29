using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.Security;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Used to monitor and control a ControlyByWeb X-300 module.
    /// </summary>
    public class X300ModuleController : IDisposable
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
        private X300OperationMode _mode = X300OperationMode.TemperatureMonitor;
		private Thread _pollThread = null;
		private Int32 _pollInterval = Common.DEFAULT_POLL_INTERVAL;
		private Boolean _isPolling = false;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the X-300 device status is polled.
        /// </summary>
        public event PollStatusEventHandler Polled;
        
        /// <summary>
        /// Occurs when polling the device fails.
        /// </summary>
        public event PollFailEventHandler PollFailed;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300ModuleController</b>
        /// class. This is the default constructor.
        /// </summary>
        public X300ModuleController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300ModuleController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        public X300ModuleController(IPAddress ipAddr) {
            this._ip = ipAddr;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300ModuleController</b>
        /// class with the IP address of the module to control and the port to communicate with it on.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        /// <param name="port">
        /// The port to communicate with the device on.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The port number assignment is invalid. Must be 1 - 65535.
        /// </exception>
        public X300ModuleController(IPAddress ipAddr, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ipAddr;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300ModuleController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public X300ModuleController(IPEndPoint endpoint) {
            if (endpoint != null) {
                this._ip = endpoint.Address;
                this._port = endpoint.Port;
            }
        }

        /// <summary>
        /// Releases all resources that are used by this component.
        /// </summary>
        /// <param name="disposing">
        /// Set <b>true</b> to dispose both managed and unmanaged resources.
        /// </param>
        public virtual void Dispose(Boolean disposing) {
            if (this._disposed) {
                return;
            }

            if (disposing) {
                if (this._isPolling) {
                    lock (this) {
                        this._isPolling = false;
                    }

                    Thread.Sleep(300);
                    if (this._pollThread != null) {
                        if (this._pollThread.IsAlive) {
                            try {
                                this._pollThread.Abort();
                            }
                            catch (ThreadAbortException) {
                            }
                        }
                        this._pollThread = null;
                    }
                }

                if (this._netStream != null) {
                    this._netStream.Close();
                    this._netStream.Dispose();
                    this._netStream = null;
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

        /// <summary>
        /// Gets the current mode of the device.
        /// </summary>
        public X300OperationMode Mode {
            get { return this._mode; }
        }

        /// <summary>
        /// Gets or sets the poll interval (in seconds).
        /// </summary>
        /// <value>
        /// The poll interval (in seconds). The default
        /// is <see cref="Common.DEFAULT_POLL_INTERVAL"/>.
        /// </value>
        public Int32 PollInterval {
            get { return this._pollInterval; }
            set { this._pollInterval = value; }
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
        /// Parses device diagnostics from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse state information from.
        /// </param>
        /// <returns>
        /// An object containing diagnostic information for the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        private X300Diagnostics GetDiagsFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X300Diagnostics diags = new X300Diagnostics();
            String itemText = String.Empty;
            Double temp = 0.0;
            Double volts = 0.0;
            Int32 flag = 0;
            Int32 count = 0;

            // Get the internal temperature.
            itemText = Common.GetNamedChildNode(node, "internalTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    diags.SetInternalTemp(temp);
                }
            }

            // Get the voltage being applied to the VIN terminals.
            itemText = Common.GetNamedChildNode(node, "vin").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out volts)) {
                    diags.SetVoltageIn(volts);
                }
            }

            // Get the actual voltage of the device's internal 5V supply.
            volts = 0.0;
            itemText = Common.GetNamedChildNode(node, "fiveVolt").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out volts)) {
                    diags.SetInternalVoltage(volts);
                }
            }

            // Get the power loss indicator for the RTC.
            itemText = Common.GetNamedChildNode(node, "memoryPowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out flag)) {
                    diags.SetMemoryPowerUpFlag(Common.GetPowerUpFlag(flag));
                }
            }

            // Get the power loss indicator for the device itself.
            flag = 0;
            itemText = Common.GetNamedChildNode(node, "devicePowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out flag)) {
                    diags.SetDevicePowerUpFlag(Common.GetPowerUpFlag(flag));
                }
            }

            // Get a count of how many times the device lost power.
            itemText = Common.GetNamedChildNode(node, "powerLossCounter").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out count)) {
                    diags.SetPowerLossCount(count);
                }
            }
            return diags;
        }

        /// <summary>
        /// Parses temperature monitor state from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse state information from.
        /// </param>
        /// <returns>
        /// An object containing the temperature monitor state for the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not currently set to <see cref="X300OperationMode.TemperatureMonitor"/>.
        /// </exception>
        private X300TempMonitorState GetMonitorStateFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            if (this._mode != X300OperationMode.TemperatureMonitor) {
                throw new InvalidOperationException("Mode must be set to TemperatureMonitor.");
            }

            X300TempMonitorState state = new X300TempMonitorState();
            String itemText = String.Empty;
            String attrib = String.Empty;
            Double temp = 00.00;
            Int32 value = 0;
            SensorInput input = null;
            Relay rel = null;

            // The default is Fahrenheit. Change to Celcius if need be.
            itemText = Common.GetNamedChildNode(node, "units").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (itemText.ToUpper() == "C") {
                    state.SetTemperatureUnits(TemperatureUnits.Celcius);
                }
            }

            // Get the sensor input values.
            for (Int32 i = 1; i <= X300Constants.TOTAL_INPUTS; i++) {
                attrib = String.Format("sensor{0}temp", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    input = new SensorInput();
                    if (itemText != "x.x") {
                        if (Double.TryParse(itemText, out temp)) {
                            input.SetTemperature(temp);
                        }
                    }
                    state.SetSensorInput(i, input);
                }
            }

            // Get the relay states.
            for (Int32 r = 1; r <= X300Constants.TOTAL_RELAYS; r++) {
                attrib = String.Format("relay{0}state", r.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out value)) {
                        rel = new Relay(Common.GetRelayState(value));
                        switch (r) {
                            case 1:
                                state.Relay1 = rel;
                                break;
                            case 2:
                                state.Relay2 = rel;
                                break;
                            case 3:
                                state.Relay3 = rel;
                                break;
                        }
                    }
                }
            }
            return state;
        }

        /// <summary>
        /// Parses thermostat state from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse state information from.
        /// </param>
        /// <returns>
        /// An object containing the thermostat state for the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not currently set to <see cref="X300OperationMode.Thermostat"/>.
        /// </exception>
        private X300ThermostatState GetThermostatFromXmlNode(XmlNode node) {
            // TODO Seems like we could do a better job than this. This method
            // feels like a run-on sentence. Maybe should break this up a bit.
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Mode must be set to Thermostat.");
            }

            X300ThermostatState state = new X300ThermostatState();
            String itemText = String.Empty;
            Double temp = 00.00;
            Int32 value = 0;
            Int64 unixTime = 0;

            // The default is Fahrenheit. Change to Celcius if need be.
            itemText = Common.GetNamedChildNode(node, "units").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (itemText.ToUpper() == "C") {
                    state.SetTemperatureUnits(TemperatureUnits.Celcius);
                }
            }

            // Get the indoor temperature.
            itemText = Common.GetNamedChildNode(node, "indoorTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetIndoorTemperature(temp);
                }
            }

            // Get the outdoor temperature.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "outdoorTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetOutdoorTemperature(temp);
                }
            }

            // Get the set temperature.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "setTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetTemperature = temp;
                }
            }

            // Get the heat relay state.
            itemText = Common.GetNamedChildNode(node, "heat").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.SetHeat(Common.GetRelayState(value));
                }
            }

            // Get the cool relay state.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "cool").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.SetCool(Common.GetRelayState(value));
                }
            }

            // Get the fan relay state.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "fan").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.SetFan(Common.GetRelayState(value));
                }
            }

            // Get the minimum 24 hour temp reading.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "minTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMin24HourTemp(temp);
                }
            }

            // Get the maximum 24 hour temp reading.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "maxTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMax24HourTemp(temp);
                }
            }

            // Get the minimum temperature reading for yesterday.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "minTempY").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMinYesterdayTemp(temp);
                }
            }

            // Get the max temperature reading for yesterday.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "maxTempY").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMaxYesterdayTemp(temp);
                }
            }

            // Get the heat mode.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "headMode").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.HeatMode = X300Utils.GetHeatMode(value);
                }
            }

            // Get the fan mode.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "fanMode").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.FanMode = X300Utils.GetFanMode(value);
                }
            }

            // Get the filter change days.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "filtChng").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    state.SetFilterChangeDays(value);
                }
            }

            // Get the min temp that can be set.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "minSTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMinimumSetTemp(temp);
                }
            }

            // Get the max temp that can be set.
            temp = 0.0;
            itemText = Common.GetNamedChildNode(node, "maxSTemp").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out temp)) {
                    state.SetMaximumSetTemp(temp);
                }
            }

            // Get the epoch time.
            itemText = Common.GetNamedChildNode(node, "time").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int64.TryParse(itemText, out unixTime)) {
                    state.SetTime(unixTime);
                }
            }

            // Get the serial number (same as MAC address).
            itemText = Common.GetNamedChildNode(node, "serialNumber").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                // Try to parse the serial number (MAC).
                Boolean parsed = false;
                PhysicalAddress serial = null;
                try {
                    serial = PhysicalAddress.Parse(itemText);
                    parsed = true;
                }
                catch (FormatException) {
                    parsed = false;
                }

                // Got a good serial.
                if (parsed) {
                    state.SetSerial(serial);
                }
            }
            return state;
        }

        /// <summary>
        /// Sends the specified command to the device and gets its response.
        /// </summary>
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
        /// Raises the X-300 polled event.
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
        /// Raises the X-300 poll failed event.
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
        /// Changes the operational mode of the controller.
        /// </summary>
        /// <param name="mode">
        /// The mode to change to.
        /// </param>
        public void ChangeMode(X300OperationMode mode) {
            // We use this method instead defining a setter for the
            // mode property because the mode should never be able
            // to be unintentionally changed. It should be an
            // explicit call to this method so the intent is clear.
            lock (this) {
                this._mode = mode;
            }
        }

        /// <summary>
        /// Gets the temperature monitor state if <see cref="Mode"/> is
        /// <see cref="X300OperationMode.TemperatureMonitor"/>.
        /// </summary>
        /// <returns>
        /// If successful, the state of the temperature monitor; Otherwise,
        /// null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.TemperatureMonitor"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public X300TempMonitorState GetTempMonitorState() {
            // Make sure we're in the right mode.
            if (this._mode != X300OperationMode.TemperatureMonitor) {
                String err = "Mode must be X300OperationMode.TemperatureMonitor to monitor temperature.";
                throw new InvalidOperationException(err);
            }

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
                    return this.GetMonitorStateFromXmlNode(node);
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the temperature monitor state if <see cref="Mode"/> is
        /// <see cref="X300OperationMode.Thermostat"/>.
        /// </summary>
        /// <returns>
        /// If successful, the state of the thermostat; Otherwise, null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public X300ThermostatState GetThermostatState() {
            // Make sure we're in the right mode.
            if (this._mode != X300OperationMode.Thermostat) {
                String err = "Mode must be X300OperationMode.Thermostat to monitor thermostat.";
                throw new InvalidOperationException(err);
            }

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
                    return this.GetThermostatFromXmlNode(node);
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
        /// If successful, a <see cref="X300Diagnostics"/> state object
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
        public X300Diagnostics GetDiagnostics() {
            // We don't care about the controller mode for diags.

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
        /// Sets the set temperature on the device. This controller must be in
        /// thermostat mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void SetTemperature(Double temp) {
            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Must be in thermostat mode to set temperature.");
            }

            // Construct the command and send it.
            String command = String.Format("GET /state.xml?setTemp={0}&noReply=1 HTTP/1.1\r\n\rn",
                                            temp.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Sets the heat mode on the device. This controller must be in
        /// thermostat mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="mode">
        /// The heat mode to set.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void SetHeatMode(HeatMode mode) {
            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Must be in thermostat mode to set heat mode.");
            }

            String command = String.Format("GET /state.xml?heatMode={0}&noReply=1 HTTP/1.1\r\n\r\n",
                                            X300Utils.GetHeadModeCode(mode).ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Hold the current set temperature on the device. Essentially, this
        /// toggles 7-day programming on/off.  This controller must be in
        /// thermostat mode. See <see cref="Mode"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void Hold() {
            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Must be in thermostat mode to hold temperature.");
            }

            String command = "GET /state.xml?hold=1&noReply=1 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Changes the fan mode.  This controller must be in
        /// thermostat mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="mode">
        /// The fan mode to set.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void ChangeFanMode(FanMode mode) {
            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Must be in thermostat mode to change fan mode.");
            }

            String command = String.Format("GET /state.xml?fanMode={0}&noReply=1 HTTP/1.1\r\n\r\n",
                                            X300Utils.GetFanModeCode(mode).ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Resets the filter counter back to the filter change interval. This
        /// will only reset the filter counter once the counter has reached
        /// zero. This controller must be in thermostat mode. 
        /// See <see cref="Mode"/>.
        /// </summary>
        /// /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void ResetFilterCounter() {
            if (this._mode != X300OperationMode.Thermostat) {
                throw new InvalidOperationException("Must be in thermostat mode to reset filter.");
            }

            String command = "GET /state.xml?rstFilt=1&noReply=1 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Sets the thermostat. Only certain values of the specified state can
        /// be committed to the device (SetTemperature, HeatMode, Holding,
        /// FanMode, and FilterResetRequested). This controller must be in 
        /// thermostat mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="state">
        /// The state of the thermostat to set. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// state cannot be null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.Thermostat"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void SetThermostat(X300ThermostatState state)
		{
			if (state == null) {
				throw new ArgumentNullException("state");
			}

			if (this._mode != X300OperationMode.Thermostat) {
				throw new InvalidOperationException("Must be in thermostat mode to set thermostat.");
			}

			Double setTemp = state.SetTemperature;
			Int32 heatMode = X300Utils.GetHeadModeCode(state.HeatMode);
			String command = String.Format("GET /state.xml?setTemp={0}&heatMode={1}",
                                            setTemp.ToString(),
                                            heatMode.ToString());
			if (state.Holding) {
				String hold = String.Format("&hold={0}", X300Constants.HOLD_TOGGLE.ToString());
				command = String.Concat(command, hold);
			}
			
			Int32 fanMode = X300Utils.GetFanModeCode(state.FanMode);
			command = String.Concat(command, String.Format("&fanMode={0}", fanMode.ToString()));
			if (state.FilterResetRequested) {
				String fltRst = String.Format("&rstFilt={0}", X300Constants.FILTER_RST.ToString());
				command = String.Concat(command, fltRst);
			}
			
			command = String.Concat(command, "&noReply=1 HTTP/1.1\r\n\r\n");
			if (this._authEnabled) {
				command = Common.AppendAuthToCommand(command, this._password);
			}
			this.SendCommand(command);
		}

        /// <summary>
        /// Pulses a relay for the specified amount of time. Pulsing the relay
        /// temporarily switches the relay on and back off again. This 
        /// controller must be in temperature monitor mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to pulse.
        /// </param>
        /// <param name="pulseTime">
        /// The amount of time (in seconds) to pulse the relay. Default is
        /// <see cref="Common.DEFAULT_PULSE_TIME"/> and the maximum allowable
        /// value is <see cref="Common.MAX_PULSE_DURATION"/>. If the specified
        /// value is less than 0.1, then the default will be used instead.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value greater than or equal to 1 or
        /// less than or equal to <see cref="X300Constants.TOTAL_RELAYS"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// pulseTime cannot be greater than <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.TemperatureMonitor"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
            if ((relayNum < 1) || (relayNum > X300Constants.TOTAL_RELAYS)) {
                String err = String.Format("Must be an integer value 1 - {0}.",
                                            X300Constants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            if (pulseTime > Common.MAX_PULSE_DURATION) {
                String err = String.Format("pulseTime cannot be greater than {0}.",
                                            Common.MAX_PULSE_DURATION.ToString());
                throw new ArgumentException(err, "pulseTime");
            }

            if (this._mode != X300OperationMode.TemperatureMonitor) {
                throw new InvalidOperationException("Must be in temperature monitor mode to set relay.");
            }

            if (pulseTime < 0.1) {
                pulseTime = Common.DEFAULT_PULSE_TIME;
            }

            Int32 pulseState = Common.GetRelayStateCode(RelayState.Pulse);
            String command = String.Format("GET /state.xml?relay{0}State={1}&pulseTime={3}&noReply=1 HTTP/1.1\r\n\r\n",
                                            relayNum.ToString(),
                                            pulseState.ToString(),
                                            pulseTime.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Pulses a relay for the default amount of time. Pulsing the relay
        /// temporarily switches the relay on and back off again. This 
        /// controller must be in temperature monitor mode. See <see cref="Mode"/>.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to pulse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value greater than or equal to 1 or
        /// less than or equal to <see cref="X300Constants.TOTAL_RELAYS"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.TemperatureMonitor"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        /// Sets the state of a specified relay.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to set.
        /// </param>
        /// <param name="rel">
        /// The relay object to apply.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value greater than or equal to 1 or
        /// less than or equal to <see cref="X300Constants.TOTAL_RELAYS"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Mode"/> is not <see cref="X300OperationMode.TemperatureMonitor"/>
        /// - or - The instance has been disposed - or - The device's IP address
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
        public void SetRelay(Int32 relayNum, Relay rel)
		{
			// Bad relay number.
			if ((relayNum < 1) || (relayNum > X300Constants.TOTAL_RELAYS)) {
				String err = String.Format("Must be an integer value 1 - {0}.",
                                            X300Constants.TOTAL_RELAYS.ToString());
				throw new ArgumentOutOfRangeException("relayNum", err);
			}

			// Wrong mode.
			if (this._mode != X300OperationMode.TemperatureMonitor) {
				throw new InvalidOperationException("Must be in temperature monitor mode to set relay.");
			}

			// If the specified relay is null, then we just turn the relay off.
			// New instances of Relay are initialized in the off state.
			if (rel == null) {
				rel = new Relay();
			}

			Int32 state = Common.GetRelayStateCode(rel.State);
			String command = String.Format("GET /state.xml?relay{0}state={1}",
                                            relayNum.ToString(),
                                            state.ToString());
			if (state == Common.RELAY_STATE_PULSE_OR_REBOOT) {
				command = String.Concat(command, String.Format("&pulseTime{0}={1}",
                                                                relayNum.ToString(),
                                                                rel.PulseTime.ToString()));
			}

			// Fire off the command.
			command = String.Concat(command, "&noReply=1 HTTP/1.1\r\n\r\n");
			if (this._authEnabled) {
				command = Common.AppendAuthToCommand(command, this._password);
			}
			this.SendCommand(command);
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
            X300ThermostatState thermoState = null;
            X300TempMonitorState tempMonState = null;
            while (this._isPolling) {
                try {
                    if (this.Mode == X300OperationMode.TemperatureMonitor) {
                        thermoState = this.GetThermostatState();
                        this.OnPolled(new PollStatusEventArgs(thermoState));
                    }
                    else {
                        tempMonState = this.GetTempMonitorState();
                        this.OnPolled(new PollStatusEventArgs(tempMonState));
                    }
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
                catch (IOException ioEx) {
                    fail = ioEx;
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
            thermoState = null;
            tempMonState = null;
        }

        /// <summary>
        /// Begins the poll cycle.
        /// </summary>
        public void BeginPollCycle() {
            if (this._isPolling) {
                return;
            }
            this._pollThread = new Thread(new ThreadStart(this.Poll));
            this._pollThread.IsBackground = true;
            this._pollThread.Name = "X300StatePoller";
            this._isPolling = true;
            this._pollThread.Start();
        }
        #endregion
    }
}
