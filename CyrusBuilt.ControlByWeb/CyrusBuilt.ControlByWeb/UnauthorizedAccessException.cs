using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// The exception that is thrown when a connection to a device is not authorized.
    /// </summary>
    public class UnauthorizedAccessException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.UnauthorizedAccessException</b> class.
        /// This is the default constructor.
        /// </summary>
        public UnauthorizedAccessException()
            : base("Authorization required to access the device.") {
        }
        #endregion
    }
}
