using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.TemperatureModule
{
    /// <summary>
    /// The state of a ControlByWeb Temperature Module.
    /// </summary>
    public class TemperatureModuleState : DeviceStateBase
    {
        #region Fields
        private TemperatureUnits _units = TemperatureUnits.Fahrenheit;
        private Relay _relay1 = null;
        private Relay _relay2 = null;
        private SensorInput _input1 = null;
        private SensorInput _input2 = null;
        private SensorInput _input3 = null;
        private SensorInput _input4 = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.TemperatureModule.TemperatureModuleState</b> class.
        /// This is the default constructor.
        /// </summary>
        public TemperatureModuleState()
            : base() {
            this._relay1 = new Relay();
            this._relay2 = new Relay();
            this._input1 = new SensorInput();
            this._input2 = new SensorInput();
            this._input3 = new SensorInput();
            this._input4 = new SensorInput();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unit of measure for the temperature.
        /// </summary>
        public TemperatureUnits Units {
            get { return this._units; }
            set { this._units = value; }
        }

        /// <summary>
        /// Gets or sets the state of relay 1.
        /// </summary>
        public Relay Relay1 {
            get { return this._relay1; }
            set { this._relay1 = value; }
        }

        /// <summary>
        /// Gets or sets the state of relay 2.
        /// </summary>
        public Relay Relay2 {
            get { return this._relay2; }
            set { this._relay2 = value; }
        }

        /// <summary>
        /// Gets or sets the first input.
        /// </summary>
        public SensorInput Input1 {
            get { return this._input1; }
            set { this._input1 = value; }
        }

        /// <summary>
        /// Gets or sets the second input.
        /// </summary>
        public SensorInput Input2 {
            get { return this._input2; }
            set { this._input2 = value; }
        }

        /// <summary>
        /// Gets or sets the third input.
        /// </summary>
        public SensorInput Input3 {
            get { return this._input3; }
            set { this._input3 = value; }
        }

        /// <summary>
        /// Gets or sets the fourth input.
        /// </summary>
        public SensorInput Input4 {
            get { return this._input4; }
            set { this._input4 = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public TemperatureModuleState Clone() {
            TemperatureModuleState clone = new TemperatureModuleState();
            clone.Input1 = this.Input1;
            clone.Input2 = this.Input2;
            clone.Input3 = this.Input3;
            clone.Input4 = this.Input4;
            clone.Relay1 = this.Relay1;
            clone.Relay2 = this.Relay2;
            clone.Units = this.Units;
            return clone;
        }

        /// <summary>
        /// Convenience method for getting an input reference by number.
        /// </summary>
        /// <param name="inputNumber">
        /// The number of the input to get.
        /// </param>
        /// <returns>
        /// The requested input.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// inputNumber must be an integer value not less than one and not
        /// greater than four (because there are exactly four inputs).
        /// </exception>
        public SensorInput GetInput(Int32 inputNumber) {
            if ((inputNumber < 1) || (inputNumber > TempModuleConstants.TOTAL_SENSOR_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            TempModuleConstants.TOTAL_SENSOR_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("inputNumber", err);
            }

            SensorInput si = null;
            switch (inputNumber) {
                case 1:
                    si = this.Input1;
                    break;
                case 2:
                    si = this.Input2;
                    break;
                case 3:
                    si = this.Input3;
                    break;
                case 4:
                    si = this.Input4;
                    break;
            }
            return si;
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
        /// than two (because there are exactly two relays).
        /// </exception>
        public Relay GetRelay(Int32 relayNum) {
            if ((relayNum < 1) || (relayNum > TempModuleConstants.TOTAL_RELAYS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            TempModuleConstants.TOTAL_RELAYS.ToString());
                throw new ArgumentOutOfRangeException("relayNum", err);
            }

            Relay rel = null;
            switch (relayNum) {
                case 1:
                    rel = this.Relay1;
                    break;
                case 2:
                    rel = this.Relay2;
                    break;
            }
            return rel;
        }

        /// <summary>
        /// Resets the state values back to default.
        /// </summary>
        public override void Reset() {
            this._units = TemperatureUnits.Fahrenheit;
            Relay rel = null;
            for (Int32 r = 1; r <= TempModuleConstants.TOTAL_RELAYS; r++) {
                rel = this.GetRelay(r);
                if (rel == null) {
                    rel = new Relay();
                    continue;
                }
                rel.Reset();
            }

            SensorInput si = null;
            for (Int32 i = 1; i <= TempModuleConstants.TOTAL_SENSOR_INPUTS; i++) {
                si = this.GetInput(i);
                if (si == null) {
                    si = new SensorInput();
                    continue;
                }
                si.Reset();
            }
        }

        /// <summary>
        /// Converts the current instance of <see cref="TemperatureModuleState"/>
        /// to a string.
        /// </summary>
        /// <returns>
        /// The string representation of the current <see cref="TemperatureModuleState"/>
        /// instance.
        /// </returns>
        public override String ToString()
        {
            // Get the temperature units.
            String s = String.Empty;
            String unit = "F";
            if (this._units == TemperatureUnits.Celcius)
            {
                unit = "C";
            }

            // Get each sensor input.
            s = String.Concat("Units: ", unit, Environment.NewLine);
            String inp = String.Empty;
            SensorInput input = null;
            for (Int32 i = 1; i <= TempModuleConstants.TOTAL_SENSOR_INPUTS; i++)
            {
                input = this.GetInput(i);
                if (input == null)
                {
                    // Input disassociated.
                    inp = String.Format("Input {0}: No input.", i.ToString());
                }
                else
                {
                    if (input.HasSensor)
                    {
                        inp = String.Format("Input {0} has sensor: Yes. Temp: {1}.",
                                            i.ToString(),
                                            input.Temperature.ToString());
                    }
                    else
                    {
                        // No sensor.
                        inp = String.Format("Input {0} has sensor: No.",
                                            i.ToString());
                    }
                }
                s = String.Concat(s, inp, Environment.NewLine);
            }

            // Get the relays.
            Relay rel = null;
            String relStr = String.Empty;
            for (Int32 x = 1; x <= TempModuleConstants.TOTAL_RELAYS; x++)
            {
                rel = this.GetRelay(x);
                if (rel == null)
                {
                    // No relay.
                    relStr = String.Format("Relay {0}: No relay.", x.ToString());
                }
                else
                {
                    relStr = String.Format("Relay {0} state: ", x.ToString());
                    switch (rel.State)
                    {
                        case RelayState.On:
                            relStr = String.Concat(relStr, "On");
                            break;
                        case RelayState.Off:
                            relStr = String.Concat(relStr, "Off");
                            break;
                        case RelayState.DisableAutoReboot:
                            relStr = String.Concat(relStr, "DisableAutoReboot");
                            break;
                        case RelayState.EnableAutoReboot:
                            relStr = String.Concat(relStr, "EnableAutoReboot");
                            break;
                        case RelayState.Pulse:
                            // Set to pulse, so we need the pulse time too.
                            String pulseTime = rel.PulseTime.ToString();
                            relStr = String.Concat(relStr, "Pulse. Pulse time: ", pulseTime);
                            break;
                        case RelayState.Reboot:
                            relStr = String.Concat(relStr, "Reboot");
                            break;
                        case RelayState.Toggle:
                            relStr = String.Concat(relStr, "Toggle");
                            break;
                    }
                }
                s = String.Concat(s, relStr, Environment.NewLine);
            }
            return s;
        }
        #endregion
    }
}
