using System;

namespace CyrusBuilt.ControlByWeb.Inputs
{
    /// <summary>
    /// Defines possible states of an input.
    /// </summary>
    public enum InputState
    {
        /// <summary>
        /// Voltage is not being applied to the input.
        /// </summary>
        Off,

        /// <summary>
        /// Voltage has been applied to the input.
        /// </summary>
        On
    }
}
