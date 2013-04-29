using System;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Utility methods specific to the WebRelay-10 module.
    /// </summary>
    public static class WebRelay10Utils
    {
        #region Methods
        /// <summary>
        /// Gets an <see cref="WebRelay10Action"/> from the specified string.
        /// </summary>
        /// <param name="action">
        /// The string to get the action from.
        /// </param>
        /// <returns>
        /// The <see cref="WebRelay10Action"/> associated with the specified string;
        /// Otherwise, <see cref="WebRelay10Action.None"/>.
        /// </returns>
        public static WebRelay10Action GetActionFromString(String action) {
            if (String.IsNullOrEmpty(action)) {
                return WebRelay10Action.None;
            }

            action = action.Trim().ToLower();
            WebRelay10Action act = WebRelay10Action.None;
            switch (action) {
                case WebRelay10Constants.ACTION_CHANGE_SCHED:
                    act = WebRelay10Action.ChangeSchedules;
                    break;
                case WebRelay10Constants.ACTION_CLEAR_EXT_VAR:
                    act = WebRelay10Action.ClearExtVar;
                    break;
                case WebRelay10Constants.ACTION_PULSE_RELAY:
                    act = WebRelay10Action.PulseRelay;
                    break;
                case WebRelay10Constants.ACTION_RELAY_OFF:
                    act = WebRelay10Action.TurnRelayOff;
                    break;
                case WebRelay10Constants.ACTION_RELAY_ON:
                    act = WebRelay10Action.TurnRelayOn;
                    break;
                case WebRelay10Constants.ACTION_SET_EXT_VAR:
                    act = WebRelay10Action.SetExtVar;
                    break;
                case WebRelay10Constants.ACTION_TOGGLE_RELAY:
                    act = WebRelay10Action.ToggleRelay;
                    break;
            }
            return act;
        }

        /// <summary>
        /// Converts the specified action into its string equivalent.
        /// </summary>
        /// <param name="action">
        /// The action to convert.
        /// </param>
        /// <returns>
        /// The string representation of the specified action.
        /// </returns>
        public static String GetActionString(WebRelay10Action action) {
            String actionStr = String.Empty;
            switch (action) {
                case WebRelay10Action.ChangeSchedules:
                    actionStr = WebRelay10Constants.ACTION_CHANGE_SCHED;
                    break;
                case WebRelay10Action.ClearExtVar:
                    actionStr = WebRelay10Constants.ACTION_CLEAR_EXT_VAR;
                    break;
                case WebRelay10Action.None:
                    break;
                case WebRelay10Action.PulseRelay:
                    actionStr = WebRelay10Constants.ACTION_PULSE_RELAY;
                    break;
                case WebRelay10Action.SetExtVar:
                    actionStr = WebRelay10Constants.ACTION_SET_EXT_VAR;
                    break;
                case WebRelay10Action.ToggleRelay:
                    actionStr = WebRelay10Constants.ACTION_TOGGLE_RELAY;
                    break;
                case WebRelay10Action.TurnRelayOff:
                    actionStr = WebRelay10Constants.ACTION_RELAY_OFF;
                    break;
                case WebRelay10Action.TurnRelayOn:
                    actionStr = WebRelay10Constants.ACTION_RELAY_ON;
                    break;
            }
            return actionStr;
        }
        #endregion
    }
}
