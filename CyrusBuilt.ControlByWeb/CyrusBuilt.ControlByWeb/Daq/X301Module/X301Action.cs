using System;

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
	/// <summary>
	/// Defines possible actions for the X-301 module.
	/// </summary>
	public enum X301Action
	{
		/// <summary>
		/// Turn the relay on.
		/// </summary>
		TurnRelayOn,
		
		/// <summary>
		/// Turn the relay off.
		/// </summary>
		TurnRelayOff,
		
		/// <summary>
		/// Pulse the relay.
		/// </summary>
		PulseRelay,
		
		/// <summary>
		/// Toggle the relay (on, then off).
		/// </summary>
		ToggleRelay,
		
		/// <summary>
		/// No action.
		/// </summary>
		None
	}
}

