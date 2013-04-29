using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// The state of a ControlByWeb X-300 module when in temperature monitor
    /// mode.
    /// </summary>
    public class X300TempMonitorState : DeviceStateBase
    {
        #region Fields
        private TemperatureUnits _units = TemperatureUnits.Fahrenheit;
        private SensorInput _sensor1 = null;
        private SensorInput _sensor2 = null;
        private SensorInput _sensor3 = null;
        private SensorInput _sensor4 = null;
        private SensorInput _sensor5 = null;
        private SensorInput _sensor6 = null;
        private SensorInput _sensor7 = null;
        private SensorInput _sensor8 = null;
        private Relay _relay1 = null;
        private Relay _relay2 = null;
        private Relay _relay3 = null;
        private readonly X300OperationMode _mode = X300OperationMode.TemperatureMonitor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300TempMonitorState</b>
        /// class. This is the default constructor.
        /// </summary>
        public X300TempMonitorState()
            : base() {
            this._sensor1 = new SensorInput();
            this._sensor2 = new SensorInput();
            this._sensor3 = new SensorInput();
            this._sensor4 = new SensorInput();
            this._sensor5 = new SensorInput();
            this._sensor6 = new SensorInput();
            this._sensor7 = new SensorInput();
            this._sensor8 = new SensorInput();
            this._relay1 = new Relay();
            this._relay2 = new Relay();
            this._relay3 = new Relay();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unit of measure for the temperature.
        /// </summary>
        public TemperatureUnits Units {
            get { return this._units; }
        }

        /// <summary>
        /// Gets the first sensor input.
        /// </summary>
        public SensorInput SensorInput1 {
            get { return this._sensor1; }
        }

        /// <summary>
        /// Gets the second sensor input.
        /// </summary>
        public SensorInput SensorInput2 {
            get { return this._sensor2; }
        }

        /// <summary>
        /// Gets the third sensor input.
        /// </summary>
        public SensorInput SensorInput3 {
            get { return this._sensor3; }
        }

        /// <summary>
        /// Gets the fourth sensor input.
        /// </summary>
        public SensorInput SensorInput4 {
            get { return this._sensor4; }
        }

        /// <summary>
        /// Gets the fifth sensor input.
        /// </summary>
        public SensorInput SensorInput5 {
            get { return this._sensor5; }
        }

        /// <summary>
        /// Gets the sixth sensor input.
        /// </summary>
        public SensorInput SensorInput6 {
            get { return this._sensor6; }
        }

        /// <summary>
        /// Gets the seventh sensor input.
        /// </summary>
        public SensorInput SensorInput7 {
            get { return this._sensor7; }
        }

        /// <summary>
        /// Gets the eighth sensor input.
        /// </summary>
        public SensorInput SensorInput8 {
            get { return this._sensor8; }
        }

        /// <summary>
        /// Gets or sets the first relay.
        /// </summary>
        public Relay Relay1 {
            get { return this._relay1; }
            set { this._relay1 = value; }
        }

        /// <summary>
        /// Gets or sets the second relay.
        /// </summary>
        public Relay Relay2 {
            get { return this._relay2; }
            set { this._relay2 = value; }
        }

        /// <summary>
        /// Gets or sets the third relay.
        /// </summary>
        public Relay Relay3 {
            get { return this._relay3; }
            set { this._relay3 = value; }
        }

        /// <summary>
        /// Get the current mode of operation for the device.
        /// </summary>
        public X300OperationMode Mode {
            get { return this._mode; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicitly sets or overrides the specified input.
        /// </summary>
        /// <param name="input">
        /// The number of the input to set.
        /// </param>
        /// <param name="sensor">
        /// The sensor input to assign to the specified input.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// input must be an integer value not less than one and not greater
        /// than eight (because there are exactly eight inputs).
        /// </exception>
        public void SetSensorInput(Int32 input, SensorInput sensor) {
            if ((input < 1) || (input > X300Constants.TOTAL_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X300Constants.TOTAL_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("input", err);
            }

            switch (input) {
                case 1: this._sensor1 = sensor; break;
                case 2: this._sensor2 = sensor; break;
                case 3: this._sensor3 = sensor; break;
                case 4: this._sensor4 = sensor; break;
                case 5: this._sensor5 = sensor; break;
                case 6: this._sensor6 = sensor; break;
                case 7: this._sensor7 = sensor; break;
                case 8: this._sensor8 = sensor; break;
            }
        }

        /// <summary>
        /// Convenience method for getting a relay reference by number.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to get.
        /// </param>
        /// <returns>
        /// The requested relay.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value not less than one and not greater
        /// than three (because there are exactly three relays).
        /// </exception>
        public Relay GetRelay(Int32 relayNum) {
            if ((relayNum < 1) || (relayNum > X300Constants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X300Constants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            Relay rel = null;
            switch (relayNum) {
                case 1: rel = this.Relay1; break;
                case 2: rel = this.Relay2; break;
                case 3: rel = this.Relay3; break;
            }
            return rel;
        }

        /// <summary>
        /// Convenience method for getting a sensor input reference by number.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to get.
        /// </param>
        /// <returns>
        /// The requested input.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// inputNum must be an integer value not less than one and not greater
        /// than eight (because there are exactly eight inputs).
        /// </exception>
        public SensorInput GetSensorInput(Int32 inputNum) {
            if ((inputNum < 1) || (inputNum > X300Constants.TOTAL_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            X300Constants.TOTAL_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("inputNum", err);
            }

            SensorInput input = null;
            switch (inputNum) {
                case 1: input = this.SensorInput1; break;
                case 2: input = this.SensorInput2; break;
                case 3: input = this.SensorInput3; break;
                case 4: input = this.SensorInput4; break;
                case 5: input = this.SensorInput5; break;
                case 6: input = this.SensorInput6; break;
                case 7: input = this.SensorInput7; break;
                case 8: input = this.SensorInput8; break;
            }
            return input;
        }

        /// <summary>
        /// Creates a new X300TempMonitorState that is a deep copy of the
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new X300TempMonitorState that is a copy of this instance.
        /// </returns>
        public X300TempMonitorState Clone() {
            X300TempMonitorState clone = new X300TempMonitorState();
            clone.Relay1 = this.Relay1;
            clone.Relay2 = this.Relay2;
            clone.Relay3 = this.Relay3;
            clone.SetTemperatureUnits(this.Units);
            for (Int32 i = 1; i <= X300Constants.TOTAL_INPUTS; i++) {
                clone.SetSensorInput(i, this.GetSensorInput(i));
            }
            return clone;
        }

        /// <summary>
        /// Resets the state values back to the defaults.
        /// </summary>
        public override void Reset() {
            this._units = TemperatureUnits.Fahrenheit;
            for (Int32 i = 1; i <= X300Constants.TOTAL_INPUTS; i++) {
                this.SetSensorInput(i, new SensorInput());
            }

            Relay rel = null;
            for (Int32 r = 1; r <= X300Constants.TOTAL_RELAYS; r++) {
                rel = this.GetRelay(r);
                if (rel == null) {
                    rel = new Relay();
                    continue;
                }
                rel.Reset();
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the unit of measure to be used for
        /// measuring temperature.
        /// </summary>
        /// <param name="units">
        /// The unit of measure to use for temperature (Celcius or Fahrenheit).
        /// </param>
        public void SetTemperatureUnits(TemperatureUnits units) {
            this._units = units;
        }
        #endregion
    }
}
