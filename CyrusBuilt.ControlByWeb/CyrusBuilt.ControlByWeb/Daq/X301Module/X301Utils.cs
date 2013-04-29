using System;

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
	/// <summary>
	/// X301 utils.
	/// </summary>
	public static class X301Utils
	{
		/// <summary>
		/// Gets an <see cref="X301Action"/> from the specified string.
		/// </summary>
		/// <param name="input">
		/// The string to get the action from.
		/// </param>
		/// <returns>
		/// The <see cref="X301Action"/> associated with the specified string;
		/// Otherwise, <see cref="X301Action.None"/>.
		/// </returns>
		public static X301Action GetActionFromString(String input) {
			if (String.IsNullOrEmpty(input)) {
				return X301Action.None;
			}
			
			input = input.ToLower().Trim();
			if (input.Contains(X301Constants.ACTION_RELAY_ON)) {
				return X301Action.TurnRelayOn;
			}
			if (input.Contains(X301Constants.ACTION_RELAY_OFF)) {
				return X301Action.TurnRelayOff;
			}
			if (input.Contains(X301Constants.ACTION_PULSE_RELAY)) {
				return X301Action.PulseRelay;
			}
			if (input.Contains(X301Constants.ACTION_TOGGLE_RELAY)) {
				return X301Action.ToggleRelay;
			}
			return X301Action.None;
		}
		
		/// <summary>
		/// Converts an action into a string.
		/// </summary>
		/// <param name="action">
		/// The action to convert.
		/// </param>
		/// <returns>
		/// The string representation of the specified action.
		/// </returns>
		public static String GetActionString(X301Action action) {
			String str = String.Empty;
			switch (action) {
				case X301Action.None:
					break;
				case X301Action.PulseRelay:
					str = X301Constants.ACTION_PULSE_RELAY;
					break;
				case X301Action.ToggleRelay:
					str = X301Constants.ACTION_TOGGLE_RELAY;
					break;
				case X301Action.TurnRelayOff:
					str = X301Constants.ACTION_RELAY_OFF;
					break;
				case X301Action.TurnRelayOn:
					str = X301Constants.ACTION_RELAY_ON;
					break;
			}
			return str;
		}

        /// <summary>
        /// Gets the value code associated with the specified action.
        /// </summary>
        /// <param name="action">
        /// The action to get the code from.
        /// </param>
        /// <returns>
        /// The value code associated with the specified action.
        /// </returns>
        public static Int32 GetActionCode(X301Action action) {
            Int32 code = X301Constants.ACTION_CODE_NONE;
            switch (action) {
                case X301Action.None:
                    break;
                case X301Action.PulseRelay:
                    code = X301Constants.ACTION_CODE_RELAY_PULSE;
                    break;
                case X301Action.ToggleRelay:
                    code = X301Constants.ACTION_CODE_RELAY_TOGGLE;
                    break;
                case X301Action.TurnRelayOff:
                    code = X301Constants.ACTION_CODE_RELAY_OFF;
                    break;
                case X301Action.TurnRelayOn:
                    code = X301Constants.ACTION_CODE_RELAY_ON;
                    break;
            }
            return code;
        }
	}
}

