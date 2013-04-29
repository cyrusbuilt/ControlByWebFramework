using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
	/// <summary>
	/// Utility methods specific to the X-320M module.
	/// </summary>
	public static class X320MUtils
	{
		/// <summary>
        /// Gets an <see cref="X320MAction"/> from the specified string.
        /// </summary>
        /// <param name="action">
        /// The string to get the action from.
        /// </param>
        /// <returns>
        /// The <see cref="X320MAction"/> associated with the specified string;
        /// Otherwise, <see cref="X320MAction.None"/>.
        /// </returns>
        public static X320MAction GetActionFromString(String action) {
            if (String.IsNullOrEmpty(action)) {
                return X320MAction.None;
            }

            action = action.ToLower().Trim();
            X320MAction act = X320MAction.None;
            switch (action) {
                case X320MConstants.ACTION_PULSE_RELAY:
                    act = X320MAction.PulseRelay;
                    break;
                case X320MConstants.ACTION_RELAY_OFF:
                    act = X320MAction.TurnRelayOff;
                    break;
                case X320MConstants.ACTION_RELAY_ON:
                    act = X320MAction.TurnRelayOn;
                    break;
                case X320MConstants.ACTION_TOGGLE_RELAY:
                    act = X320MAction.ToggleRelay;
                    break;
				default:
					act = X320MAction.None;
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
        public static String GetActionString(X320MAction action) {
            String str = String.Empty;
            switch (action) {
                case X320MAction.None:
                    break;
                case X320MAction.PulseRelay:
                    str = X320MConstants.ACTION_PULSE_RELAY;
                    break;
                case X320MAction.ToggleRelay:
                    str = X320MConstants.ACTION_TOGGLE_RELAY;
                    break;
                case X320MAction.TurnRelayOff:
                    str = X320MConstants.ACTION_RELAY_OFF;
                    break;
                case X320MAction.TurnRelayOn:
                    str = X320MConstants.ACTION_RELAY_ON;
                    break;
            }
            return str;
        }

        /// <summary>
        /// Gets the type of humidity history from the specified string.
        /// </summary>
        /// <param name="input">
        /// A string containing the name of a humidity history type. (see
        /// HUMIDITY_* constants in <see cref="X320MConstants"/> class).
        /// </param>
        /// <returns>
        /// The corresponding history type or <see cref="HumidityReadings.None"/>
        /// if the specified string does not contain a valid humidity type name.
        /// </returns>
        public static HumidityReadings GetHumidityHistoryType(String input) {
            if (String.IsNullOrEmpty(input)) {
                return HumidityReadings.None;
            }

            input = input.Trim();
            HumidityReadings reading = HumidityReadings.None;
            switch (input) {
                case X320MConstants.HUMIDITY_HIGH_TODAY:
                    reading = HumidityReadings.HighToday;
                    break;
                case X320MConstants.HUMIDITY_HIGH_YESTERDAY:
                    reading = HumidityReadings.HighYesterday;
                    break;
                case X320MConstants.HUMIDITY_LOW_TODAY:
                    reading = HumidityReadings.LowToday;
                    break;
                case X320MConstants.HUMIDITY_LOW_YESTERDAY:
                    reading = HumidityReadings.LowYesterday;
                    break;
            }
            return reading;
        }

        /// <summary>
        /// Gets the state element name of the specified humidity reading type.
        /// </summary>
        /// <param name="reading">
        /// The humidity reading type to get the name of.
        /// </param>
        /// <returns>
        /// A string containing the type name.
        /// </returns>
        public static String GetHumidityHistoryName(HumidityReadings reading) {
            String name = String.Empty;
            switch (reading) {
                case HumidityReadings.HighToday:
                    name = X320MConstants.HUMIDITY_HIGH_TODAY;
                    break;
                case HumidityReadings.HighYesterday:
                    name = X320MConstants.HUMIDITY_HIGH_YESTERDAY;
                    break;
                case HumidityReadings.LowToday:
                    name = X320MConstants.HUMIDITY_LOW_TODAY;
                    break;
                case HumidityReadings.LowYesterday:
                    name = X320MConstants.HUMIDITY_LOW_YESTERDAY;
                    break;
                case HumidityReadings.None:
                default:
                    break;
            }
            return name;
        }

        /// <summary>
        /// Get the rainfall reading type by name from the specified string.
        /// </summary>
        /// <param name="input">
        /// A string containing the state element name corresponding to the
        /// desired reading type.
        /// </param>
        /// <returns>
        /// The requested reading type or <see cref="RainfallReadings.None"/>
        /// if the input is invalid.
        /// </returns>
        public static RainfallReadings GetRainfallHistoryType(String input) {
            if (String.IsNullOrEmpty(input)) {
                return RainfallReadings.None;
            }

            input = input.Trim();
            RainfallReadings reading = RainfallReadings.None;
            switch (input) {
                case X320MConstants.RAIN_LAST_HOUR:
                    reading = RainfallReadings.LastHour;
                    break;
                case X320MConstants.RAIN_TOTAL_TODAY:
                    reading = RainfallReadings.TotalToday;
                    break;
                case X320MConstants.RAIN_TOTAL_LAST_SEVEN_DAYS:
                    reading = RainfallReadings.TotalSevenDays;
                    break;
            }
            return reading;
        }

        /// <summary>
        /// Gets the state element name of the specified rainfall reading type.
        /// </summary>
        /// <param name="reading">
        /// The reading type to get the name of.
        /// </param>
        /// <returns>
        /// The element name of the specified reading type.
        /// </returns>
        public static String GetRainfallHistoryName(RainfallReadings reading) {
            String name = String.Empty;
            switch (reading) {
                case RainfallReadings.LastHour:
                    name = X320MConstants.RAIN_LAST_HOUR;
                    break;
                case RainfallReadings.TotalSevenDays:
                    name = X320MConstants.RAIN_TOTAL_LAST_SEVEN_DAYS;
                    break;
                case RainfallReadings.TotalToday:
                    name = X320MConstants.RAIN_TOTAL_TODAY;
                    break;
                case RainfallReadings.None:
                default:
                    break;
            }
            return name;
        }

        /// <summary>
        /// Gets the state element name of the specified temperature reading.
        /// </summary>
        /// <param name="reading">
        /// The reading to get the element name of.
        /// </param>
        /// <returns>
        /// The element name of the specified reading type.
        /// </returns>
        public static String GetTemperatureHistoryName(TemperatureReadings reading) {
            String name = String.Empty;
            switch (reading) {
                case TemperatureReadings.DewPoint:
                    name = X320MConstants.TEMP_DEW_POINT;
                    break;
                case TemperatureReadings.HeatIndex:
                    name = X320MConstants.TEMP_HEAT_INDEX;
                    break;
                case TemperatureReadings.HighTempToday:
                    name = X320MConstants.TEMP_HIGH_TODAY;
                    break;
                case TemperatureReadings.HighTempYesterday:
                    name = X320MConstants.TEMP_HIGH_YESTERDAY;
                    break;
                case TemperatureReadings.LowTempToday:
                    name = X320MConstants.TEMP_LOW_TODAY;
                    break;
                case TemperatureReadings.LowTempYesterday:
                    name = X320MConstants.TEMP_LOW_YESTERDAY;
                    break;
                case TemperatureReadings.WindChill:
                    name = X320MConstants.TEMP_WIND_CHILL;
                    break;
                case TemperatureReadings.None:
                default:
                    break;
            }
            return name;
        }

        /// <summary>
        /// Gets the state element name of the specified barometer reading type.
        /// </summary>
        /// <param name="reading">
        /// The reading to get the element name of.
        /// </param>
        /// <returns>
        /// The element name of the specified reading type.
        /// </returns>
        public static String GetBarometerHistoryName(BarometerReadings reading) {
            String name = String.Empty;
            switch (reading) {
                case BarometerReadings.LastFifteenHours:
                    name = X320MConstants.BAROM_LAST_FIFTEEN_HOURS;
                    break;
                case BarometerReadings.LastHour:
                    name = X320MConstants.BAROM_LAST_HOUR;
                    break;
                case BarometerReadings.LastNineHours:
                    name = X320MConstants.BAROM_LAST_NINE_HOURS;
                    break;
                case BarometerReadings.LastSixHours:
                    name = X320MConstants.BAROM_LAST_SIX_HOURS;
                    break;
                case BarometerReadings.LastThreeHours:
                    name = X320MConstants.BAROM_LAST_THREE_HOURS;
                    break;
                case BarometerReadings.LastTwelveHours:
                    name = X320MConstants.BAROM_LAST_TWELVE_HOURS;
                    break;
                case BarometerReadings.LastTwentyFourHours:
                    name = X320MConstants.BAROM_LAST_TWENTY_FOUR_HOURS;
                    break;
                case BarometerReadings.None:
                default:
                    break;
            }
            return name;
        }
	}
}

