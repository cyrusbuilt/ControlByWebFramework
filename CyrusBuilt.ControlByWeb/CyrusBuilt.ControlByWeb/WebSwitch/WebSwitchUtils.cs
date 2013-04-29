using System;

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// WebSwitch module utiliy methods.
    /// </summary>
    public static class WebSwitchUtils
    {
        /// <summary>
        /// Gets a <see cref="WebSwitchAction"/> from the specified string.
        /// </summary>
        /// <param name="action">
        /// The string to get the action from.
        /// </param>
        /// <returns>
        /// The <see cref="WebSwitchAction"/> associated with the specified string;
        /// Otherwise, <see cref="WebSwitchAction.None"/>.
        /// </returns>
        public static WebSwitchAction GetActionFromString(String action) {
            if (String.IsNullOrEmpty(action)) {
                return WebSwitchAction.None;
            }

            action = action.ToLower().Trim();
            WebSwitchAction act = WebSwitchAction.None;
            switch (action) {
                case WebSwitchConstants.ACTION_PULSE_RELAY:
                    act = WebSwitchAction.PulseRelay;
                    break;
                case WebSwitchConstants.ACTION_RELAY_OFF:
                    act = WebSwitchAction.TurnRelayOff;
                    break;
                case WebSwitchConstants.ACTION_RELAY_ON:
                    act = WebSwitchAction.TurnRelayOn;
                    break;
                case WebSwitchConstants.ACTION_TOGGLE_RELAY:
                    act = WebSwitchAction.ToggleRelay;
                    break;
            }
            return act;
        }
    }
}
