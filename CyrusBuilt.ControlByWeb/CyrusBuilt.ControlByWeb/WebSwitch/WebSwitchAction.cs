using System;

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// Defines possible event actions for the WebSwitch module.
    /// </summary>
    public enum WebSwitchAction
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
        /// Disables scheduled events. (Only used by WebSwitch Plus)
        /// </summary>
        DisableEvents,

        /// <summary>
        /// Enables scheduled events. (Only used by WebSwitch Plus)
        /// </summary>
        EnableEvents,

        /// <summary>
        /// No action will be performed.
        /// </summary>
        None
    }
}
