using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.Security;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Used to monitor and control a ControlByWeb WebRelay Quad module.
    /// </summary>
    public class WebRelayQuadController : IDisposable
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
		/// Occurs when polling the device fails.
		/// </summary>
		public event PollFailEventHandler PollFailed;
		#endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayQuadController</b>
        /// class. This is the default controller.
        /// </summary>
        public WebRelayQuadController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayQuadController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        public WebRelayQuadController(IPAddress ipAddr) {
            this._ip = ipAddr;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayQuadController</b>
        /// class with the IP address of the module to control and the port to 
        /// communicate with it on.
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
        public WebRelayQuadController(IPAddress ipAddr, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ipAddr;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayQuadController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public WebRelayQuadController(IPEndPoint endpoint) {
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
        private WebRelayQuadState GetStateFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            String attrib = String.Empty;
            String itemText = String.Empty;
            Int32 value = 0;
            WebRelayQuadState state = new WebRelayQuadState();
            for (Int32 i = 1; i <= 4; i++) {
                // Find the child node and get its value.
                attrib = String.Format("relay{0}state", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                
                // Get the state from the retrieved value.
                RelayState relState = RelayState.Off;
                if (Int32.TryParse(itemText, out value)) {
                    relState = Common.GetRelayState(value);
                }

                // Set the state.
                state.GetRelay(i).State = relState;
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
        private WebRelayQuadState SendCommand(String command) {
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
				// No response from device.
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
			WebRelayQuadState state = null;
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
        #endregion

        #region Methods
		/// <summary>
		/// Raises the polled event.
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
		/// Raises the poll failed event.
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
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public WebRelayQuadState GetState() {
            String command = "GET /state.xml?noReply=0 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            return this.SendCommand(command);
        }

        /// <summary>
        /// Pulses the specified relay for the specified amount of time.
        /// Pulsing the relay temporarily switches the relay on and back off
        /// again.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to pulse.
        /// </param>
        /// <param name="pulseTime">
        /// The amount of time (in seconds) to pulse the relay.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// pulseTime cannot be greater than <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void PulseRelay(Int32 relayNum, Double pulseTime) {
            if ((relayNum < 1) || (relayNum > 4)) {
                throw new ArgumentOutOfRangeException("relayNum", "Must be a value of 1 - 4.");
            }

            if (pulseTime < 0.1) {
                return;
            }

            if (pulseTime > Common.MAX_PULSE_DURATION) {
                String err = String.Format("pulseTime cannot be greater than {0}.",
                                            Common.MAX_PULSE_DURATION.ToString());
                throw new ArgumentException(err, "pulseTime");
            }

            Int32 pulseState = Common.GetRelayStateCode(RelayState.Pulse);
            String command = String.Format("GET /state.xml?relay{0}state={1}&pulseTime={2}&noReply=1 HTTP/1.1\r\n\r\n",
                                            relayNum.ToString(),
                                            pulseState.ToString(),
                                            pulseTime.ToString());
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            this.SendCommand(command);
        }

        /// <summary>
        /// Pulses the specified relay for the default amount of time (1.5 
        /// seconds). Pulsing the relay temporarily switches the relay on 
        /// and back off again.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to pulse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
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
        /// Sets the state of WebRelay Quad device.
        /// </summary>
        /// <param name="state">
        /// The state to set. This state is what will be committed to the device.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// state cannot be null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void SetState(WebRelayQuadState state) {
            if (state == null) {
                throw new ArgumentNullException("state");
            }

            // Get the current state for comparison.
            WebRelayQuadState currentState = this.GetState();
            if (currentState == null) {
                return;
            }

            // Iterate through the relays and build the appropriate command.
            Int32 stateCode = Common.RELAY_STATE_OFF;
            String relCmd = String.Empty;
            String pulseCmd = String.Empty;
            String command = "GET /state.xml?";
            Relay tempRel = null;
            Relay rel = null;
            for (Int32 i = 1; i <= 4; i++) {
                rel = null;
                tempRel = state.GetRelay(i);
                if (!currentState.GetRelay(i).Equals(tempRel)) {
                    rel = tempRel;
                }

                // Disassociated relay or the state isn't changing. Do nothing.
                if (rel == null) {
                    continue;
                }

                // Build the state command for the current relay.
                stateCode = Common.GetRelayStateCode(rel.State);
                relCmd = String.Format("relay{0}state={1}", 
                                        i.ToString(), 
                                        stateCode.ToString());

                // If we are pulsing, then we need to add the pulse time.
                if (stateCode == Common.RELAY_STATE_PULSE_OR_REBOOT) {
                    pulseCmd = String.Format("&pulseTime{0}={1}",
                                                i.ToString(),
                                                rel.PulseTime.ToString());
                    relCmd = String.Concat(relCmd, pulseCmd);
                }
                command = String.Concat(command, relCmd);
            }

            // Finalize the command. We do not expect a response back.
            command = String.Concat(command, "&noReply=1 HTTP/1.1\r\n\r\n");
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }

            // Send it.
            this.SendCommand(command);
        }

        /// <summary>
        /// Resets the state of the WebRelay Quad module to its defaults.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public void ResetState() {
            WebRelayQuadState state = this.GetState();
            if (state == null) {
                return;
            }
            state.Reset();
            this.SetState(state);
        }

        /// <summary>
        /// Changes the state of a specific relay.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to change the state of.
        /// </param>
        /// <param name="state">
        /// The state to set.
        /// </param>
        /// <returns>
        /// The current state of the device. If no response recieved from the
        /// device, then returns null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public WebRelayQuadState ChangeRelayState(Int32 relayNum, RelayState state) {
            if ((relayNum < 1) || (relayNum > 4)) {
                throw new ArgumentOutOfRangeException("relayNum", "Must be a value 1 - 4.");
            }

            // We get the current state for comparison. If the relay state we
            // are attempting to apply is no different than the current state,
            // then we'll do nothing and return the current state as-is. If we
            // can't get the current state, return null.
            WebRelayQuadState currentState = this.GetState();
            if (currentState == null) {
                return null;
            }

            // Find the specified relay and compare state.
            String command = "GET /state.xml?";
            String relCmd = String.Empty;
            Relay rel1 = null;
            Relay rel = null;
            for (Int32 i = 1; i <= 4; i++) {
                if (i == relayNum) {
                    rel1 = currentState.GetRelay(i);
                    if ((rel1 == null) || (rel1.State != state)) {
                        rel = rel1;
                    }
                    break;
                }
            }

            // The state we are trying to apply is the same as the current
            // state or the relay is disassociated. Just return the current
            // state.
            if (rel == null) {
                return currentState;
            }

            // We are changing state. Build the command.
            Int32 stateCode = Common.GetRelayStateCode(state);
            relCmd = String.Format("relay{0}state={1}", 
                                    relayNum.ToString(),
                                    stateCode.ToString());

            // If pulsing, go ahead and pulse for the default time.
            // If they want a more granular pulse, they should be
            // using PulseRelay() instead.
            if (stateCode == Common.RELAY_STATE_PULSE_OR_REBOOT) {
                String pulseCmd = String.Format("&pulseTime{0}={1}",
                                                relayNum.ToString(),
                                                Common.DEFAULT_PULSE_TIME);
                relCmd = String.Concat(relCmd, pulseCmd);
            }

            // Finalize the command and send it. Return the new state.
            command = String.Concat(command, relCmd, "&noReply=0 HTTP/1.1\r\n\r\n");
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }
            return this.SendCommand(command);
        }

        /// <summary>
        /// Switches the specified relay on.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to switch on.
        /// </param>
        /// <returns>
        /// The current state of the device. If no response recieved from the
        /// device, then returns null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public WebRelayQuadState SwitchRelayOn(Int32 relayNum) {
            return this.ChangeRelayState(relayNum, RelayState.On);
        }

        /// <summary>
        /// Switches the specified relay off.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to switch off.
        /// </param>
        /// <returns>
        /// The current state of the device. If no response recieved from the
        /// device, then returns null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value of 1 - 4.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This object instance has been disposed - or - The IP address of the
        /// device to connect to is undefined.
        /// </exception>
        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        /// <exception cref="ConnectionFailedException">
        /// Unable to establish a connection to the device.
        /// </exception>
        /// <exception cref="IOException">
        /// Could not read/write the underlying NetworkStream.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Either the device is configured to use Basic Authentication
        /// and no control password was sent - or - invalid credentials
        /// were used.
        /// </exception>
        /// <exception cref="BadResponseFromDeviceException">
        /// The response message returned from the device is not valid. It may
        /// be worth double-checking to see that the right <see cref="IPAddress"/>
        /// was specified. If another device exists at that address and is
        /// listening on the same port, it may return some time of message
        /// that is not understood by this method.
        /// </exception>
        public WebRelayQuadState SwitchRelayOff(Int32 relayNum) {
			return this.ChangeRelayState(relayNum, RelayState.Off);
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
		/// Poll the device state.
		/// </summary>
		private void Poll() {
			Exception fail = null;
			WebRelayQuadState state = null;
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
			this._pollThread.Name = "webRelayQuadStatePoller";
			this._isPolling = true;
			this._pollThread.Start();
		}
        #endregion
    }
}
