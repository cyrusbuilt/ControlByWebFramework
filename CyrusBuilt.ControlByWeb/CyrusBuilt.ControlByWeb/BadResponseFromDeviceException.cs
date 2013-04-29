using System;
using System.Text;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// The exception that is thrown when a bad response is received from a device.
    /// </summary>
    public class BadResponseFromDeviceException : Exception
    {
        #region Message Constants
        private const String ERR_MSG = "An unexpected or invalid response was received from the device.";
        #endregion

        #region Fields
        private String _response = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an new instance of the <b>CyrusBuilt.ControlByWeb.BadResponseFromDeviceException</b> class.
        /// This is the default constructor.
        /// </summary>
        public BadResponseFromDeviceException()
            : base(ERR_MSG) {
        }

        /// <summary>
        /// Initializes an new instance of the <b>CyrusBuilt.ControlByWeb.BadResponseFromDeviceException</b> class
        /// with the actual response recieved from the device.
        /// </summary>
        /// <param name="response">
        /// The actual response recieved from the device.
        /// </param>
        public BadResponseFromDeviceException(String response)
            : base(ERR_MSG) {
                this._response = response;
        }

        /// <summary>
        /// Initializes an new instance of the <b>CyrusBuilt.ControlByWeb.BadResponseFromDeviceException</b> class
        /// with a byte buffer containing the actual response recieved from the device.
        /// </summary>
        /// <param name="recvBuffer">
        /// A byte buffer containing the actual response recieved from the device.
        /// </param>
        public BadResponseFromDeviceException(Byte[] recvBuffer)
            : base(ERR_MSG) {
                if ((recvBuffer != null) && (recvBuffer.Length > 0)) {
                    this._response = Encoding.ASCII.GetString(recvBuffer);
                    Array.Clear(recvBuffer, 0, recvBuffer.Length);
                }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the actual response recieved from the device.
        /// </summary>
        public String Response {
            get { return this._response; }
        }
        #endregion
    }
}
