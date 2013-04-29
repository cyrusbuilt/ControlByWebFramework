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
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Security;

namespace CyrusBuilt.ControlByWeb.Daq.AnalogModule
{
    /// <summary>
    /// Used to monitor and control a ControlByWeb Analog Module.
    /// </summary>
    public class AnalogModuleController : IDisposable
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
		/// Occurs polling the device state fails.
		/// </summary>
		public event PollFailEventHandler PollFailed;
		#endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AnalogModuleController</b>
        /// class. This is the default constructor.
        /// </summary>
        public AnalogModuleController() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AnalogModuleController</b>
        /// class with the IP address of the module to control.
        /// </summary>
        /// <param name="ipAddr">
        /// The IP address of the module to control.
        /// </param>
        public AnalogModuleController(IPAddress ipAddr) {
            this._ip = ipAddr;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AnalogModuleController</b>
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
        public AnalogModuleController(IPAddress ipAddr, Int32 port) {
            if ((port < 1) || (port > 65535)) {
                throw new ArgumentOutOfRangeException("port", "Port must be a value 1 - 65535.");
            }
            this._ip = ipAddr;
            this._port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AnalogModuleController</b>
        /// class with the IP endpoint to connect to.
        /// </summary>
        /// <param name="endpoint">
        /// The IP endpoint to connect to.
        /// </param>
        public AnalogModuleController(IPEndPoint endpoint) {
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
        /// Parses device state information from both specified XML nodes.
        /// </summary>
        /// <param name="devState">
        /// The XML node containing device state information.
        /// </param>
        /// <param name="adcState">
        /// The XML node containing input mode information.
        /// </param>
        /// <returns>
        /// An object containing device state information parsed from both nodes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="devState"/> and <paramref name="adcState"/> cannot
        /// be null.
        /// </exception>
        private AnalogModuleState GetStateFromXmlNodes(XmlNode devState, XmlNode adcState) {
            if (devState == null) {
                throw new ArgumentNullException("devState");
            }

            if (adcState == null) {
                throw new ArgumentNullException("adcState");
            }

            //devState = /state.xml (head node = "datavalues")
            //adcState = /adcstate.xml (head node = "datavalues")

            AnalogModuleState state = new AnalogModuleState();
            AMInput inp = null;
            String itemText = String.Empty;
            String attrib = String.Empty;
            Double value = 0.0;

            // Get the inputs.
            for (Int32 i = AnalogModuleConstants.MIN_INPUT_ID; i <= AnalogModuleConstants.MAX_INPUT_VALUE; i++) {
                // Get the input and state.
                inp = null;
                attrib = String.Format("input{0}state", i.ToString());
                itemText = Common.GetNamedChildNode(devState, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out value)) {
                        inp = new AMInput(value);
                    }
                }

                // We got an input.
                if (inp != null) {
                    // Get the input mode.
                    attrib = String.Format("an{0}_mode", i.ToString());
                    itemText = Common.GetNamedChildNode(adcState, attrib).InnerText;
                    if (!String.IsNullOrEmpty(itemText)) {
                        if (itemText == "differential") {
                            inp = new AMInput(inp.Value, AnalogInputModes.Differential);
                        }
                    }
                    state.SetOrOverrideInput(i, inp);
                }
            }

            // Get the power-up flag.
            Int32 flag = Common.GetPowerUpFlagCode(PowerUpFlag.Off);
            itemText = Common.GetNamedChildNode(devState, "powerupflag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out flag)) {
                    state.SetOrOverridePowerUpState(Common.GetPowerUpFlag(flag));
                }
            }

            // Get the resolution.
            value = 0.0;
            itemText = Common.GetNamedChildNode(adcState, "resolution").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out value)) {
                    state.Resolution = value;
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

            // Dump the send and recieve buffers, then return the XML response.
            Array.Clear(sendBytes, 0, sendBytes.Length);
            Array.Clear(recvBytes, 0, recvBytes.Length);
            return doc;
        }
        #endregion

        #region Methods
		/// <summary>
		/// Raises the polled event.
		/// </summary>
		/// <param name='e'>
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
		/// <param name='e'>
		/// The event arguments.
		/// </param>
		protected virtual void OnPollFailed(PollFailedEventArgs e) {
			if (this.PollFailed != null) {
				this.PollFailed(this, e);
			}
		}
		
        /// <summary>
        /// Gets the state of the device.
        /// </summary>
        /// <returns>
        /// An object containing information about the state of the device.
        /// </returns>
        /// <remarks>
        /// This method makes two requests: the first request is to /state.xml
        /// (the XML page containing the device state), and the other is to
        /// /adcstate.xml (the XML page containing input operation modes) and
        /// then parses the data contained in both responses.
        /// </remarks>
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
        public AnalogModuleState GetState() {
            // Request /state.xml.
            String command = "GET /state.xml&noReply=0 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }

            XmlNode devState = null;
            XmlDocument result = this.SendCommand(command);
            if (result != null) {
                try {
                    // Got the response, now get the parent node.
                    devState = result.SelectSingleNode("/datavalues");
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }

            // Request /adcstate.xml.
            command = "GET /adcstate.xml&noReply=0 HTTP/1.1\r\n\r\n";
            if (this._authEnabled) {
                command = Common.AppendAuthToCommand(command, this._password);
            }

            XmlNode adcState = null;
            result = this.SendCommand(command);
            if (result != null) {
                try {
                    // Got the response, now get the parent node.
                    adcState = result.SelectSingleNode("/datavalues");
                }
                catch (ArgumentNullException) {
                    throw new BadResponseFromDeviceException();
                }
            }

            // We got both (valid) nodes.  Now parse the state and return.
            if ((devState != null) && (adcState != null)) {
                return this.GetStateFromXmlNodes(devState, adcState);
            }
            return null;
        }

        /// <summary>
        /// Sets the resolution of the analog-to-digital converter (ADC).
        /// </summary>
        /// <param name="resolution">
        /// The resolution (in bits) to set. Must not be greater than
        /// <see cref="AnalogModuleConstants.MAX_RESOLUTION"/>. The default
        /// is <see cref="AnalogModuleConstants.MAX_RESOLUTION"/>.
        /// </param>
        /// <remarks>
        /// The higher the resolution, the slower the conversion will be.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="resolution"/> is greater than 
        /// <see cref="AnalogModuleConstants.MAX_RESOLUTION"/>.
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
        public void SetResolution(Double resolution) {
			if (resolution > AnalogModuleConstants.MAX_RESOLUTION) {
				String err = String.Format("Resolution cannot be greater than {0}.",
                                            AnalogModuleConstants.MAX_RESOLUTION.ToString());
				throw new ArgumentException(err, "resolution");
			}

			String command = String.Format("GET /adcstate.xml?res={0} HTTP/1.1\r\n\r\n", resolution.ToString());
			if (this._authEnabled) {
				command = Common.AppendAuthToCommand(command, this._password);
			}
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
		/// Polls the state of the device.
		/// </summary>
		private void Poll() {
			Exception fail = null;
			AnalogModuleState state = null;
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
			this._pollThread.Name = "analogModuleStatePoller";
			this._isPolling = true;
			this._pollThread.Start();
		}
        #endregion
    }
}
