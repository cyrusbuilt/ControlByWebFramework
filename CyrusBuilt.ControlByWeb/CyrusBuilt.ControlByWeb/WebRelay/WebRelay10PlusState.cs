using System;
using CyrusBuilt.ControlByWeb.Inputs;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Represents the state of a WebRelay-10 Plus device which includes
    /// standard inputs and sensor inputs.
    /// </summary>
    public class WebRelay10PlusState : WebRelay10State
    {
        #region Fields
        private StandardInput _input1 = null;
        private StandardInput _input2 = null;
        private Double _highTime1 = 0.00;
        private Double _highTime2 = 0.00;
        private TemperatureUnits _units = TemperatureUnits.Fahrenheit;
        private SensorInput _sensor1 = null;
        private SensorInput _sensor2 = null;
        private SensorInput _sensor3 = null;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public WebRelay10PlusState()
            : base() {
            // Construct inputs.
            this._input1 = new StandardInput();
            this._input2 = new StandardInput();

            // Construct sensors.
            this._sensor1 = new SensorInput();
            this._sensor2 = new SensorInput();
            this._sensor3 = new SensorInput();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the first input.
        /// </summary>
        public StandardInput Input1 {
            get { return this._input1; }
        }

        /// <summary>
        /// Gets the second input.
        /// </summary>
        public StandardInput Input2 {
            get { return this._input2; }
        }

        /// <summary>
        /// Gets the amount of time (in seconds) that <see cref="Input1"/> was
        /// last on.
        /// </summary>
        public Double HighTime1 {
            get { return this._highTime1; }
        }

        /// <summary>
        /// Gets the amount of time (in seconds) that <see cref="Input2"/> was
        /// last on.
        /// </summary>
        public Double HighTime2 {
            get { return this._highTime2; }
        }

        /// <summary>
        /// Gets the unit of measure for the temperature sensors.
        /// </summary>
        public TemperatureUnits Units {
            get { return this._units; }
        }

        /// <summary>
        /// Gets the first sensor input.
        /// </summary>
        public SensorInput Sensor1 {
            get { return this._sensor1; }
        }

        /// <summary>
        /// Gets the second sensor input.
        /// </summary>
        public SensorInput Sensor2 {
            get { return this._sensor2; }
        }

        /// <summary>
        /// Gets the third sensor input.
        /// </summary>
        public SensorInput Sensor3 {
            get { return this._sensor3; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convenience method for getting an input by number.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to get (1 or 2);
        /// </param>
        /// <returns>
        /// The requested input or null if <paramref name="inputNum"/> is an
        /// invalid input number or the requested input is a null reference.
        /// </returns>
        public StandardInput GetInput(Int32 inputNum) {
            StandardInput input = null;
            switch (inputNum) {
                case 1: input = this._input1; break;
                case 2: input = this._input2; break;
            }
            return input;
        }

        /// <summary>
        /// Explicitly sets or overrides the specified input.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to set (1 or 2).
        /// </param>
        /// <param name="input">
        /// The input to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputNum"/> must be either 1 or 2.
        /// </exception>
        public void SetOrOverrideInput(Int32 inputNum, StandardInput input) {
            if (input == null) {
                input = new StandardInput();
            }

            switch (inputNum) {
                case 1: this._input1 = input; break;
                case 2: this._input2 = input; break;
                default: throw new ArgumentOutOfRangeException("inputNum", "Must be either 1 or 2.");
            }
        }

        /// <summary>
        /// Convenience method for getting the high time of the specified input.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to get the high time of.
        /// </param>
        /// <returns>
        /// The elapsed time (in seconds) that the specified input was last on
        /// or -1 if <paramref name="inputNum"/> is not a valid input number.
        /// </returns>
        public Double GetHighTime(Int32 inputNum) {
            Double hTime = -1;
            switch (inputNum) {
                case 1: hTime = this._highTime1; break;
                case 2: hTime = this._highTime2; break;
            }
            return hTime;
        }

        /// <summary>
        /// Explicitly sets or overrides the high time for a given input.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to set the high time for (1 or 2).
        /// </param>
        /// <param name="highTime">
        /// The amount of time elapsed (in seconds) since the input was last
        /// switched on.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputNum"/> must be either 1 or 2.
        /// </exception>
        public void SetOrOverrideHighTime(Int32 inputNum, Double highTime) {
            switch (inputNum) {
                case 1: this._highTime1 = highTime; break;
                case 2: this._highTime2 = highTime; break;
                default: throw new ArgumentOutOfRangeException("inputNum", "Must be either 1 or 2.");
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the temperature units of measure.
        /// </summary>
        /// <param name="units">
        /// The units of measure to set.
        /// </param>
        public void SetOrOverrideTempUnits(TemperatureUnits units) {
            this._units = units;
        }

        /// <summary>
        /// Convenience method for getting a sensor by number.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the sensor to get (1 - 3).
        /// </param>
        /// <returns>
        /// The requested sensor, or null if <paramref name="inputNum"/> is an invalid
        /// sensor number or the requested sensor is a null reference.
        /// </returns>
        public SensorInput GetSensor(Int32 inputNum) {
            SensorInput sensor = null;
            switch (inputNum) {
                case 1: sensor = this._sensor1; break;
                case 2: sensor = this._sensor2; break;
                case 3: sensor = this._sensor3; break;
            }
            return sensor;
        }

        /// <summary>
        /// Explicitly sets or overrides the specified sensor input.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to set (1 - 3).
        /// </param>
        /// <param name="sensor">
        /// The sensor to assign to the input.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputNum"/> is an invalid sensor number.
        /// </exception>
        public void SetOrOverrideSensor(Int32 inputNum, SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }

            switch (inputNum) {
                case 1: this._sensor1 = sensor; break;
                case 2: this._sensor2 = sensor; break;
                case 3: this._sensor3 = sensor; break;
                default: throw new ArgumentOutOfRangeException("inputNum", "Must be 1 - 3.");
            }
        }

        /// <summary>
        /// Resets all the state values back to their defaults.
        /// </summary>
        public override void Reset() {
            base.Reset();
            this._units = TemperatureUnits.Fahrenheit;

            // Standard inputs.
            StandardInput input = null;
            for (Int32 i = 1; i <= WebRelay10Constants.TOTAL_STANDARD_INPUTS; i++) {
                input = this.GetInput(i);
                if (input != null) {
                    input.Reset();
                    continue;
                }
                input = new StandardInput();
            }

            // High times.
            Double hTime = 0.00;
            for (Int32 h = 1; h <= WebRelay10Constants.TOTAL_STANDARD_INPUTS; h++) {
                this.SetOrOverrideHighTime(h, hTime);
            }

            // Sensor inputs.
            SensorInput sensor = null;
            for (Int32 s = 1; s <= WebRelay10Constants.TOTAL_SENSOR_INPUTS; s++) {
                sensor = this.GetSensor(s);
                if (sensor != null) {
                    sensor.Reset();
                    continue;
                }
                sensor = new SensorInput();
            }
        }

        /// <summary>
        /// Creates a new WebRelay10PlusState that is a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A WebRelay10PlusState that is a copy of this instance.
        /// </returns>
        public new WebRelay10PlusState Clone() {
            // Standard inputs.
            WebRelay10PlusState clone = new WebRelay10PlusState();
            for (Int32 i = 1; i <= WebRelay10Constants.TOTAL_STANDARD_INPUTS; i++) {
                clone.SetOrOverrideInput(i, this.GetInput(i));
            }

            // Relays.
            for (Int32 r = 1; r <= WebRelay10Constants.TOTAL_RELAYS; r++) {
                clone.SetRelay(r, this.GetRelay(r));
            }

            // External vars.
            for (Int32 e = WebRelay10Constants.EXT_VAR_MIN_ID; e <= WebRelay10Constants.EXT_VAR_MAX_ID; e++) {
                clone.SetOrOverrideExternalVar(e, this.GetExternalVar(e));
            }

            // High times. Yep.
            for (Int32 h = 1; h <= WebRelay10Constants.TOTAL_STANDARD_INPUTS; h++) {
                clone.SetOrOverrideHighTime(h, this.GetHighTime(h));
            }

            // Sensor inputs.
            for (Int32 s = 1; s <= WebRelay10Constants.TOTAL_SENSOR_INPUTS; s++) {
                clone.SetOrOverrideSensor(s, this.GetSensor(s));
            }

            // Everything else.
            clone.SetOrOverrideTempUnits(this.Units);
            clone.SetOrOverrideTime(this.Time);
            return clone;
        }
        #endregion
    }
}
