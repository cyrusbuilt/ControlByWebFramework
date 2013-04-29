using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Barometer history readings.
    /// </summary>
    public enum BarometerReadings
    {
        /// <summary>
        /// Barometric pressure for the last hour.
        /// </summary>
        LastHour,

        /// <summary>
        /// Barometric pressure for the last 3 hours.
        /// </summary>
        LastThreeHours,

        /// <summary>
        /// Barometric pressure for the last 6 hours.
        /// </summary>
        LastSixHours,

        /// <summary>
        /// Barometric pressure for the last 9 hours.
        /// </summary>
        LastNineHours,

        /// <summary>
        /// Barometric pressure for the last 12 hours.
        /// </summary>
        LastTwelveHours,

        /// <summary>
        /// Barometric pressure for the last 15 hours.
        /// </summary>
        LastFifteenHours,

        /// <summary>
        /// Barometric pressure for the last 24 hours.
        /// </summary>
        LastTwentyFourHours,

        /// <summary>
        /// No pressure reading.
        /// </summary>
        None
    }
}
