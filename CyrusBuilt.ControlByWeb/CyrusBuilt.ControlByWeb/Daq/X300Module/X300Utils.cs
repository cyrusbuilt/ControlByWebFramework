using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Utility methods specific to the X-300 module.
    /// </summary>
    public static class X300Utils
    {
        /// <summary>
        /// Gets the <see cref="X300OperationMode"/> from the specified code.
        /// </summary>
        /// <param name="mode">
        /// Must be either <see cref="X300Constants.MODE_THERMOSTAT"/> or
        /// <see cref="X300Constants.MODE_TEMPERATUREMONITOR"/>.
        /// </param>
        /// <returns>
        /// The <see cref="X300OperationMode"/> associated with the specified value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// mode is invalid.
        /// </exception>
        public static X300OperationMode GetOperationMode(Int32 mode) {
            if ((mode != X300Constants.MODE_THERMOSTAT) &&
                (mode != X300Constants.MODE_TEMPERATUREMONITOR)) {
                String err = "mode must be either X300Constants.MODE_THERMOSTAT"
                                + " or X300Constants.MODE_TEMPERATUREMONITOR.";
                throw new ArgumentException(err, "mode");
            }

            if (mode == X300Constants.MODE_THERMOSTAT) {
                return X300OperationMode.Thermostat;
            }
            return X300OperationMode.TemperatureMonitor;;
        }

        /// <summary>
        /// Gets the integer value code associated with the specified mode.
        /// </summary>
        /// <param name="mode">
        /// The mode to get the value of.
        /// </param>
        /// <returns>
        /// The code associated with the specified mode.
        /// </returns>
        public static Int32 GetOperationModeCode(X300OperationMode mode) {
            if (mode == X300OperationMode.Thermostat) {
                return X300Constants.MODE_THERMOSTAT;
            }
            return X300Constants.MODE_TEMPERATUREMONITOR;;
        }

        /// <summary>
        /// Gets the appropriate <see cref="HeatMode"/> associated with the
        /// specified code.
        /// </summary>
        /// <param name="mode">
        /// The value of the mode to get. See the heat mode constants in
        /// <see cref="X300Constants"/> for possible values.
        /// </param>
        /// <returns>
        /// The appropriate heat mode.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// mode must be an integer value not less than zero and not greater
        /// than three.
        /// </exception>
        public static HeatMode GetHeatMode(Int32 mode) {
            if ((mode < X300Constants.HEAT_MODE_OFF) ||
                (mode > X300Constants.HEAT_MODE_AUTO)) {
                    String err = String.Format("Must be a value {0} - {1}.",
                                                X300Constants.HEAT_MODE_OFF.ToString(),
                                                X300Constants.HEAT_MODE_AUTO.ToString());
                    throw new ArgumentOutOfRangeException("mode", err);
            }
        
            HeatMode heatMode = HeatMode.Off;
            switch (mode) {
                case X300Constants.HEAT_MODE_OFF:
                    heatMode = HeatMode.Off;
                    break;
                case X300Constants.HEAT_MODE_HEATONLY:
                    heatMode = HeatMode.HeatOnly;
                    break;
                case X300Constants.HEAT_MODE_COOLONLY:
                    heatMode = HeatMode.CoolOnly;
                    break;
                case X300Constants.HEAT_MODE_AUTO:
                    heatMode = HeatMode.Auto;
                    break;
            }
            return heatMode;
        }

        /// <summary>
        /// Gets the integer value associated with the specified heat mode.
        /// </summary>
        /// <param name="mode">
        /// The mode to get the value of.
        /// </param>
        /// <returns>
        /// The value associated with the specified mode. See the heat mode
        /// constants in <see cref="X300Constants"/> for possible return values.
        /// </returns>
        public static Int32 GetHeadModeCode(HeatMode mode) {
            Int32 retCode = X300Constants.HEAT_MODE_OFF;
            switch (mode) {
                case HeatMode.Off:
                    retCode = X300Constants.HEAT_MODE_OFF;
                    break;
                case HeatMode.HeatOnly:
                    retCode = X300Constants.HEAT_MODE_HEATONLY;
                    break;
                case HeatMode.CoolOnly:
                    retCode = X300Constants.HEAT_MODE_COOLONLY;
                    break;
                case HeatMode.Auto:
                    retCode = X300Constants.HEAT_MODE_AUTO;
                    break;
            }
            return retCode;
        }

        /// <summary>
        /// Gets the appropriate <see cref="FanMode"/> associated with the
        /// specified code.
        /// </summary>
        /// <param name="mode">
        /// An integer value associated with the requested mode.
        /// </param>
        /// <returns>
        /// The fan mode associated with the specified code.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// mode must be either <see cref="X300Constants.FAN_MODE_ON"/>
        /// or <see cref="X300Constants.FAN_MODE_AUTO"/>.
        /// </exception>
        public static FanMode GetFanMode(Int32 mode) {
            if ((mode < X300Constants.FAN_MODE_ON) ||
                (mode > X300Constants.FAN_MODE_AUTO)) {
                    String err = "Must be either X300Constants.FAN_MODE_ON or"
                                    + " X300Constants.FAN_MODE_AUTO.";
                    throw new ArgumentOutOfRangeException("mode", err);
            }

            if (mode == X300Constants.FAN_MODE_AUTO) {
                return FanMode.Auto;
            }
            return FanMode.On;
        }

        /// <summary>
        /// Gets the integer value associated with the specified
        /// <see cref="FanMode"/>.
        /// </summary>
        /// <param name="mode">
        /// The mode to get the code of.
        /// </param>
        /// <returns>
        /// The code associated with the specified mode.
        /// </returns>
        public static Int32 GetFanModeCode(FanMode mode) {
            if (mode == FanMode.Auto) {
                return X300Constants.FAN_MODE_AUTO;
            }
            return X300Constants.FAN_MODE_ON;
        }
    }
}
