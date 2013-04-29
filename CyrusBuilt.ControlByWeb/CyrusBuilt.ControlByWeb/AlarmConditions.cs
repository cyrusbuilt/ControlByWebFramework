using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Possible alarm conditions.
    /// </summary>
    public enum AlarmConditions
    {
        /// <summary>
        /// Alarm state is normal (off).
        /// </summary>
        Normal,

        /// <summary>
        /// Alarm condition 1 (High).
        /// </summary>
        High,

        /// <summary>
        /// Alarm condition 2 (Low).
        /// </summary>
        Low
    }
}
