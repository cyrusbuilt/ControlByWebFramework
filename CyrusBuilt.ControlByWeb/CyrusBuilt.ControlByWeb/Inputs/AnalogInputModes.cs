using System;

namespace CyrusBuilt.ControlByWeb.Inputs
{
    /// <summary>
    /// Possible modes for an analog input.
    /// </summary>
    public enum AnalogInputModes
    {
        /// <summary>
        /// The input is being used in single-ended mode.
        /// </summary>
        SingleEnded,

        /// <summary>
        /// The input is being used in differential mode with the adjacent input.
        /// </summary>
        Differential
    }
}
