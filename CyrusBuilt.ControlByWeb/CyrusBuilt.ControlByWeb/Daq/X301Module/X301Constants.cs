using System;

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
	/// <summary>
	/// X301 constants.
	/// </summary>
	public static class X301Constants
	{
		#region Module Constants
		/// <summary>
		/// The total number of inputs in the X-301 module (2).
		/// </summary>
		public const Int32 TOTAL_INPUTS = 2;
		
        /// <summary>
        /// The total number of relays in the X-301 module (2).
        /// </summary>
        public const Int32 TOTAL_RELAYS = TOTAL_INPUTS;
		#endregion
		
		#region Action Constants
		/// <summary>
		/// Turn the relay on.
		/// </summary>
		public const String ACTION_RELAY_ON = "on";
		
		/// <summary>
		/// Turn the relay off.
		/// </summary>
		public const String ACTION_RELAY_OFF = "off";
		
		/// <summary>
		/// Pulse the relay.
		/// </summary>
		public const String ACTION_PULSE_RELAY = "pulse";
		
		/// <summary>
		/// Toggle the relay (on, then off).
		/// </summary>
		public const String ACTION_TOGGLE_RELAY = "toggle";

        /// <summary>
        /// No action (0).
        /// </summary>
        public const Int32 ACTION_CODE_NONE = 0;

        /// <summary>
        /// Turn relay(s) on (1).
        /// </summary>
        public const Int32 ACTION_CODE_RELAY_ON = 1;

        /// <summary>
        /// Turn relay(s) off (2).
        /// </summary>
        public const Int32 ACTION_CODE_RELAY_OFF = 2;

        /// <summary>
        /// Pulse relay (3).
        /// </summary>
        public const Int32 ACTION_CODE_RELAY_PULSE = 3;

        /// <summary>
        /// Toggle relay (4).
        /// </summary>
        public const Int32 ACTION_CODE_RELAY_TOGGLE = 4;

        /// <summary>
        /// Disable events (5).
        /// </summary>
        public const Int32 ACTION_CODE_DISABLE_EVENTS = 5;

        /// <summary>
        /// Enable events (6).
        /// </summary>
        public const Int32 ACTION_CODE_ENABLE_EVENTS = 6;

        /// <summary>
        /// Set external variable zero (7).
        /// </summary>
        public const Int32 ACTION_CODE_SET_EXT_VAR0 = 7;

        /// <summary>
        /// Clear external variable zero (8).
        /// </summary>
        public const Int32 ACTION_CODE_CLEAR_EXT_VAR0 = 8;
		#endregion
		
		#region External Variable Constants
		/// <summary>
		/// The minimum external variable ID.
		/// </summary>
		public const Int32 EXT_VAR_MIN_ID = 0;
		
		/// <summary>
		/// The maximum external variable ID.
		/// </summary>
		public const Int32 EXT_VAR_MAX_ID = 4;
		#endregion
	}
}

