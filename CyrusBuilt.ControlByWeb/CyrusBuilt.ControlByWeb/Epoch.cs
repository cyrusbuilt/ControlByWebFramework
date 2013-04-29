using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Represents an Epoch (Unix) time value. Epoch (Unix) time is defined as
    /// being the number of seconds elapsed since January 1, 1970 (known as the
    /// epoch).
    /// </summary>
    public class Epoch
    {
        #region Fields
        private Int64 _value = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Epoch</b>
        /// class. This is the default constructor.
        /// </summary>
        public Epoch() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Epoch</b>
        /// class with the Epoch (Unix time) value.
        /// </summary>
        /// <param name="value">
        /// The number of seconds elapsed since the epoch.
        /// </param>
        public Epoch(Int64 value) {
            this._value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the value of the Epoch time (number of seconds elapsed).
        /// </summary>
        public Int64 Value {
            get { return this._value; }
        }

        /// <summary>
        /// Gets the Epoch time (Unix time) based on the current date and time.
        /// </summary>
        public static Epoch Now {
            get {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return new Epoch(Convert.ToInt64((DateTime.Now - epoch).TotalSeconds));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts the current Epoch instance into a <see cref="DateTime"/> value.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime"/> representation of this Epoch time instance.
        /// </returns>
        public DateTime ToDateTime() {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(this._value);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> value into an Epoch value.
        /// </summary>
        /// <param name="date">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// A new Epoch based on the provided <see cref="DateTime"/>. If the
        /// specified date is null, then null is returned.
        /// </returns>
        public static Epoch FromDateTime(DateTime date) {
            // We treat DateTime.MinValue like null here because DateTime is
            // not a nullable type.
            if (date == DateTime.MinValue) {
                return null;
            }
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return new Epoch(Convert.ToInt64((date - epoch).TotalSeconds));
        }

        /// <summary>
        /// Returns as <see cref="System.String"/> that represents the current
        /// <see cref="CyrusBuilt.ControlByWeb.Epoch"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="CyrusBuilt.ControlByWeb.Epoch"/>.
        /// </returns>
        public override string ToString() {
            return this._value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Epoch Parse(String input) {
            return new Epoch(Int64.Parse(input));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public static Boolean TryParse(String input, out Epoch epoch) {
            Int64 result = 0;
            if (Int64.TryParse(input, out result)) {
                epoch = new Epoch(result);
                return true;
            }
            else {
                epoch = null;
                return false;
            }
        }
        #endregion
    }
}
