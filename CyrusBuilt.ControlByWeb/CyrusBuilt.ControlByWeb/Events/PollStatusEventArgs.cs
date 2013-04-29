using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Device status poll event arguments class.
    /// </summary>
    public class PollStatusEventArgs : EventArgs
    {
        #region Fields
        private DeviceStateBase _state = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Events.PollStatusEventArgs</b>
        /// class. This is the default constructor.
        /// </summary>
        /// <param name="state">
        /// The state of the device being polled.
        /// </param>
        public PollStatusEventArgs(DeviceStateBase state)
            : base() {
                this._state = state;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the state of the device being polled.
        /// </summary>
        public DeviceStateBase State {
            get { return this._state; }
        }
        #endregion
    }
}
