using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Value constants specific to the X-300 module.
    /// </summary>
    public static class X300Constants
    {
        #region Module Constants
        /// <summary>
        /// The total number of inputs the X-300 has.
        /// </summary>
        public const Int32 TOTAL_INPUTS = 8;

        /// <summary>
        /// The total number of relays the X-300 has.
        /// </summary>
        public const Int32 TOTAL_RELAYS = 3;

        /// <summary>
        /// The command value to reset the the filter count. 
        /// </summary>
        public const Int32 FILTER_RST = 1;

        /// <summary>
        /// The command value to hold the current set temperature. Essentially
        /// toggles the 7 day programming on/off.
        /// </summary>
        public const Int32 HOLD_TOGGLE = 1;

        /// <summary>
        /// The default number of days before a filter change.
        /// </summary>
        public const Int32 DEFAULT_FILTER_CHANGE_DAYS = 60;

        /// <summary>
        /// The absolute bare-minimum allowable temperature that can be set.
        /// </summary>
        public const Double MIN_TEMP_ABSOLUTE = 0.0; 
        #endregion

        #region Device Mode Constants
        /// <summary>
        /// Device is in thermostat mode (6).
        /// </summary>
        public const Int32 MODE_THERMOSTAT = 6;

        /// <summary>
        /// Device is in temperature monitor mode (7).
        /// </summary>
        public const Int32 MODE_TEMPERATUREMONITOR = 7;
        #endregion

        #region Heat Mode Constants
        /// <summary>
        /// System off (0).
        /// </summary>
        public const Int32 HEAT_MODE_OFF = 0;

        /// <summary>
        /// System is heating (1).
        /// </summary>
        public const Int32 HEAT_MODE_HEATONLY = 1;

        /// <summary>
        /// System is cooling (2).
        /// </summary>
        public const Int32 HEAT_MODE_COOLONLY = 2;

        /// <summary>
        /// System will heat or cool depending on ambient temperature (3).
        /// </summary>
        public const Int32 HEAT_MODE_AUTO = 3;
        #endregion

        #region Fan Mode Constants
        /// <summary>
        /// Fan is always on (0).
        /// </summary>
        public const Int32 FAN_MODE_ON = 0;

        /// <summary>
        /// Fan works automatically (1).
        /// </summary>
        public const Int32 FAN_MODE_AUTO = 1;
        #endregion
    }
}
