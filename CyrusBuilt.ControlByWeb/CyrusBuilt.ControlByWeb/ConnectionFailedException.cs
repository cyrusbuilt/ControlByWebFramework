using System;
using System.Net;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// The exception that occurs when connecting to Five Input Module device fails.
    /// </summary>
    public class ConnectionFailedException : Exception
    {
        #region Fields
        private IPAddress _address = null;
        private Int32 _port = Common.DEFAULT_PORT;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.ConnectionFailedException</b> class.
        /// This is the default constructor.
        /// </summary>
        public ConnectionFailedException()
            : base("Failed to connect to remote endpoint.") {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.ConnectionFailedException</b> class
        /// with the remote endpoint to which the connection failed.
        /// </summary>
        /// <param name="endPoint">
        /// The remote endpoint to which the connection failed.
        /// </param>
        public ConnectionFailedException(IPEndPoint endPoint)
            : base("Failed to connect to remote endpoint.") {
                if (endPoint != null) {
                    this._address = endPoint.Address;
                    this._port = endPoint.Port;
                }
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.ConnectionFailedException</b> class
        /// with the IP address and port to which the connection failed.
        /// </summary>
        /// <param name="address">
        /// The IPv4 address of the device to which the connection failed.
        /// </param>
        /// <param name="port">
        /// The port number that the connection failed on. Default is port 80.
        /// </param>
        public ConnectionFailedException(IPAddress address, Int32 port)
            : base("Failed to connect to remote endpoint.") {
                this._address = address;
                this._port = port;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the IPv4 address of the device to which the connection failed.
        /// </summary>
        public IPAddress Address {
            get { return this._address; }
        }

        /// <summary>
        /// Gets the port that the connection was attempted on.
        /// </summary>
        public Int32 Port {
            get { return this._port; }
        }

        /// <summary>
        /// Gets the message that describes the current exception.
        /// </summary>
        public override string Message {
            get {
                if (this._address != null) {
                    return String.Format("Unable to establish connection to {0}:{1}.",
                                            this._address.ToString(),
                                            this._port.ToString());
                }
                return base.Message;
            }
        }
        #endregion
    }
}
