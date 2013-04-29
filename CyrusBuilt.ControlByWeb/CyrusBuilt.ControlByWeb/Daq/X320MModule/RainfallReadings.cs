using System;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Rainfall history readings.
    /// </summary>
    public enum RainfallReadings
    {
        /// <summary>
        /// Rainfall for the last hour.
        /// </summary>
        LastHour,

        /// <summary>
        /// Total rainfall for today.
        /// </summary>
        TotalToday,

        /// <summary>
        /// Total rainfall for the last 7 days.
        /// </summary>
        TotalSevenDays,

        /// <summary>
        /// No rainfall.
        /// </summary>
        None
    }
}
