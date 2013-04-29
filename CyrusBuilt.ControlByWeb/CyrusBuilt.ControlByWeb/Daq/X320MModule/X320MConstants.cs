using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
	/// <summary>
	/// Contains value constants specific to the X-320M module.
	/// </summary>
	public static class X320MConstants
	{
		#region Module Constants
		/// <summary>
        /// The total number of relays in the X-320 module (2).
        /// </summary>
        public const Int32 TOTAL_RELAYS = 2;

        /// <summary>
        /// The total number of auxiliary inputs (2).
        /// </summary>
        public const Int32 TOTAL_AUX_INPUTS = 2;
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

        #region Rainfall History Constants
        /// <summary>
        /// Rainfall for the last hour.
        /// </summary>
        public const String RAIN_LAST_HOUR = "rain1h";

        /// <summary>
        /// Rainfall for the last 24 hours.
        /// </summary>
        public const String RAIN_TOTAL_TODAY = "rainToday";

        /// <summary>
        /// Rainfall for the last 7 days.
        /// </summary>
        public const String RAIN_TOTAL_LAST_SEVEN_DAYS = "rain7d";
        #endregion

        #region Humidity History Constants
        /// <summary>
        /// The highest relative humidity reading for today.
        /// </summary>
        public const String HUMIDITY_HIGH_TODAY = "humidityH";

        /// <summary>
        /// The highest relative humidity reading for yesterday.
        /// </summary>
        public const String HUMIDITY_HIGH_YESTERDAY = "humidityHY";

        /// <summary>
        /// The lowest relative humidity reading for today.
        /// </summary>
        public const String HUMIDITY_LOW_TODAY = "humidityL";

        /// <summary>
        /// The lowest relative humidity reading for yesterday.
        /// </summary>
        public const String HUMIDITY_LOW_YESTERDAY = "humidityLY";
        #endregion

        #region Temperature History Constants
        /// <summary>
        /// The highest temperature for today.
        /// </summary>
        public const String TEMP_HIGH_TODAY = "tempH";

        /// <summary>
        /// The highest temperature for yesterday.
        /// </summary>
        public const String TEMP_HIGH_YESTERDAY = "tempHY";

        /// <summary>
        /// The lowest temperature for today.
        /// </summary>
        public const String TEMP_LOW_TODAY = "tempL";

        /// <summary>
        /// The highest temperature for yesterday.
        /// </summary>
        public const String TEMP_LOW_YESTERDAY = "tempHY";

        /// <summary>
        /// The heat index.
        /// </summary>
        public const String TEMP_HEAT_INDEX = "heatIndex";

        /// <summary>
        /// The wind chill.
        /// </summary>
        public const String TEMP_WIND_CHILL = "windChill";

        /// <summary>
        /// The dew point.
        /// </summary>
        public const String TEMP_DEW_POINT = "dewPoint";
        #endregion

        #region Barometric Pressure History Constants
        /// <summary>
        /// The barometric pressure reading for the last hour.
        /// </summary>
        public const String BAROM_LAST_HOUR = "presN1";

        /// <summary>
        /// The barometric pressure reading for the last three hours.
        /// </summary>
        public const String BAROM_LAST_THREE_HOURS = "presN3";

        /// <summary>
        /// The barometric pressure reading for the last six hours.
        /// </summary>
        public const String BAROM_LAST_SIX_HOURS = "presN6";

        /// <summary>
        /// The barometric pressure reading for the last nine hours.
        /// </summary>
        public const String BAROM_LAST_NINE_HOURS = "presN9";

        /// <summary>
        /// The barometric pressure reading for the last twelve hours.
        /// </summary>
        public const String BAROM_LAST_TWELVE_HOURS = "presN12";

        /// <summary>
        /// The barometric pressure reading for the last fiftenn hours.
        /// </summary>
        public const String BAROM_LAST_FIFTEEN_HOURS = "presN15";

        /// <summary>
        /// The barometric pressure reading for the last twenty-four hours.
        /// </summary>
        public const String BAROM_LAST_TWENTY_FOUR_HOURS = "presN24";
        #endregion
    }
}

