using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320Module
{
    /// <summary>
    /// Contains value constants specific to the X-320 module.
    /// </summary>
    public static class X320Constants
    {
        #region Module Constants
        /// <summary>
        /// The total number of relays in the X-320 module (2).
        /// </summary>
        public const Int32 TOTAL_RELAYS = 2;

        /// <summary>
        /// Total number of I/Os (2). Note: A relay is an output.
        /// </summary>
        public const Int32 TOTAL_IO = TOTAL_RELAYS;

        /// <summary>
        /// Total number of analog inputs (4).
        /// </summary>
        public const Int32 TOTAL_ANALOG_INPUTS = 4;

        /// <summary>
        /// Total number of sensor inputs (6).
        /// </summary>
        public const Int32 TOTAL_SENSOR_INPUTS = 6;

        /// <summary>
        /// Total number of external variables (4).
        /// </summary>
        public const Int32 TOTAL_EXT_VARS = 4;
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
