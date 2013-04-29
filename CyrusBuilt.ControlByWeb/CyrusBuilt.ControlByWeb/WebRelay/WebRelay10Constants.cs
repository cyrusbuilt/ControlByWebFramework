using System;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Value constants specific to the WebRelay10 module.
    /// </summary>
    public static class WebRelay10Constants
    {
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

        #region Module Constants
        /// <summary>
        /// The total number of relays (10.... goes without saying really).
        /// </summary>
        public const Int32 TOTAL_RELAYS = 10;

        /// <summary>
        /// The total number of standard inputs in a WebRelay10 module.
        /// </summary>
        public const Int32 TOTAL_STANDARD_INPUTS = 2;

        /// <summary>
        /// The total number of sensor inputs in a WebRelay10 module.
        /// </summary>
        public const Int32 TOTAL_SENSOR_INPUTS = 3;

        /// <summary>
        /// The total number of external variables supported by the WebRelay10 module.
        /// </summary>
        public const Int32 TOTAL_EXT_VARS = EXT_VAR_MAX_ID + 1;
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

        /// <summary>
        /// Set external variable 0.
        /// </summary>
        public const String ACTION_SET_EXT_VAR = "set extVar0";

        /// <summary>
        /// Clear external variable 0.
        /// </summary>
        public const String ACTION_CLEAR_EXT_VAR = "clear extVar0";

        /// <summary>
        /// Change schedules.
        /// </summary>
        public const String ACTION_CHANGE_SCHED = "change schedules";
        #endregion
    }
}
