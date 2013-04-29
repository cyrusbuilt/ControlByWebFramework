using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.Security;
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

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
    /// <summary>
    /// Used to monitor and control a ControlyByWeb X-301 module.
    /// </summary>
    public class X301ModuleController : IDisposable
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
        /// Occurs when the device state is polled.
        /// </summary>
        public event PollStatusEventHandler Polled;

        /// <summary>
        /// Occurs when polling the device state fails.
        /// </summary>
        public event PollFailEventHandler PollFailed;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X301Module.X301ModuleController</b>
        /// class. This is the default constructor.
        /// </summary>
        public X301ModuleController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X301Module.X301ModuleController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        public X301ModuleController(IPAddress ipAddr) {
            this._ip = ipAddr;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X301Module.X301ModuleController</b>
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
        public X301ModuleController(IPAddress ipAddr, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ipAddr;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X301Module.X301ModuleController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public X301ModuleController(IPEndPoint endpoint) {
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
        private X301Diagnostics GetDiagsFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X301Diagnostics diags = new X301Diagnostics();
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
        /// Parses device state info from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse state information from.
        /// </param>
        /// <returns>
        /// An object containing state information for the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        private X301State GetStateFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X301State state = new X301State();
            String itemText = String.Empty;
            String attrib = String.Empty;
            Int32 iValue = 0;
            Double dValue = 0.0;
            Int64 unixTime = 0;
            Relay rel = null;
            StandardInput input = null;
            RelayState relState = RelayState.Off;
            InputState inState = InputState.Off;
            Int32 count = 0;

            // Get the relay states.
            for (Int32 r = 1; r <= X301Constants.TOTAL_RELAYS; r++) {
                rel = null;
                relState = RelayState.Off;
                iValue = 0;
                attrib = String.Format("relay{0}state", r.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out iValue)) {
                        relState = Common.GetRelayState(iValue);
                        rel = new Relay(relState);
                        switch (r) {
                            case 1: state.Relay1 = rel; break;
                            case 2: state.Relay2 = rel; break;
                        }
                    }
                }
            }

            // Get the input states.
            for (Int32 i = 1; i <= X301Constants.TOTAL_INPUTS; i++) {
                input = null;
                inState = InputState.Off;
                iValue = 0;
                attrib = String.Format("input{0}state", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out iValue)) {
                        if (iValue == Common.INPUT_STATE_ON) {
                            inState = InputState.On;
                        }
                    }
                }

                count = 0;
                iValue = 0;
                attrib = String.Format("count{0}", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out iValue)) {
                        count = iValue;
                    }
                }

                input = new StandardInput(inState, count);
                switch (i) {
                    case 1: state.SetInput1(input); break;
                    case 2: state.SetInput2(input); break;
                }
            }

            // Get high times. *snicker*
            for (Int32 h = 1; h <= X301Constants.TOTAL_INPUTS; h++) {
                dValue = 0.0;
                attrib = String.Format("hightime{0}", h.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out dValue)) {
                        switch (h) {
                            case 1: state.SetHighTime1(dValue); break;
                            case 2: state.SetHighTime2(dValue); break;
                        }
                    }
                }
            }

            // Get the external variable values.
            for (Int32 e = X301Constants.EXT_VAR_MIN_ID; e <= X301Constants.EXT_VAR_MAX_ID; e++) {
                dValue = 0.0;
                attrib = String.Format("extvar{0}", e.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out dValue)) {
                        state.SetExternalVar(e, dValue);
                    }
                }
            }

            // Get the serial number (same as MAC address).
            itemText = Common.GetNamedChildNode(node, "serialNumber").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                // Try to parse the serial number (MAC). PhysicalAddress has a
                // Parse() method, but no TryParse(). We'll compensate for that
                // here.
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

            // Get the epoch time.
            itemText = Common.GetNamedChildNode(node, "time").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int64.TryParse(itemText, out unixTime)) {
                    state.SetTime(unixTime);
                }
            }
            return state;
        }

        /// <summary>
        /// Parses device event info from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse event information from.
        /// </param>
        /// <returns>
        /// An object containing event information for the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// node cannot be null.
        /// </exception>
        private X301Event GetEventFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            X301Event evt = new X301Event();
            String itemText = String.Empty;
            Int32 value = 0;
            DateTime time = DateTime.MinValue;
            Double duration = 0.0;
            X301Action action = X301Action.None;

            // TODO need to test this somehow.
            // Get the ID of the event.  We'll have to go up one level and parse
            // it from the parent node. <event0> or <event1> for example.
            String parentName = node.ParentNode.Name;
            if (parentName.Contains("event")) {
                Int32 startPos = parentName.IndexOf("event");
                itemText = parentName.Substring((startPos + 5), 1);
                if (Int32.TryParse(itemText, out value)) {
                    evt.SetOrOverrideId(value);
                }
            }
            else {
                // TODO Return null or throw exception?
            }

            // Get whether or not this event is active.
            itemText = Common.GetNamedChildNode(node, "active").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (itemText == "yes") {
                    evt.SetOrOverrideActiveState(true);
                }
            }

            // Get the current time as reported by the device.
            itemText = Common.GetNamedChildNode(node, "currentTime").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (DateTime.TryParse(itemText, out time)) {
                    evt.SetOrOverrideCurrentTime(time);
                }
            }

            // Get the time of the next event occurrance.
            itemText = Common.GetNamedChildNode(node, "nextEvent").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (DateTime.TryParse(itemText, out time)) {
                    evt.SetOrOverrideNextEvent(time);
                }
            }

            // Get the period of time between event occurrences (only for
            // recurring events).
            itemText = Common.GetNamedChildNode(node, "period").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                evt.SetOrOverridePeriod(itemText);
            }

            // Gets the count of remaining times this event will occur.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "count").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    evt.SetOrOverrideCount(value);
                }
            }

            // Get the relay of this event.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "relay").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    if ((value > 0) && (value < (X301Constants.TOTAL_RELAYS + 1))) {
                        evt.SetOrOverrideRelay(value);
                    }
                }
            }

            // Get action of this event.
            itemText = Common.GetNamedChildNode(node, "action").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                action = X301Utils.GetActionFromString(itemText);
                evt.SetOrOverrideAction(action);
            }

            // Get the pulse duration.
            itemText = Common.GetNamedChildNode(node, "pulseDuration").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out duration)) {
                    evt.SetOrOverridePulseDuration(duration);
                }
            }
            return evt;
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

            // Dump the send and recieve buffers, then return the XML response.
            Array.Clear(sendBytes, 0, sendBytes.Length);
            Array.Clear(recvBytes, 0, recvBytes.Length);
            return doc;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the X-301 polled event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected virtual void OnPolled(PollStatusEventArgs e) {
            if (this.Polled != null) {
                this.Polled(this, e);
            }
        }

        /// <summary>
        /// Raises the X-301 poll failed event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected virtual void OnPollFailed(PollFailedEventArgs e) {
            if (this.PollFailed != null) {
                this.PollFailed(this, e);
            }
        }

        /// <summary>
        /// Gets diagnostic information from the device.
        /// </summary>
        /// <returns>
        /// If successful, a <see cref="X301Diagnostics"/> state object
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
        public X301Diagnostics GetDiagnostics() {
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
        /// Connects to the device and gets its current state.
        /// </summary>
        /// <returns>
        /// If successful, returns a new <see cref="X301State"/> object containing
        /// the device's current state. Returns null if no response received
        /// from the device.
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
        public X301State GetState() {
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
        /// Sets the state of the X-301 device.
        /// </summary>
        /// <param name="state">
        /// The state to set. This state is what will be committed to the device.
        /// </param>
        /// <remarks>
        /// This method only affects the state of the relays, all other
        /// properties of the specified state object are ignored.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// state cannot be null.
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
        public void SetState(X301State state) {
            if (state == null) {
                throw new ArgumentNullException("state");
            }

            // First we get the current state for comparison.
            X301State currentState = this.GetState();
            if (currentState == null) {
                return;
            }

            // We essentially construct and send 2 commands here, one for
            // each relay.
            String command = String.Empty;
            Relay rel = null;
            Int32 stateCode = Common.RELAY_STATE_OFF;
            String relCmd = String.Empty;
            String pulseCmd = String.Empty;
            for (Int32 r = 1; r <= X301Constants.TOTAL_RELAYS; r++) {
                command = "GET /state.xml?";
                stateCode = Common.RELAY_STATE_OFF;
                relCmd = String.Empty;
                pulseCmd = String.Empty;

                // Compare the state of the relay we are setting to the current
                // state of the relay. We don't do anything if we aren't
                // actually changing anything.
                rel = null;
                switch (r) {
                    case 1:
                        if (!currentState.Relay1.Equals(state.Relay1)) {
                            rel = state.Relay1;
                        }
                        break;
                    case 2:
                        if (!currentState.Relay2.Equals(state.Relay2)) {
                            rel = state.Relay2;
                        }
                        break;
                }

                // Construct the command if we are changing states.
                if (rel != null) {
                    stateCode = Common.GetRelayStateCode(rel.State);
                    relCmd = String.Format("relay{0}state={1}", 
                                            r.ToString(), 
                                            stateCode.ToString());

                    // Append pulse time if we are pulsing the relay.
                    command = String.Concat(command, relCmd);
                    if (stateCode == Common.RELAY_STATE_PULSE_OR_REBOOT) {
                        pulseCmd = String.Format("&pulseTime={0}", 
                                                    rel.PulseTime.ToString());
                        command = String.Concat(command, pulseCmd);
                    }

                    // Finalize and send.
                    command = String.Concat(command, "&noReply=1 HTTP/1.1\r\n\r\n");
                    if (this._authEnabled) {
                        command = Common.AppendAuthToCommand(command, this._password);
                    }
                    this.SendCommand(command);
                }
            }
            currentState = null;
        }

        /// <summary>
        /// Resets the state of the X-301 module to its defaults.
        /// </summary>
        /// <remarks>
        /// This method only affects the state of the relays, all other
        /// properties of the device state are ignored.
        /// </remarks>
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
        public void ResetState() {
            X301State state = this.GetState();
            if (state == null) {
                return;
            }
            state.Reset();
            this.SetState(state);
            state = null;
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
            if ((relayNum < 1) || (relayNum > X301Constants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X301Constants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Get the current state for comparison.
            X301State state = this.GetState();
            if (state == null) {
                return;
            }

            // Skip if the relay is already on.
            if (state.Relay1.State == RelayState.On) {
                return;
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
            if ((relayNum < 1) || (relayNum > X301Constants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X301Constants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Get the current state for comparison.
            X301State state = this.GetState();
            if (state == null) {
                return;
            }

            // Skip if the relay is already off.
            if (state.Relay1.State == RelayState.Off) {
                return;
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
            if ((relayNum < 1) || (relayNum > X301Constants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X301Constants.TOTAL_RELAYS.ToString());
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

            // Get the current state for comparison.
            X301State state = this.GetState();
            if (state == null) {
                return;
            }

            // If we're already pulsing and for the same pulse time, then we
            // skip it.
            Relay thisRel = new Relay(RelayState.Pulse, pulseTime);
            Boolean doPulse = false;
            switch (relayNum) {
                case 1:
                    if (!state.Relay1.Equals(thisRel)) {
                        doPulse = true;
                    }
                    break;
                case 2:
                    if (!state.Relay2.Equals(thisRel)) {
                        doPulse = true;
                    }
                    break;
            }

            // We're ok to proceed with the pulse command.
            if (doPulse) {
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
        /// Retrieves an event from the X-301 device.
        /// </summary>
        /// <param name="id">
        /// The ID of the event to retrive.
        /// </param>
        /// <returns>
        /// If successful, the requested event; Otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// id must be greater than or equal to <see cref="EventConstants.EVENT_MIN_ID"/>
        /// and less than or equal to <see cref="EventConstants.EVENT_MAX_ID"/>.
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
        public X301Event GetEvent(Int32 id) {
            if ((id < EventConstants.EVENT_MIN_ID) || (id > EventConstants.EVENT_MAX_ID)) {
                String err = String.Format("id must be {0} - {1}.",
                                            EventConstants.EVENT_MIN_ID.ToString(),
                                            EventConstants.EVENT_MAX_ID.ToString());
                throw new ArgumentOutOfRangeException("id", err);
            }

            // Build the request.
            String eventName = String.Format("event{0}", id.ToString());
            String command = String.Format("GET /{0}.xml HTTP/1.1\r\n\r\n", eventName);
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }

            // Retrieve the event.
            XmlDocument response = this.SendCommand(command);
            if (response != null) {
                try {
                    XmlNode node = response.SelectSingleNode("/" + eventName);
                    return this.GetEventFromXmlNode(node);
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }
            response = null;
            return null;
        }

        /// <summary>
        /// Gets a collection of events from the X-301 device.
        /// </summary>
        /// <returns>
        /// A collection of events retrieved from the device. If no events
        /// exist, then an empty collection is returned.
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
        public X301EventCollection GetEvents() {
            X301Event evt = null;
            X301EventCollection evtColl = new X301EventCollection();
            for (Int32 i = EventConstants.EVENT_MIN_ID; i <= EventConstants.EVENT_MAX_ID; i++) {
                evt = this.GetEvent(i);
                if (evt == null) {
                    continue;
                }
                evtColl.Add(evt);
            }
            evt = null;
            return evtColl;
        }

        /// <summary>
        /// DO NOT USE THIS METHOD AS IT IS NOT CURRENTLY FULLY SUPPORTED.
        /// </summary>
        /// <param name="evt">
        /// Null.
        /// </param>
        /// <exception cref="UnsupportedMethodException">
        /// A call to this method was made, which is currently unsupported.
        /// </exception>
        public void SetEvent(X301Event evt) {
            throw new UnsupportedMethodException("SetEvent()");

            if (evt == null) {
                throw new ArgumentNullException("evt");
            }

            Epoch startTime = Epoch.FromDateTime(evt.NextEvent);
            PeriodUnits unit = PeriodUnits.Disabled;
            String period = evt.Period.Trim();
            String periodAmt = period.Substring(0);
            String periodUnitStr = period.Substring(period.Length - 1);

            Int32 temp = 0;
            Int32 periodVal = 0;
            if (Int32.TryParse(periodAmt, out temp)) {
                periodVal = temp;
            }

            Int32 periodUnit = EventUtils.GetPeriodCode(unit);
            if (periodUnit == EventConstants.PERIOD_DISABLED) {
                // if periodVal == 0 and periodUnit == 0, then event is disabled.
                periodVal = 0;
                periodUnit = 0;
            }

            Int32 action = X301Utils.GetActionCode(evt.Action);

            // When setting an event, the next occurrence value should always be zero.
            Int32 nextOccurrence = 0;

            // TODO: Need to figure out days.  Days are not reported by event status.
            // The only thing I can think of is to have a collection that
            // stores days of the week and only allows one instance of each day
            // in the collection which would be a property of X301Event and
            // only used when setting the event.

            // Days are computed in a single byte. Each bit representing a different day.
            // Setting the bit value to 1 enables that day. Below are the Base-10 
            // values for each day:
            // Saturday  = 1
            // Friday    = 2
            // Thursday  = 4
            // Wednesday = 8
            // Tuesday   = 16
            // Monday    = 32
            // Sunday    = 64
            // Add the Base-10 values for each day together and pass the sum as
            // parameter 10. For example, an event that occurs everyday would
            // equal 127.

            // 0  - Event ID.
            // 1  - Start time (epoch).
            // 2  - Period.
            // 3  - Period units.
            // 4  - Recurrence count.
            // 5  - Relay number (0 - Relay 1, 1 - Relay 2, 2 - Both).
            // 6  - Relay action.
            // 7  - Pulse duration (use 0 if not pulsing).
            // 8  - Next occurrence.
            // 9  - Description
            // 10 - Days (Base10).
            // For relay number, there is currently no way to account for a
            // "both relays" condition since (according to the manual) the
            // event status returned indicates 1 for relay one and 2 for relay
            // two, but does not mention a code for both (as of 12/2/2011).
            // The event status page, however, will apparently only report relay
            // 1 or 2. I have no idea what it returns for both, as it is not
            // documented.

            // Additionally, the event status page also does not return the
            // event description either.
            // See http://www.controlbyweb.com/x301/x301_Manualv1.0.pdf
            String command = String.Format("GET /eventSetup.srv?e{0}={1};{2};{3};{4};{5};{6};{7};{8};{9};{10}; HTTP/1.1\r\n\r\n",
                                            evt.Id.ToString(),
                                            startTime.ToString(),
                                            periodVal.ToString(),
                                            periodUnit.ToString(),
                                            evt.Count.ToString(),
                                            (evt.RelayNumber - 1).ToString(),
                                            action.ToString(),
                                            evt.PulseDuration.ToString(),
                                            nextOccurrence.ToString(),
                                            evt.Description);

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
            X301State state = null;
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
            fail = null;
            state = null;
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
            this._pollThread.Name = "X301StatePoller";
            this._isPolling = true;
            this._pollThread.Start();
        }
        #endregion
    }
}
