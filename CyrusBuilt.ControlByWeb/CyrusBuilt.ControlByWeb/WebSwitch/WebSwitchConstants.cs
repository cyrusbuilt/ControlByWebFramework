using System;

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// Value constants specific to the WebSwitch device.
    /// </summary>
    public static class WebSwitchConstants
    {
        #region Module Constants
        /// <summary>
        /// The total number of inputs the WebSwitch Plus has (2).
        /// </summary>
        public const Int32 TOTAL_INPUTS = 2;

        /// <summary>
        /// The total number of relays the WebSwitch and WebSwitch Plus has (2).
        /// </summary>
        public const Int32 TOTAL_RELAYS = TOTAL_INPUTS;

        /// <summary>
        /// The total number of sensors the WebSwitch Plus has (3).
        /// </summary>
        public const Int32 TOTAL_SENSORS = 3;

        /// <summary>
        /// The total number of external variables supported by the WebSwitch (5).
        /// </summary>
        public const Int32 TOTAL_EXT_VARS = 5;
        #endregion

        #region External Variable Constants
        /// <summary>
        /// The minimum external variable ID (0).
        /// </summary>
        public const Int32 MAX_EXT_VAR_ID = 0;

        /// <summary>
        /// The maximum external variable ID (4).
        /// </summary>
        public const Int32 MIN_EXT_VAR_ID = (TOTAL_EXT_VARS - 1);
        #endregion

        #region Action Constants
        /// <summary>
        /// Turn the relay(s) on.
        /// </summary>
        public const String ACTION_RELAY_ON = "turn relay(s) on";

        /// <summary>
        /// Turn the relay(s) off.
        /// </summary>
        public const String ACTION_RELAY_OFF = "turn relay(s) off";

        /// <summary>
        /// Pulse the relay(s).
        /// </summary>
        public const String ACTION_PULSE_RELAY = "pulse relay(s)";

        /// <summary>
        /// Toggle the relay(s).
        /// </summary>
        public const String ACTION_TOGGLE_RELAY = "toggle relay(s)";
        #endregion

        #region Action Codes
        /// <summary>
        /// No action will be taken.
        /// </summary>
        public const Int32 ACTIONCODE_NO_ACTION = 0;

        /// <summary>
        /// The relay(s) will be switched ON.
        /// </summary>
        public const Int32 ACTIONCODE_ACTIVATE_RELAY = 1;

        /// <summary>
        /// The relay(s) will be switched OFF.
        /// </summary>
        public const Int32 ACTIONCODE_DEACTIVATE_RELAY = 2;

        /// <summary>
        /// The relay(s) will be pulsed.
        /// </summary>
        public const Int32 ACTIONCODE_PULSE_RELAY = 3;

        /// <summary>
        /// The relay(s) will be toggled ON, then OFF.
        /// </summary>
        public const Int32 ACTIONCODE_TOGGLE_RELAY = 4;

        /// <summary>
        /// Scheduled events will be disabled. (Only used by WebSwitch Plus)
        /// </summary>
        public const Int32 ACTIONCODE_DISABLE_EVENTS = 5;

        /// <summary>
        /// Scheduled events will be enabled. (Only used by WebSwitch Plus)
        /// </summary>
        public const Int32 ACTIONCODE_ENABLE_EVENTS = 6;

        /// <summary>
        /// External variable 0 will be set with a value.
        /// </summary>
        public const Int32 ACTIONCODE_SET_EXT_VAR0 = 7;

        /// <summary>
        /// The value of external variable 0 will be cleared.
        /// </summary>
        public const Int32 ACTIONCODE_CLEAR_EXT_VAR0 = 8;
        #endregion
    }
}
