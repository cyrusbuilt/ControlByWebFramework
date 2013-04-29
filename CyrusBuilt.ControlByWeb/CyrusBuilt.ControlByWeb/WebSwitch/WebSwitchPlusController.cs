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

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// Used to monitor and control a ControlyByWeb WebSwitch Plus module.
    /// </summary>
    public class WebSwitchPlusController : IDisposable
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
        /// Occurs when polling the device status fails.
        /// </summary>
        public event PollFailEventHandler PollFailed;
        #endregion

        #region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusController"/> 
		/// class. This is the default constructor.
		/// </summary>
		public WebSwitchPlusController() {
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusController"/>
		/// class with the IP address of the module to control.
		/// </summary>
		/// <param name="ipAddr">
		/// The IP address of the module to control.
		/// </param>
		public WebSwitchPlusController(IPAddress ipAddr) {
			this._ip = ipAddr;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusController"/> 
		/// class with the IP address of the module to control and the port to 
		/// communicate with it on.
		/// </summary>
		/// <param name="ipAddr">
		/// The IP address of the module to control.
		/// </param>
		/// <param name="port">
		/// The port to communicate with the device on.
		/// </param>
		/// <exception cref='ArgumentOutOfRangeException'>
		/// The port number assignment is invalid. Must be 1 - 65535.
		/// </exception>
		public WebSwitchPlusController(IPAddress ipAddr, Int32 port) {
			if ((port < 1) || (port > 65535)) {
				throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
			}
			this._ip = ipAddr;
			this._port = port;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusController"/> 
		/// class with the IP endpoint to connect to.
		/// </summary>
		/// <param name="endpoint">
		/// The IP endpoint to connect to.
		/// </param>
		public WebSwitchPlusController(IPEndPoint endpoint) {
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
        /// Parses the device state from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse the device state info from.
        /// </param>
        /// <returns>
        /// The state of the device.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> cannot be null.
        /// </exception>
        private WebSwitchPlusState GetStateFromXmlNode(XmlNode node) {
			if (node == null) {
				throw new ArgumentNullException("node");
			}

			WebSwitchPlusState state = new WebSwitchPlusState();
			String itemText = String.Empty;
			String attrib = String.Empty;
			Relay rel = null;
			Int32 relState = 0;
			Int32 val = 0;
			RebootState rebState = RebootState.Pinging;
			
			// Outputs.
			for (Int32 r = 1; r <= WebSwitchConstants.TOTAL_RELAYS; r++) {
				// Relays.
				val = 0;
				relState = 0;
				rel = null;
				attrib = String.Format("relay{0}state", r.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out relState)) {
						rel = new Relay(Common.GetRelayState(relState));
						state.SetRelay(r, rel);
					}
				}
				
				// Reboot state.
				rebState = RebootState.Pinging;
				attrib = String.Format("reboot{0}state", r.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out val)) {
						rebState = Common.GetRebootState(val);
						switch (r) {
							case 1:
								state.SetOrOverrideRebootState1(rebState);
								break;
							case 2:
								state.SetOrOverrideRebootState2(rebState);
								break;
						}
					}
				}
				
				// Reboot failures.
				val = 0;
				attrib = String.Format("failures{0}", r.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out val)) {
						switch (r) {
							case 1:
								state.SetOrOverrideRebootFailures1(val);
								break;
							case 2:
								state.SetOrOverrideRebootFailures2(val);
								break;
						}
					}
				}
				
				// Reboot attempts.
				val = 0;
				attrib = String.Format("rbtAttempts{0}", r.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out val)) {
						switch (r) {
							case 1:
								state.SetOrOverrideRebootAttempts1(val);
								break;
							case 2:
								state.SetOrOverrideRebootAttempts2(val);
								break;
						}
					}
				}
				
				// Total reboots.
				val = 0;
				attrib = String.Format("totalreboots{0}", r.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out val)) {
						switch (r) {
							case 1:
								state.SetOrOverrideTotalReboots1(val);
								break;
							case 2:
								state.SetOrOverrideTotalReboots2(val);
								break;
						}
					}
				}
			}
			
			// Standard inputs.
			InputState inState = InputState.Off;
			StandardInput inp = null;
			for (Int32 i = 1; i <= WebSwitchConstants.TOTAL_INPUTS; i++) {
				inp = null;
				val = 0;
				attrib = String.Format("input{0}state", i.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Int32.TryParse(itemText, out val)) {
						inState = InputState.Off;
						if (val == 1) {
							inState = InputState.On;
						}
						inp = new StandardInput(inState);
						state.SetInput(i, inp);
					}
				}
			}
			
			// Temperature units.
			itemText = Common.GetNamedChildNode(node, "units").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (itemText.ToLower() == "c") {
					state.SetOrOverrideTempUnits(TemperatureUnits.Celcius);
				}
			}
			
			// Sensor inputs.
			Double temp = 0.0;
			for (Int32 s = 1; s <= WebSwitchConstants.TOTAL_SENSORS; s++) {
				temp = 0.0;
				attrib = String.Format("sensor{0}temp", s.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Double.TryParse(itemText, out temp)) {
						state.SetSensor(s, new SensorInput(temp));
					}
				}
			}
			
			// External variables.
			for (Int32 e = WebSwitchConstants.MIN_EXT_VAR_ID; e<= WebSwitchConstants.MAX_EXT_VAR_ID; e++) {
				temp = 0.0;
				attrib = String.Format("extvar{0}", e.ToString());
				itemText = Common.GetNamedChildNode(node, attrib).InnerText;
				if (!String.IsNullOrEmpty(itemText)) {
					if (Double.TryParse(itemText, out temp)) {
						state.SetExternalVar(e, temp);
					}
				}
			}
			
			// Serial number.
			itemText = Common.GetNamedChildNode(node, "serialNumber").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				try {
					state.SetOrOverrideSerial(itemText);
				}
				catch (FormatException) {
				}
			}
			
			// Time.
            Int64 time = 0;
			itemText = Common.GetNamedChildNode(node, "time").InnerText;
			if (!String.IsNullOrEmpty(itemText)) {
				if (Int64.TryParse(itemText, out time)) {
					state.SetOrOverrideTime(time);
				}
			}
			return state;
		}

        /// <summary>
        /// Gets event info from the specified XML node.
        /// </summary>
        /// <param name="node">
        /// The XML node to parse the event data from.
        /// </param>
        /// <returns>
        /// The device event as parsed from the specified node.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node"/> cannot be null.
        /// </exception>
        private WebSwitchPlusEvent GetEventFromXmlNode(XmlNode node) {
			if (node == null) {
				throw new ArgumentNullException("node");
			}

			WebSwitchPlusEvent evt = new WebSwitchPlusEvent();
			DateTime time = DateTime.MinValue;
			Int32 val = 0;
			String itemText = String.Empty;
            Double duration = 0.0;
            WebSwitchAction action = WebSwitchAction.None;

            // TODO need to test this somehow.
            // Get the ID of the event.  We'll have to go up one level and parse
            // it from the parent node. <event0> or <event1> for example.
            String parentName = node.ParentNode.Name;
            if (parentName.Contains("event")) {
                Int32 startPos = parentName.IndexOf("event");
                itemText = parentName.Substring((startPos + 5), 1);
                if (Int32.TryParse(itemText, out val)) {
                    evt.SetOrOverrideId(val);
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
            val = 0;
            itemText = Common.GetNamedChildNode(node, "count").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out val)) {
                    evt.SetOrOverrideCount(val);
                }
            }

            // Get the relay of this event.
            val = 0;
            itemText = Common.GetNamedChildNode(node, "relay").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out val)) {
                    if ((val > 0) && (val < 3)) {
                        evt.SetOrOverrideRelay(val);
                    }
                }
            }

            // Get action of this event.
            itemText = Common.GetNamedChildNode(node, "action").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                action = WebSwitchUtils.GetActionFromString(itemText);
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
        private WebSwitchPlusDiagnostics GetDiagsFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            WebSwitchPlusDiagnostics diags = new WebSwitchPlusDiagnostics();
            String itemText = String.Empty;
            Int32 value = 0;

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
        /// Raises the <see cref="Polled"/> event if a handler is assigned.
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
        /// Raises the <see cref="PollFailed"/> event if a handler is assigned.
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
        /// Gets the state of the WebSwitch Plus device.
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
        public WebSwitchPlusState GetState() {
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
        /// If successful, a <see cref="WebSwitchPlusDiagnostics"/> state object
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
        public WebSwitchPlusDiagnostics GetDiagnostics() {
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
        /// Retrieves an event from the WebSwitch Plus device.
        /// </summary>
        /// <param name="id">
        /// The ID of the event to retrieve.
        /// </param>
        /// <returns>
        /// If successful, the requested event; Otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be greater than or equal to <see cref="EventConstants.EVENT_MIN_ID"/>
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
        public WebSwitchPlusEvent GetEvent(Int32 id) {
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
        /// Gets a collection of events from the WebSwitch Plus device.
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
        public WebSwitchPlusEventCollection GetEvents() {
            WebSwitchPlusEvent evt = null;
            WebSwitchPlusEventCollection eventColl = new WebSwitchPlusEventCollection();
            for (Int32 i = EventConstants.EVENT_MIN_ID; i <= EventConstants.EVENT_MAX_ID; i++) {
                evt = this.GetEvent(i);
                if (evt != null) {
                    eventColl.Add(evt);
                }
            }
            evt = null;
            return eventColl;
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
            WebSwitchPlusState state = null;
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
            this._pollThread.Name = "WebSwitchStatePoller";
            this._isPolling = true;
            this._pollThread.Start();
        }

        /// <summary>
        /// Commits the set to the device. Currently, only the relay states
        /// will be committed, all other values will be ignored.
        /// </summary>
        /// <param name="state">
        /// The state to commit to the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> cannot be null.
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
        public void SetState(WebSwitchPlusState state) {
            if (state == null) {
                throw new ArgumentNullException("state");
            }

            // First we get the current state for comparison. We will only
            // commit values that have actually changed.
            WebSwitchPlusState currentState = this.GetState();
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
            for (Int32 i = 1; i <= WebSwitchConstants.TOTAL_RELAYS; i++) {
                rel = null;
                switch (i) {
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
                                            i.ToString(),
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
        /// Resets the state of the WebSwitch Plus module to its defaults.
        /// </summary>
        /// <remarks>
        /// This method only affects the state of the relays and counters, all
        /// other state values are ignored.
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
            WebSwitchPlusState state = this.GetState();
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
            if ((relayNum < 1) || (relayNum > WebSwitchConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            WebSwitchConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Get the current state for comparison.
            WebSwitchPlusState state = this.GetState();
            if (state == null) {
                return;
            }

            // Skip if the relay is already on.
            if (((relayNum == 1) && (state.Relay1.State == RelayState.On)) ||
                ((relayNum == 2) && (state.Relay2.State == RelayState.On))) {
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
            if ((relayNum < 1) || (relayNum > WebSwitchConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            WebSwitchConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            // Get the current state for comparison.
            WebSwitchPlusState state = this.GetState();
            if (state == null) {
                return;
            }

            // Skip if the relay is already off.
            if (((relayNum == 1) && (state.Relay1.State == RelayState.Off)) ||
                ((relayNum == 2) && (state.Relay2.State == RelayState.Off))) {
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
            if ((relayNum < 1) || (relayNum > WebSwitchConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            WebSwitchConstants.TOTAL_RELAYS.ToString());
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
            WebSwitchPlusState state = this.GetState();
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

        // TODO Event *set* support.  As of right now, this is kinda of tough
        // for a couple reasons:
        // 1.) Like the X-301, you can *set* an event description, but not *get*
        // the description.
        // 2.) You can *set* the days that an event will occur, but you cannot
        // *get* the days that the event is occurring on. It is hard to modify
        // a current schedule if you don't know what the original values are.
        // 3.) *Getting* an event will show 1 or 2 for the relay (output) number,
        // but when *setting* an event, you specify 0 - Relay 1, 1 - Relay 2, 2 - both.
        // In one of the other modules, 1,2 - is used to indicate both when
        // getting an event.  WTF? I'm hoping to clarify all this with Xytronix.
        #endregion
    }
}
