using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320Module
{
    /// <summary>
    /// Utility methods specific to the X-320 module.
    /// </summary>
    public static class X320Utils
    {
        /// <summary>
        /// Gets an <see cref="X320Action"/> from the specified string.
        /// </summary>
        /// <param name="action">
        /// The string to get the action from.
        /// </param>
        /// <returns>
        /// The <see cref="X320Action"/> associated with the specified string;
        /// Otherwise, <see cref="X320Action.None"/>.
        /// </returns>
        public static X320Action GetActionFromString(String action) {
            if (String.IsNullOrEmpty(action)) {
                return X320Action.None;
            }

            action = action.ToLower().Trim();
            X320Action act = X320Action.None;
            switch (action) {
                case X320Constants.ACTION_CHANGE_SCHED:
                    act = X320Action.ChangeSchedules;
                    break;
                case X320Constants.ACTION_CLEAR_EXT_VAR:
                    act = X320Action.ClearExtVar;
                    break;
                case X320Constants.ACTION_PULSE_RELAY:
                    act = X320Action.PulseRelay;
                    break;
                case X320Constants.ACTION_RELAY_OFF:
                    act = X320Action.TurnRelayOff;
                    break;
                case X320Constants.ACTION_RELAY_ON:
                    act = X320Action.TurnRelayOn;
                    break;
                case X320Constants.ACTION_SET_EXT_VAR:
                    act = X320Action.SetExtVar;
                    break;
                case X320Constants.ACTION_TOGGLE_RELAY:
                    act = X320Action.ToggleRelay;
                    break;
            }
            return act;
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
        public static String GetActionString(X320Action action) {
            String str = String.Empty;
            switch (action) {
                case X320Action.ChangeSchedules:
                    str = X320Constants.ACTION_CHANGE_SCHED;
                    break;
                case X320Action.ClearExtVar:
                    str = X320Constants.ACTION_CLEAR_EXT_VAR;
                    break;
                case X320Action.None:
                    break;
                case X320Action.PulseRelay:
                    str = X320Constants.ACTION_PULSE_RELAY;
                    break;
                case X320Action.SetExtVar:
                    str = X320Constants.ACTION_RELAY_OFF;
                    break;
                case X320Action.ToggleRelay:
                    str = X320Constants.ACTION_TOGGLE_RELAY;
                    break;
                case X320Action.TurnRelayOff:
                    str = X320Constants.ACTION_RELAY_OFF;
                    break;
                case X320Action.TurnRelayOn:
                    str = X320Constants.ACTION_RELAY_ON;
                    break;
            }
            return str;
        }
    }
}
