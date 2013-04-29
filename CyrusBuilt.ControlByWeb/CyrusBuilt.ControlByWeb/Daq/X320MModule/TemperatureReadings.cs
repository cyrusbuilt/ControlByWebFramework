using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Historical and calculated temperature reading types.
    /// </summary>
    public enum TemperatureReadings
    {
        /// <summary>
        /// Today's highest temperature.
        /// </summary>
        HighTempToday,

        /// <summary>
        /// Today's lowest temperature.
        /// </summary>
        LowTempToday,

        /// <summary>
        /// Yesterday's highest temperature.
        /// </summary>
        HighTempYesterday,

        /// <summary>
        /// Yesterday's lowest temperature.
        /// </summary>
        LowTempYesterday,

        /// <summary>
        /// Current heat index.
        /// </summary>
        HeatIndex,

        /// <summary>
        /// Current wind chill.
        /// </summary>
        WindChill,

        /// <summary>
        /// Current dew point.
        /// </summary>
        DewPoint,

        /// <summary>
        /// No reading.
        /// </summary>
        None
    }
}
