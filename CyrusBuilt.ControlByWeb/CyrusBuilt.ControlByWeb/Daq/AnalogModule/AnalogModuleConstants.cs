using System;

namespace CyrusBuilt.ControlByWeb.Daq.AnalogModule
{
    /// <summary>
    /// Value constants specific to the Analog Module.
    /// </summary>
    public static class AnalogModuleConstants
    {
        #region Module Constants
        /// <summary>
        /// The total number of inputs the module has (8).
        /// </summary>
        public const Int32 TOTAL_INPUTS = 8;
        #endregion

        #region Input Constants
        /// <summary>
        /// The minimum input ID value (0).
        /// </summary>
        public const Int32 MIN_INPUT_ID = 0;

        /// <summary>
        /// The maximum input ID value (7).
        /// </summary>
        public const Int32 MAX_INPUT_ID = (TOTAL_INPUTS - 1);

        /// <summary>
        /// The maximum readable value on any given input (5V).
        /// </summary>
        public const Double MAX_INPUT_VALUE = 5.0;

        /// <summary>
        /// The input state returned by the device when the input value is out
        /// of range (valid range is 0 to 5 volts).
        /// </summary>
        public const String ERR_OUT_OF_RANGE = "OUT_OF_RANGE";

        /// <summary>
        /// The highest resolution (in bits) that the analog-to-digital
        /// converter can convert at (24.6).
        /// </summary>
        public const Double MAX_RESOLUTION = 24.6;
        #endregion
    }
}
