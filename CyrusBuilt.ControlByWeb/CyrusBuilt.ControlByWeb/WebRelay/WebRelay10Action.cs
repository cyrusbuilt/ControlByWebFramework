using System;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Defines possible event actions for the WebRelay-10 module.
    /// </summary>
    public enum WebRelay10Action
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
