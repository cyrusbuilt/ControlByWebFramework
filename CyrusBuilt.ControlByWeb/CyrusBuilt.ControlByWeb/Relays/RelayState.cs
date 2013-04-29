using System;

namespace CyrusBuilt.ControlByWeb.Relays
{
    /// <summary>
    /// Defines operational states of a relay.
    /// </summary>
    public enum RelayState
    {
        /// <summary>
        /// The relay is not energized.
        /// </summary>
        Off,

        /// <summary>
        /// The relay is energized.
        /// </summary>
        On,

        /// <summary>
        /// The relay is pulsing from on to off.
        /// </summary>
        Pulse,

        /// <summary>
        /// The relay is rebooting the device it is controlling.
        /// </summary>
        Reboot,

        /// <summary>
        /// Automatic reboot disabled.
        /// </summary>
        DisableAutoReboot,

        /// <summary>
        /// Automatic reboot enabled.
        /// </summary>
        EnableAutoReboot,

        /// <summary>
        /// Toggles the state of the relay.
        /// </summary>
        Toggle
    }
}
