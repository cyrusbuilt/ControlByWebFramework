using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Static utility methods for events.
    /// </summary>
    public static class EventUtils
    {
        #region Methods
        /// <summary>
        /// Gets the period unit associated with the specified period code.
        /// </summary>
        /// <param name="period">
        /// The integer value associated with the requested period. See the
        /// period constants in <see cref="EventConstants"/>. Any value not
        /// equal to one of the value constants will cause <see cref="PeriodUnits.Disabled"/>
        /// to be returned.
        /// </param>
        /// <returns>
        /// Returns the period unit associated with the specified value.
        /// </returns>
        public static PeriodUnits GetPeriod(Int32 period) {
            PeriodUnits unit = PeriodUnits.Disabled;
            switch (period) {
                case EventConstants.PERIOD_SECONDS:
                    break;
                case EventConstants.PERIOD_MINUTES:
                    unit = PeriodUnits.Minutes;
                    break;
                case EventConstants.PERIOD_HOURS:
                    unit = PeriodUnits.Hours;
                    break;
                case EventConstants.PERIOD_DAYS:
                    unit = PeriodUnits.Days;
                    break;
                case EventConstants.PERIOD_WEEKS:
                    unit = PeriodUnits.Weeks;
                    break;
            }
            return unit;
        }

        /// <summary>
        /// Gets an integer value code associated with the specified period
        /// unit.
        /// </summary>
        /// <param name="unit">
        /// The period unit to get the associated code of.
        /// </param>
        public static Int32 GetPeriodCode(PeriodUnits unit) {
            Int32 unitCode = EventConstants.PERIOD_DISABLED;
            switch (unit) {
                case PeriodUnits.Seconds:
                    break;
                case PeriodUnits.Minutes:
                    unitCode = EventConstants.PERIOD_MINUTES;
                    break;
                case PeriodUnits.Hours:
                    unitCode = EventConstants.PERIOD_HOURS;
                    break;
                case PeriodUnits.Days:
                    unitCode = EventConstants.PERIOD_DAYS;
                    break;
                case PeriodUnits.Weeks:
                    unitCode = EventConstants.PERIOD_WEEKS;
                    break;
                case PeriodUnits.Disabled:
                    unitCode = EventConstants.PERIOD_DISABLED;
                    break;
            }
            return unitCode;
        }

        /// <summary>
        /// Gets the period unit associated with the specified string.
        /// </summary>
        /// <param name="input">
        /// Specify "s" for seconds, "m" for minutes, "h" for hours, "d" for
        /// days, "w" for weeks, or "0" for disabled.
        /// </param>
        /// <returns>
        /// The period associated with the specified string. If an invalid
        /// string was specified, then returns <see cref="PeriodUnits.Disabled"/>.
        /// </returns>
        public static PeriodUnits GetPeriodFromString(String input) {
            if (String.IsNullOrEmpty(input)) {
                return PeriodUnits.Disabled;
            }

            PeriodUnits unit = PeriodUnits.Disabled;
            input = input.Trim().ToLower();
            if (input == "s") {
                unit = PeriodUnits.Seconds;
            }
            if (input == "0") {
                unit = PeriodUnits.Disabled;
            }
            if (input == "m") {
                unit = PeriodUnits.Minutes;
            }
            if (input == "h") {
                unit = PeriodUnits.Hours;
            }
            if (input == "d") {
                unit = PeriodUnits.Days;
            }
            if (input == "w") {
                unit = PeriodUnits.Weeks;
            }
            return unit;
        }
        #endregion
    }
}
