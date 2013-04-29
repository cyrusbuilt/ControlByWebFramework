using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
	/// <summary>
	/// X320 M action.
	/// </summary>
	public enum X320MAction
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
		/// No action.
		/// </summary>
		None
	}
}

