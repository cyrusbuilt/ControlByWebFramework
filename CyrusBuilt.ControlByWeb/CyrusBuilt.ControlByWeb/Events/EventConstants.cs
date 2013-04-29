using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Defines value constants specific to events.
    /// </summary>
    public static class EventConstants
    {
        #region Period Constants
        /// <summary>
        /// The time period in seconds.
        /// </summary>
        public const Int32 PERIOD_SECONDS = 0;

        /// <summary>
        /// The time period in minutes.
        /// </summary>
        public const Int32 PERIOD_MINUTES = 1;

        /// <summary>
        /// The time period in hours.
        /// </summary>
        public const Int32 PERIOD_HOURS = 2;

        /// <summary>
        /// The time period in days.
        /// </summary>
        public const Int32 PERIOD_DAYS = 3;

        /// <summary>
        /// The time period in weeks.
        /// </summary>
        public const Int32 PERIOD_WEEKS = 4;

        /// <summary>
        /// The period is disabled.
        /// </summary>
        /// <remarks>
        /// When disabling an event, you should specify "0" for the period.
        /// A period value of "0 s" on the other hand, would mean zero seconds.
        /// This value is strictly for logic purposes.
        /// </remarks>
        public const Int32 PERIOD_DISABLED = 5;
        #endregion

        #region Event Constants
        /// <summary>
        /// The minimum allowable event ID value (0).
        /// </summary>
        public const Int32 EVENT_MIN_ID = 0;

        /// <summary>
        /// The maximum allowable event ID value (99).
        /// </summary>
        public const Int32 EVENT_MAX_ID = 99;

        /// <summary>
        /// The maximum length of an event description string.
        /// </summary>
        public const Int32 DESCRIPTION_MAX_LENGTH = 20;
        #endregion

        #region Repitition Constants
        /// <summary>
        /// Causes an event to repeat continuously.
        /// </summary>
        public const Int32 REPEAT_CONTINUOUS = 0;
        #endregion
    }
}
