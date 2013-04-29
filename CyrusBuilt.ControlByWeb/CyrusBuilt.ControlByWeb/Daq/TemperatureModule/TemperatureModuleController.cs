using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.Security;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace CyrusBuilt.ControlByWeb.Daq.TemperatureModule
{
    /// <summary>
    /// Used to monitor and control a ControlByWeb Temperature Module.
    /// </summary>
    public class TemperatureModuleController : IDisposable
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
        /// Occurs when the TemperatureModule device state is polled.
        /// </summary>
        public event PollStatusEventHandler Polled;

        /// <summary>
        /// Occurs when polling the device fails.
        /// </summary>
        public event PollFailEventHandler PollFailed;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.TemperatureModule.TemperatureModuleController</b>
        /// class.  This is the default constructor.
        /// </summary>
        public TemperatureModuleController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.TemperatureModule.TemperatureModuleController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        public TemperatureModuleController(IPAddress ipAddr) {
            this._ip = ipAddr;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.TemperatureModule.TemperatureModuleController</b>
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
        public TemperatureModuleController(IPAddress ipAddr, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ipAddr;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.TemperatureModule.TemperatureModuleController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public TemperatureModuleController(IPEndPoint endpoint) {
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
        /// Parses the device state from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse state information from.
        /// </param>
        /// <returns>
        /// The device state.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        private TemperatureModuleState GetStateFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            TemperatureModuleState state = new TemperatureModuleState();
            String itemText = String.Empty;
            String attrib = String.Empty;
            Double temp = 00.00;

            // The default is Fahrenheit. Change to Celcius if need be.
            itemText = Common.GetNamedChildNode(node, "units").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (itemText.ToUpper() == "C") {
                    state.Units = TemperatureUnits.Celcius;
                }
            }

            // Get the sensor state and temp.  If we got an actual temperature
            // value, then a sensor is assumed to be attached.
            Int32 value = 0;
            for (Int32 i = 1; i <= TempModuleConstants.TOTAL_SENSOR_INPUTS; i++) {
                attrib = String.Format("sensor{0}temp", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (Double.TryParse(itemText, out temp)) {
                    state.GetInput(i).AddSensor(temp);
                }

                // Get the state of both relays.
                for (Int32 x = 1; x <= TempModuleConstants.TOTAL_RELAYS; x++) {
                    attrib = String.Format("relay{0}state", x.ToString());
                    itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                    if (Int32.TryParse(itemText, out value)) {
                        if ((value >= Common.RELAY_STATE_OFF) && 
                            (value <= Common.RELAY_STATE_TOGGLE)) {
                            state.GetRelay(x).State = Common.GetRelayState(value);
                        }
                    }
                }
            }
            return state;
        }

        /// <summary>
        /// Connects to the device and sends the specified command to it.
        /// </summary>
        /// <param name="command">
        /// The command to send to the device. The command should end with
        /// "\r\n\r\n".
        /// </param>
        /// <returns>
        /// If successful, the state of the client after the device recieved
        /// the command. If no response is recieved from the device, null
        /// is returned.
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
        private TemperatureModuleState SendCommand(String command) {
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
            TemperatureModuleState state = null;
            try {
                // Load the XML and build the module state object.
                doc.Load(memStream);
                XmlNode node = doc.SelectSingleNode("/datavalues");
                state = this.GetStateFromXmlNode(node);
            }
            catch (XmlException) {
                throw new BadResponseFromDeviceException(recvBytes);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException(recvBytes);
            }

            // Dump the send and recieve buffers, then return the state.
            Array.Clear(sendBytes, 0, sendBytes.Length);
            Array.Clear(recvBytes, 0, recvBytes.Length);
            return state;
        }

        /// <summary>
        /// Assigns an input to the specified input number in the specified state.
        /// </summary>
        /// <param name="state">
        /// The state to modify.
        /// </param>
        /// <param name="input">
        /// The <see cref="SensorInput"/> to assign to the specified input number.
        /// </param>
        /// <param name="deviceInputNum">
        /// The number assigned to the input to set.
        /// </param>
        /// <returns>
        /// If successful, the modified state. If the specified state is null,
        /// then returns null.
        /// </returns>
        private TemperatureModuleState SetInput(TemperatureModuleState state, SensorInput input, Int32 deviceInputNum) {
            if (state == null) {
                return null;
            }

            if ((deviceInputNum < 1) || (deviceInputNum > TempModuleConstants.TOTAL_SENSOR_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            TempModuleConstants.TOTAL_SENSOR_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("deviceInputNum", err);
            }

            if (input == null) {
                state.GetInput(deviceInputNum).Reset();
            }

            SensorInput si = state.GetInput(deviceInputNum);
            if (!si.Equals(input)) {
				switch (deviceInputNum) {
					case 1: state.Input1 = input; break;
					case 2: state.Input2 = input; break;
					case 3: state.Input3 = input; break;
					case 4: state.Input4 = input; break;
				}
			}

            return state;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the TemperatureModule polled event.
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
        /// Raises the TemperatureModule poll failed event.
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
        /// Connects to the device and gets its current state.
        /// </summary>
        /// <returns>
        /// If successful, returns a new WebRelay state object containing
        /// the device's current state. Returns null if no response received
        /// from the device.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address is
        /// not defined.
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
        public TemperatureModuleState GetState() {
            String command = "GET /state.xml?noReply=0 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            return this.SendCommand(command);
        }

        /// <summary>
        /// Pulses a relay for the specified amount of time. Pulsing the relay
        /// temporarily switches the relay on and back off again.
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
        /// relayNum must be either 1 or 2.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// pulseTime cannot be greater than <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address is
        /// not defined.
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
            if ((relayNum < 1) || (relayNum > TempModuleConstants.TOTAL_RELAYS)) {
                throw new ArgumentOutOfRangeException("relayNum", "Must be either one or two.");
            }

            if (pulseTime > Common.MAX_PULSE_DURATION) {
                String err = String.Format("pulseTime cannot be greater than {0}.",
                                            Common.MAX_PULSE_DURATION.ToString());
                throw new ArgumentException(err, "pulseTime");
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
        /// Pulses a relay for the default amount of time (1.5 seconds).
        /// Pulsing the relay temporarily switches the relay on and back off
        /// again.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to pulse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be either 1 or 2.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address is
        /// not defined.
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
        /// Sets the state of the Temperature module device.
        /// </summary>
        /// <param name="state">
        /// The state to commit to the device.
        /// </param>
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
        public void SetState(TemperatureModuleState state) {
            if (state == null) {
                throw new ArgumentNullException("state");
            }
            String command = String.Empty;
            String unit = "F";
            TemperatureModuleState currentState = this.GetState();

            // If the unit of measure is changing, then set the new
            // unit of measurement.
            if (currentState.Units != state.Units) {
                if (state.Units == TemperatureUnits.Celcius) {
                    unit = "C";
                }

                command = String.Format("GET /state.xml?units={0}&noReply=1 HTTP/1.1\r\n\r\n", unit);
                if (this._authEnabled) {
                    command = Common.AppendAuthToCommand(command, this._password);
                }
                this.SendCommand(command);
            }

            // Set the relay state if the state of the relay is changing.
            // If set to pulse, then pulse the relay.
            Relay tempRel = null;
            Relay rel = null;
            for (Int32 i = 1; i <= TempModuleConstants.TOTAL_RELAYS; i++) {
                rel = null;
                tempRel = state.GetRelay(i);
                if (!currentState.GetRelay(i).Equals(tempRel)) {
                    rel = tempRel;
                }

                if (rel == null) {
                    break;
                }

                if (rel.State == RelayState.Pulse) {
                    this.PulseRelay(i, rel.PulseTime);
                }
                else {
                    command = String.Format("GET /state.xml?relay{0}State={1}&noReply=1 HTTP/1.1\r\n\r\n",
                                        i.ToString(),
                                        Common.GetRelayStateCode(rel.State));
                    if (this._authEnabled) {
                        command = Common.AppendAuthToCommand(command, this._password);
                    }
                    this.SendCommand(command);
                }
            }

            // Set the sensor temperatures (or disable if no sensor) if the
            // state of the input is changing.
            SensorInput tempInput = null;
            SensorInput input = null;
            for (Int32 x = 1; x <= TempModuleConstants.TOTAL_SENSOR_INPUTS; x++) {
                input = null;
                tempInput = state.GetInput(x);
                if (!currentState.GetInput(x).Equals(tempInput)) {
                    input = tempInput;
                }

                if (input == null) {
                    break;
                }

                if (input.HasSensor) {
                    command = String.Format("GET /index.srv?setTemp={0}&{1}=Apply&noReply=1 HTTP/1.1\r\n\r\n",
                                            input.Temperature.ToString(),
                                            x.ToString());
                }
                else {
                    command = String.Format("GET /index.srv?setTemp=XX.X&{0}=Apply&noReply=1 HTTP/1.1\r\n\r\n",
                                            x.ToString());
                }

                if (this._authEnabled) {
                    command = Common.AppendAuthToCommand(command, this._password);
                }
                this.SendCommand(command);
            }
        }

        /// <summary>
        /// Adds a sensor to the specified input. If the specified input is
        /// null (stateless) a new <see cref="SensorInput"/> will be created
        /// and a applied to the specified input with the sensor enabled.
        /// </summary>
        /// <param name="inputNum">
        /// The number identifying the input to add the sensor to.
        /// </param>
        /// <returns>
        /// The current state of the device.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// inputNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The instance has been disposed - or - The device's IP address is
        /// not defined.
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
        public TemperatureModuleState AddOrReplaceSensorInput(Int32 inputNum) {
            if ((inputNum < 1) || (inputNum > TempModuleConstants.TOTAL_SENSOR_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            TempModuleConstants.TOTAL_SENSOR_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("inputNum", err);
            }

            // First we have to get the current state of the device.
            TemperatureModuleState state = this.GetState();
            if (state == null) {
                // We could connect to the device but we got no response.
                return null;
            }

            // Get the specified sensor input.
            SensorInput input = state.GetInput(inputNum);
            if (input == null) {
                input = new SensorInput();
            }

            // If the specified input already has a sensor, remove it (replace).
            if (input.HasSensor) {
                input.RemoveSensor();
            }

            // Add a new sensor and commit the altered state to the device.
            input.AddSensor();
            state = this.SetInput(state, input, inputNum);
            this.SetState(state);
            return this.GetState();
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
            TemperatureModuleState state = null;
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
            state = null;
            fail = null;
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
            this._pollThread.Name = "tempModuleStatePoller";
            this._isPolling = true;
            this._pollThread.Start();
        }
        #endregion
    }
}
