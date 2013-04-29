using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Device status poll event failure arguments class.
    /// </summary>
    public class PollFailedEventArgs : EventArgs
    {
        #region Fields
        private Exception _failCause = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Events.PollFailedEventArgs</b>
        /// class. This is the default constructor.
        /// </summary>
        public PollFailedEventArgs()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Events.PollFailedEventArgs</b>
        /// class with the exception that is the cause of the failure.
        /// </summary>
        /// <param name="failCause">
        /// The exception that is the cause of the poll failure.
        /// </param>
        public PollFailedEventArgs(Exception failCause)
            : base() {
                this._failCause = failCause;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the exception that is the cause of the poll failure.
        /// </summary>
        public Exception FailureCause {
            get { return this._failCause; }
        }
        #endregion
    }
}
