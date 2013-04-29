using System;
using System.Net.NetworkInformation;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
	/// <summary>
	/// Represents the state of a ControlByWeb WebSwitch Plus device. Note that most 
    /// of the properties of this object are read-only by user code, so most of
    /// the properties contain getters only. You can use the set methods to 
    /// explicitly to set or override the values if necessary (such as from 
    /// methods that populate the properties with values parsed from a response
    /// from the device).
	/// </summary>
	public class WebSwitchPlusState : DeviceStateBase
    {
		#region Fields
		private Relay _relay1 = null;
		private Relay _relay2 = null;
		private RebootState _rebootState1 = RebootState.Pinging;
		private RebootState _rebootState2 = RebootState.Pinging;
		private Int32 _failures1 = 0;
		private Int32 _failures2 = 0;
		private Int32 _rebootAttempts1 = 0;
		private Int32 _rebootAttempts2 = 0;
		private Int32 _totalReboots1 = 0;
		private Int32 _totalReboots2 = 0;
		private StandardInput _input1 = null;
		private StandardInput _input2 = null;
		private TemperatureUnits _units = TemperatureUnits.Fahrenheit;
		private SensorInput _sensor1 = null;
		private SensorInput _sensor2 = null;
		private SensorInput _sensor3 = null;
		private Double _extVar0 = 0.0;
		private Double _extVar1 = 0.0;
		private Double _extVar2 = 0.0;
		private Double _extVar3 = 0.0;
		private Double _extVar4 = 0.0;
		private PhysicalAddress _serial = null;
		private Epoch _time = null;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusState"/>
		/// class. This is the default constructor.
		/// </summary>
		public WebSwitchPlusState()
            : base() {
            // Construct relays.
			this._relay1 = new Relay();
			this._relay2 = new Relay();

            // Construct standard inputs.
			this._input1 = new StandardInput();
			this._input2 = new StandardInput();

            // Construct sensor inputs.
			this._sensor1 = new SensorInput();
			this._sensor2 = new SensorInput();
			this._sensor3 = new SensorInput();
		}
		#endregion
		
		#region Properties
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
		/// Gets the reboot state of the first relay.
		/// </summary>
		public RebootState RebootState1 {
			get { return this._rebootState1; }
		}
		
		/// <summary>
		/// Gets the reboot state of the second relay.
		/// </summary>
		public RebootState RebootState2 {
			get { return this._rebootState2; }
		}
		
		/// <summary>
		/// Gets the reboot failures for the first relay.
		/// </summary>
		public Int32 RebootFailures1 {
			get { return this._failures1; }
		}
		
		/// <summary>
		/// Gets the reboot failures for the second relay.
		/// </summary>
		public Int32 RebootFailures2 {
			get { return this._failures2; }
		}
		
		/// <summary>
		/// Gets the reboot attempts for the first relay.
		/// </summary>
		public Int32 RebootAttempts1 {
			get { return this._rebootAttempts1; }
		}
		
		/// <summary>
		/// Gets the reboot attempts for the second relay.
		/// </summary>
		public Int32 RebootAttempts2 {
			get { return this._rebootAttempts2; }
		}
		
		/// <summary>
		/// Gets the total reboots for the first relay.
		/// </summary>
		public Int32 TotalReboots1 {
			get { return this._totalReboots1; }
		}
		
		/// <summary>
		/// Gets the total reboots for the second relay.
		/// </summary>
		public Int32 TotalReboots2 {
			get { return this._totalReboots2; }
		}
		
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
		/// Gets the unit of measure for the sensor temperature.
		/// </summary>
		public TemperatureUnits Units {
			get { return this._units; }
		}
		
		/// <summary>
		/// Gets the first sensor.
		/// </summary>
		public SensorInput Sensor1 {
			get { return this._sensor1; }
		}
		
		/// <summary>
		/// Gets the second sensor.
		/// </summary>
		public SensorInput Sensor2 {
			get { return this._sensor2; }
		}
		
		/// <summary>
		/// Gets the third sensor.
		/// </summary>
		public SensorInput Sensor3 {
			get { return this._sensor3; }
		}
		
		/// <summary>
		/// Gets the value of exteral variable zero.
		/// </summary>
		public Double ExtVar0 {
			get { return this._extVar0; }
		}
		
		/// <summary>
		/// Gets the value of external variable one.
		/// </summary>
		public Double ExtVar1 {
			get { return this._extVar1; }
		}
		
		/// <summary>
		/// Gets the value of external variable two.
		/// </summary>
		public Double ExtVar2 {
			get { return this._extVar2; }
		}
		
		/// <summary>
		/// Gets the value of external variable three.
		/// </summary>
		public Double ExtVar3 {
			get { return this._extVar3; }
		}
		
		/// <summary>
		/// Gets the value of external variable four.
		/// </summary>
		public Double ExtVar4 {
			get { return this._extVar4; }
		}
		
		/// <summary>
		/// Gets the serial.
		/// </summary>
		public PhysicalAddress Serial {
			get { return this._serial; }
		}
		
		/// <summary>
		/// Gets the time as reported by the device..
		/// </summary>
		public Epoch Time {
			get { return this._time; }
		}
		#endregion
		
		#region Methods
        /// <summary>
        /// Explicitly sets or overrides the reboot state for the first relay.
        /// </summary>
        /// <param name="state">
        /// The state to set.
        /// </param>
        public void SetOrOverrideRebootState1(RebootState state) {
            this._rebootState1 = state;
        }

        /// <summary>
        /// Explicitly sets or overrides the reboot state for the second relay.
        /// </summary>
        /// <param name="state">
        /// The state to set.
        /// </param>
        public void SetOrOverrideRebootState2(RebootState state) {
            this._rebootState2 = state;
        }

        /// <summary>
        /// Explicitly sets or overrides the number of reboot failures for the
        /// first relay.
        /// </summary>
        /// <param name="failures">
        /// The number of failures to set.
        /// </param>
        public void SetOrOverrideRebootFailures1(Int32 failures) {
            if (failures < 0) {
                failures = 0;
            }
            this._failures1 = failures;
        }

        /// <summary>
        /// Explicitly sets or overrides the number of reboot failures for the
        /// second relay.
        /// </summary>
        /// <param name="failures">
        /// The number of failures to set.
        /// </param>
        public void SetOrOverrideRebootFailures2(Int32 failures) {
            if (failures < 0) {
                failures = 0;
            }
            this._failures2 = failures;
        }

        /// <summary>
        /// Explicitly sets or overrides the number of reboot attempts for the
        /// first relay.
        /// </summary>
        /// <param name="attempts">
        /// The number of reboot attempts to set.
        /// </param>
        public void SetOrOverrideRebootAttempts1(Int32 attempts) {
            if (attempts < 0) {
                attempts = 0;
            }
            this._rebootAttempts1 = attempts;
        }

        /// <summary>
        /// Explicitly sets or overrides the number of reboot attempts for the
        /// second relay.
        /// </summary>
        /// <param name="attempts">
        /// The number of reboot attempts to set.
        /// </param>
        public void SetOrOverrideRebootAttempts2(Int32 attempts) {
            if (attempts < 0) {
                attempts = 0;
            }
            this._rebootAttempts2 = attempts;
        }

        /// <summary>
        /// Explicitly sets or overrides the total number of reboots for the
        /// first relay.
        /// </summary>
        /// <param name="reboots">
        /// The total number of reboots to set.
        /// </param>
        public void SetOrOverrideTotalReboots1(Int32 reboots) {
            if (reboots < 0) {
                reboots = 0;
            }
            this._totalReboots1 = reboots;
        }

        /// <summary>
        /// Explicitly sets or overrides the total number of reboots for the
        /// second relay.
        /// </summary>
        /// <param name="reboots">
        /// The total number of reboots to set.
        /// </param>
        public void SetOrOverrideTotalReboots2(Int32 reboots) {
            if (reboots < 0) {
                reboots = 0;
            }
            this._totalReboots2 = reboots;
        }

        /// <summary>
        /// Explicitly sets or overrides the first input.
        /// </summary>
        /// <param name="input">
        /// The input to set.
        /// </param>
        public void SetOrOverrideInput1(StandardInput input) {
            if (input == null) {
                input = new StandardInput();
            }
            this._input1 = input;
        }

        /// <summary>
        /// Explicitly sets or overrides the second input.
        /// </summary>
        /// <param name="input">
        /// The input to set.
        /// </param>
        public void SetOrOverrideInput2(StandardInput input) {
            if (input == null) {
                input = new StandardInput();
            }
            this._input2 = input;
        }

        /// <summary>
        /// Explicitly sets or overrides the temperature unit of measture.
        /// </summary>
        /// <param name="units">
        /// The unit of measure to set.
        /// </param>
        public void SetOrOverrideTempUnits(TemperatureUnits units) {
            this._units = units;
        }

        /// <summary>
        /// Explicitly sets or overrides the first sensor.
        /// </summary>
        /// <param name="sensor">
        /// The sensor to set.
        /// </param>
        public void SetOrOverrideSensor1(SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }
            this._sensor1 = sensor;
        }

        /// <summary>
        /// Explicitly sets or overrides the second sensor.
        /// </summary>
        /// <param name="sensor">
        /// The sensor to set.
        /// </param>
        public void SetOrOverrideSensor2(SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }
            this._sensor2 = sensor;
        }

        /// <summary>
        /// Explicitly sets or overrides the third sensor.
        /// </summary>
        /// <param name="sensor">
        /// The sensor to set.
        /// </param>
        public void SetOrOverrideSensor3(SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }
            this._sensor3 = sensor;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of external variable zero.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetOrOverrideExtVar0(Double value) {
            this._extVar0 = value;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of external variable one.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetOrOverrideExtVar1(Double value) {
            this._extVar1 = value;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of external variable two.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetOrOverrideExtVar2(Double value) {
            this._extVar2 = value;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of external variable three.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetOrOverrideExtVar3(Double value) {
            this._extVar3 = value;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of external variable four.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetOrOverrideExtVar4(Double value) {
            this._extVar4 = value;
        }

        /// <summary>
        /// Explictly sets or overrides the serial number (MAC) of the device.
        /// </summary>
        /// <param name="serial">
        /// The serial to set.
        /// </param>
        public void SetOrOverrideSerial(PhysicalAddress serial) {
            this._serial = serial;            
        }

        /// <summary>
        /// Explictly sets or overrides the serial number (MAC) of the device.
        /// </summary>
        /// <param name="serial">
        /// The string containing the serial to set.
        /// </param>
        /// <exception cref="FormatException">
        /// <paramref name="serial"/> could not be parsed into a valid
        /// <see cref="PhysicalAddress"/> (serial).
        /// </exception>
        public void SetOrOverrideSerial(String serial) {
            try {
                this.SetOrOverrideSerial(PhysicalAddress.Parse(serial));
            }
            catch (FormatException) {
                throw;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the time on the device.
        /// </summary>
        /// <param name="time">
        /// The time to set (Unix/Epoch time).
        /// </param>
        public void SetOrOverrideTime(Epoch time) {
            if (time == null) {
                time = new Epoch();
            }
            this._time = time;
        }

        /// <summary>
        /// Explicitly sets or overrides the Epoch (Unix) time value.
        /// </summary>
        /// <param name="seconds">
        /// The number of seconds elapsed since January 1, 1970 (epoch).
        /// </param>
        public void SetOrOverrideTime(Int64 seconds) {
            if (seconds >= 0) {
                this._time = new Epoch(seconds);
            }
        }

        /// <summary>
        /// Convenience method for getting a relay by number.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to get.
        /// </param>
        /// <returns>
        /// The requested relay, or null if <paramref name="relayNum"/> is
        /// invalid.
        /// </returns>
        public Relay GetRelay(Int32 relayNum) {
            Relay retRel = null;
            switch (relayNum) {
                case 1: retRel = this._relay1; break;
                case 2: retRel = this._relay2; break;
                default: break;
            }
            return retRel;
        }

        /// <summary>
        /// Convenience method for setting a specific relay.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to set.
        /// </param>
        /// <param name="relay">
        /// The relay to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relayNum"/> must be 1 or 2.
        /// </exception>
        public void SetRelay(Int32 relayNum, Relay relay) {
            if (relay == null) {
                relay = new Relay();
            }
            switch (relayNum) {
                case 1: this._relay1 = relay; break;
                case 2: this._relay2 = relay; break;
                default: throw new ArgumentOutOfRangeException("relayNum", "Value must be 1 or 2.");
            }
        }

        /// <summary>
        /// Convenience method for getting an input by number.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to get.
        /// </param>
        /// <returns>
        /// The requested input, or null if <paramref name="inputNum"/> is
        /// invalid.
        /// </returns>
        public StandardInput GetInput(Int32 inputNum) {
            StandardInput retInp = null;
            switch (inputNum) {
                case 1: retInp = this._input1; break;
                case 2: retInp = this._input2; break;
                default: break;
            }
            return retInp;
        }

        /// <summary>
        /// Convenience method for setting a specific input.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to set.
        /// </param>
        /// <param name="input">
        /// The input to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputNum"/> must be 1 or 2.
        /// </exception>
        public void SetInput(Int32 inputNum, StandardInput input) {
            if (input == null) {
                input = new StandardInput();
            }
            switch (inputNum) {
                case 1: this._input1 = input; break;
                case 2: this._input2 = input; break;
                default: throw new ArgumentOutOfRangeException("inputNum", "Value must be 1 or 2.");
            }
        }

        /// <summary>
        /// Gets a sensor by number.
        /// </summary>
        /// <param name="sensorNum">
        /// The number of the sensor to get.
        /// </param>
        /// <returns>
        /// The requested sensor, or null if <paramref name="sensorNum"/> is
        /// invalid.
        /// </returns>
        public SensorInput GetSensor(Int32 sensorNum) {
            SensorInput sens = null;
            switch (sensorNum) {
                case 1: sens = this._sensor1; break;
                case 2: sens = this._sensor2; break;
                case 3: sens = this._sensor3; break;
                default: break;
            }
            return sens;
        }

        /// <summary>
        /// Convenience method for setting a specific sensor.
        /// </summary>
        /// <param name="sensorNum">
        /// The number of the sensor to set.
        /// </param>
        /// <param name="sensor">
        /// The sensor to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sensorNum"/> must be 1, 2, or 3.
        /// </exception>
        public void SetSensor(Int32 sensorNum, SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }
            switch (sensorNum) {
                case 1: this._sensor1 = sensor; break;
                case 2: this._sensor2 = sensor; break;
                case 3: this._sensor3 = sensor; break;
                default: throw new ArgumentOutOfRangeException("sensorNum", "Value must be 1, 2, or 3");
            }
        }

        /// <summary>
        /// Convenience method for getting the value of the specified external
        /// variable.
        /// </summary>
        /// <param name="varId">
        /// The ID of the external variable to get the value of.
        /// </param>
        /// <returns>
        /// The value of the external variable.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="varId"/> must be a value <see cref="WebSwitchConstants.MIN_EXT_VAR_ID"/>
        /// - <see cref="WebSwitchConstants.MAX_EXT_VAR_ID"/>.
        /// </exception>
        public Double GetExternalVar(Int32 varId) {
            if ((varId < WebSwitchConstants.MIN_EXT_VAR_ID) ||
                (varId > WebSwitchConstants.MAX_EXT_VAR_ID)) {
                    String err = String.Format("Variable ID must be a value {0} - {1}.",
                                                WebSwitchConstants.MIN_EXT_VAR_ID.ToString(),
                                                WebSwitchConstants.MAX_EXT_VAR_ID.ToString());
                    throw new ArgumentOutOfRangeException("varId", err);
            }

            Double val = 0.0;
            switch (varId) {
                case 0: val = this._extVar0; break;
                case 1: val = this._extVar1; break;
                case 2: val = this._extVar2; break;
                case 3: val = this._extVar3; break;
                case 4: val = this._extVar4; break;
            }
            return val;
        }

        /// <summary>
        /// Convenience method for setting an external variable value by ID.
        /// </summary>
        /// <param name="varId">
        /// The ID of the external variable to set.
        /// </param>
        /// <param name="value">
        /// The value to assign to the specified external variable.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="varId"/> must be a value <see cref="WebSwitchConstants.MIN_EXT_VAR_ID"/>
        /// - <see cref="WebSwitchConstants.MAX_EXT_VAR_ID"/>.
        /// </exception>
        public void SetExternalVar(Int32 varId, Double value) {
            if ((varId < WebSwitchConstants.MIN_EXT_VAR_ID) ||
                (varId > WebSwitchConstants.MAX_EXT_VAR_ID)) {
                String err = String.Format("Variable ID must be a value {0} - {1}.",
                                            WebSwitchConstants.MIN_EXT_VAR_ID.ToString(),
                                            WebSwitchConstants.MAX_EXT_VAR_ID.ToString());
                throw new ArgumentOutOfRangeException("varId", err);
            }

            switch (varId) {
                case 0: this._extVar0 = value; break;
                case 1: this._extVar1 = value; break;
                case 2: this._extVar2 = value; break;
                case 3: this._extVar3 = value; break;
                case 4: this._extVar4 = value; break;
            }
        }

        /// <summary>
        /// Resets all the state values back to their defaults.
        /// </summary>
        public override void Reset() {
            // Relays.
            Relay rel = null;
            for (Int32 r = 1; r <= WebSwitchConstants.TOTAL_RELAYS; r++) {
                rel = this.GetRelay(r);
                if (rel != null) {
                    rel.Reset();
                    continue;
                }
                this.SetRelay(r, new Relay());
            }

            // Reboot states.
            this._rebootState1 = RebootState.Pinging;
            this._rebootState2 = RebootState.Pinging;

            // Reboot failures.
            this._failures1 = 0;
            this._failures2 = 0;

            // Reboot attempts.
            this._rebootAttempts1 = 0;
            this._rebootAttempts2 = 0;

            // Total reboots.
            this._totalReboots1 = 0;
            this._totalReboots2 = 0;

            // Inputs.
            StandardInput inp = null;
            for (Int32 i = 1; i <= WebSwitchConstants.TOTAL_INPUTS; i++) {
                inp = this.GetInput(i);
                if (inp != null) {
                    inp.Reset();
                    continue;
                }
                this.SetInput(i, new StandardInput());
            }

            // Temp units.
            this._units = TemperatureUnits.Fahrenheit;

            // Sensors.
            SensorInput sensInp = null;
            for (Int32 s = 1; s <= WebSwitchConstants.TOTAL_SENSORS; s++) {
                sensInp = this.GetSensor(s);
                if (sensInp != null) {
                    sensInp.Reset();
                    continue;
                }
                this.SetSensor(s, new SensorInput());
            }

            // External variables.
            for (Int32 e = WebSwitchConstants.MIN_EXT_VAR_ID; e <= WebSwitchConstants.MAX_EXT_VAR_ID; e++) {
                this.SetExternalVar(e, 0.0);
            }

            // Serial.
            this._serial = null;

            // Time.
            this._time = null;
        }

        /// <summary>
        /// Creates a new instance of WebSwitchPlusState that is a deep copy of the
        /// current instance.
        /// </summary>
        /// <returns>
        /// A WebSwitchPlusState that is a copy of this instance.
        /// </returns>
        public WebSwitchPlusState Clone() {
            WebSwitchPlusState clone = new WebSwitchPlusState();
            clone.Relay1 = this._relay1;
            clone.Relay2 = this._relay2;
            clone.SetOrOverrideRebootState1(this._rebootState1);
            clone.SetOrOverrideRebootState2(this._rebootState2);
            clone.SetOrOverrideRebootFailures1(this._failures1);
            clone.SetOrOverrideRebootFailures2(this._failures1);
            clone.SetOrOverrideRebootAttempts1(this._rebootAttempts1);
            clone.SetOrOverrideRebootAttempts2(this._rebootAttempts2);
            clone.SetOrOverrideTotalReboots1(this._totalReboots1);
            clone.SetOrOverrideTotalReboots2(this._totalReboots2);
            clone.SetOrOverrideInput1(this._input1);
            clone.SetOrOverrideInput2(this._input2);
            clone.SetOrOverrideTempUnits(this._units);
            clone.SetOrOverrideSensor1(this._sensor1);
            clone.SetOrOverrideSensor2(this._sensor2);
            clone.SetOrOverrideSensor3(this._sensor3);
            clone.SetOrOverrideExtVar0(this._extVar0);
            clone.SetOrOverrideExtVar1(this._extVar1);
            clone.SetOrOverrideExtVar2(this._extVar2);
            clone.SetOrOverrideExtVar3(this._extVar3);
            clone.SetOrOverrideExtVar4(this._extVar4);
            clone.SetOrOverrideSerial(this._serial);
            clone.SetOrOverrideTime(this._time);
            return clone;
        }
		#endregion
	}
}

