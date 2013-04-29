using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Defines periods of time for an event to occur.
    /// </summary>
    public enum PeriodUnits
    {
        /// <summary>
        /// The period is in seconds.
        /// </summary>
        Seconds,

        /// <summary>
        /// The period is in minutes.
        /// </summary>
        Minutes,

        /// <summary>
        /// The period is in hours.
        /// </summary>
        Hours,

        /// <summary>
        /// The period is in days.
        /// </summary>
        Days,

        /// <summary>
        /// The period is in weeks.
        /// </summary>
        Weeks,

        /// <summary>
        /// Event is disabled.
        /// </summary>
        Disabled
    }
}
