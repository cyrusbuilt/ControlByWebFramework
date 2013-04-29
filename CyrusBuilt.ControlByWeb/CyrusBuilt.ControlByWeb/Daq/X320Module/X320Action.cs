using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320Module
{
    /// <summary>
    /// Defines possible event actions for the X-320 module.
    /// </summary>
    public enum X320Action
    {
        /// <summary>
        /// Will turn the relay(s) on.
        /// </summary>
        TurnRelayOn,

        /// <summary>
        /// Will turn the relay(s) off.
        /// </summary>
        TurnRelayOff,

        /// <summary>
        /// Will pulse the relays(s).
        /// </summary>
        PulseRelay,

        /// <summary>
        /// Will toggle the relay(s).
        /// </summary>
        ToggleRelay,

        /// <summary>
        /// Will set an external veriable.
        /// </summary>
        SetExtVar,

        /// <summary>
        /// Will clear an external variable.
        /// </summary>
        ClearExtVar,

        /// <summary>
        /// Will change schedules.
        /// </summary>
        ChangeSchedules,

        /// <summary>
        /// No action will be performed.
        /// </summary>
        None
    }
}
