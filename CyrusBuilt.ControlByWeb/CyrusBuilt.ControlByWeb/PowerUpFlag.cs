using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Component power loss indicator flags.
    /// </summary>
    public enum PowerUpFlag
    {
        /// <summary>
        /// Indicates normal operation for a given power up indicator state.
        /// </summary>
        Off,

        /// <summary>
        /// Indicates power loss for a give power up indicator state.
        /// </summary>
        On
    }
}
