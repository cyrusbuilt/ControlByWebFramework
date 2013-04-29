using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Available heat modes (temperature regulation modes).
    /// </summary>
    public enum HeatMode
    {
        /// <summary>
        /// System is off.
        /// </summary>
        Off,

        /// <summary>
        /// System is heating only.
        /// </summary>
        HeatOnly,

        /// <summary>
        /// System is cooling only.
        /// </summary>
        CoolOnly,

        /// <summary>
        /// System will heat or cool automatically as needed.
        /// </summary>
        Auto
    }
}
