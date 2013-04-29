using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using System;
using System.Net.NetworkInformation;

namespace CyrusBuilt.ControlByWeb.Daq.X320Module
{
    /// <summary>
    /// Represents the state of an X-320 module.
    /// </summary>
    public class X320State : DeviceStateBase
    {
        // TODO The X-320 has 2 digital I/Os. These can either be an input
        // (StandardInput) or an output (Relay) depending on the current mode.
        // May need to define a DigitalIO class and IOMode enum and replace
        // the current implementation of _io1 and _io2.  Not sure how we can
        // determine if the current mode is input or output though. It can be
        // set in the web interface but I see nothing about reading this value
        // from XML (its not in eventX.xml or diagnostics.xml either).

        // UPDATE: Sent e-mail to tech support at Xytronix. They confirm that
        // there is indeed no way to determine what mode the digital I/Os are
        // by reading the state.xml page. They intend to release a firmware
        // update that includes additional tags in the XML response to indicate
        // the modes of the two digital I/Os and the analog I/Os. Waiting for
        // firmware update and accompanying documentation to proceed further.

        // UPDATE: As of my last check on 7/5/2012, the X-320 still does not
        // contain a flag in the state.xml file indicating whether the inputs
        // are configured as output in analog mode or as inputs in digital mode.
        // Additionally the manual makes no mention of this either, and the
        // current parameters in state.xml are ambiguous at best. As such,
        // this class must assume the device is configured in output mode
        // and the I/O is analog. This means that <io1state> and <io2state>
        // (Digital I/Os 1 and 2, respectively) are assumed to be treated
        // as relays.  That being said, this class IS NOT COMPATIBLE with
        // an X-320 configured with its I/Os configured as digital inputs.

        #region Fields
        private Relay _relay1 = null;
        private Relay _relay2 = null;
        private Double _highTime1 = 0.00;
        private Double _highTime2 = 0.00;
        private Int32 _rawCount1 = 0;
        private Int32 _rawCount2 = 0;
        private Int32 _multiplier = 1;
        private Int32 _offset = 0;
        private Double _rawFrequency = 0.000;
        private Double _frequency = 0.00;
        private AlarmConditions _freqAlarm = AlarmConditions.Normal;
        private AnalogInput _anInput1 = null;
        private AnalogInput _anInput2 = null;
        private AnalogInput _anInput3 = null;
        private AnalogInput _anInput4 = null;
        private AlarmConditions _anAlarm1 = AlarmConditions.Normal;
        private AlarmConditions _anAlarm2 = AlarmConditions.Normal;
        private AlarmConditions _anAlarm3 = AlarmConditions.Normal;
        private AlarmConditions _anAlarm4 = AlarmConditions.Normal;
        private TemperatureUnits _units = TemperatureUnits.Fahrenheit;
        private SensorInput _sensor1 = null;
        private SensorInput _sensor2 = null;
        private SensorInput _sensor3 = null;
        private SensorInput _sensor4 = null;
        private SensorInput _sensor5 = null;
        private SensorInput _sensor6 = null;
        private AlarmConditions _sensAlarm1 = AlarmConditions.Normal;
        private AlarmConditions _sensAlarm2 = AlarmConditions.Normal;
        private AlarmConditions _sensAlarm3 = AlarmConditions.Normal;
        private AlarmConditions _sensAlarm4 = AlarmConditions.Normal;
        private AlarmConditions _sensAlarm5 = AlarmConditions.Normal;
        private AlarmConditions _sensAlarm6 = AlarmConditions.Normal;
        private Double _extVar0 = 0.0;
        private Double _extVar1 = 0.0;
        private Double _extVar2 = 0.0;
        private Double _extVar3 = 0.0;
        private PhysicalAddress _serial = null;
        private Epoch _time = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320Module.X320State</b>
        /// class. This is the default constructor.
        /// </summary>
        public X320State()
            : base () {
            // Init relays.
            this._relay1 = new Relay();
            this._relay2 = new Relay();

            // Init analog inputs.
            this._anInput1 = new AnalogInput();
            this._anInput2 = new AnalogInput();
            this._anInput3 = new AnalogInput();
            this._anInput4 = new AnalogInput();

            // Init sensor inputs.
            this._sensor1 = new SensorInput();
            this._sensor2 = new SensorInput();
            this._sensor3 = new SensorInput();
            this._sensor4 = new SensorInput();
            this._sensor5 = new SensorInput();
            this._sensor6 = new SensorInput();

            // Init epoch (Unix time).
            this._time = new Epoch();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first relay.
        /// </summary>
        public Relay Relay1 {
            get { return this._relay1; }
            set {
                if (value == null) {
                    this._relay1 = new Relay();
                    return;
                }
                this._relay1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second relay.
        /// </summary>
        public Relay Relay2 {
            get { return this._relay2; }
            set {
                if (value == null) {
                    this._relay2 = new Relay();
                    return;
                }
                this._relay2 = value;
            }
        }

        /// <summary>
        /// Gets the duration (in seconds) that the first relay was last held
        /// in the ON state.
        /// </summary>
        public Double HighTime1 {
            get { return this._highTime1; }
        }

        /// <summary>
        /// Gets the duration (in seconds) that the second relay was last held
        /// in the OFF state.
        /// </summary>
        public Double HightTime2 {
            get { return this._highTime2; }
        }

        /// <summary>
        /// The number of times relay one transitioned from OFF to ON.
        /// </summary>
        public Int32 TransitionCount1 {
            get { return this._rawCount1; }
        }

        /// <summary>
        /// The number of times relay two transitioned from OFF to ON.
        /// </summary>
        public Int32 TransitionCount2 {
            get { return this._rawCount2; }
        }

        /// <summary>
        /// Gets or sets the multiplier (slope).
        /// </summary>
        public Int32 Multiplier {
            get { return this._multiplier; }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException("X320State.Multiplier", "Value cannot be less than 1.");
                }
                this._multiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public Int32 Offset {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// Gets the scaled transition count value for relay one. This is
        /// computed as <see cref="TransitionCount1"/> * <see cref="Multiplier"/> + <see cref="Offset"/>.
        /// </summary>
        public Double ScaledCount1 {
            get { return (this._rawCount1 * this._multiplier + this._offset); }
        }

        /// <summary>
        /// Gets the scaled transition count value for relay two. This is
        /// computed as <see cref="TransitionCount2"/> * <see cref="Multiplier"/> + <see cref="Offset"/>.
        /// </summary>
        public Double ScaledCount2 {
            get { return (this._rawCount2 * this._multiplier + this._offset); }
        }

        /// <summary>
        /// Gets the raw signal frequency from the frequency input.
        /// </summary>
        public Double RawFrequency {
            get { return this._rawFrequency; }
        }

        /// <summary>
        /// Gets the scaled frequency reading from the frequency input.
        /// </summary>
        public Double ScaledFrequency {
            get { return this._frequency; }
        }

        /// <summary>
        /// Gets the current condition of the frequency alarm.
        /// </summary>
        public AlarmConditions FrequencyAlarm {
            get { return this._freqAlarm; }
        }

        /// <summary>
        /// Gets the first analog input.
        /// </summary>
        public AnalogInput Input1 {
            get { return this._anInput1; }
        }

        /// <summary>
        /// Gets the second analog input.
        /// </summary>
        public AnalogInput Input2 {
            get { return this._anInput2; }
        }

        /// <summary>
        /// Gets the third analog input.
        /// </summary>
        public AnalogInput Input3 {
            get { return this._anInput3; }
        }

        /// <summary>
        /// Gets the fourth analog input.
        /// </summary>
        public AnalogInput Input4 {
            get { return this._anInput4; }
        }

        /// <summary>
        /// Gets the alarm condition for the first analog input.
        /// </summary>
        public AlarmConditions InputAlarm1 {
            get { return this._anAlarm1; }
        }

        /// <summary>
        /// Gets the alarm condition for the second analog input.
        /// </summary>
        public AlarmConditions InputAlarm2 {
            get { return this._anAlarm2; }
        }

        /// <summary>
        /// Gets the alarm condition for the third analog input.
        /// </summary>
        public AlarmConditions InputAlarm3 {
            get { return this._anAlarm3; }
        }

        /// <summary>
        /// Gets the alarm condition for the fourth analog input.
        /// </summary>
        public AlarmConditions InputAlarm4 {
            get { return this._anAlarm4; }
        }

        /// <summary>
        /// Gets the unit of measure for temperature.
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

        /// <summary>
        /// Gets the fourth sensor input.
        /// </summary>
        public SensorInput Sensor4 {
            get { return this._sensor4; }
        }

        /// <summary>
        /// Gets the fifth sensor input.
        /// </summary>
        public SensorInput Sensor5 {
            get { return this._sensor5; }
        }

        /// <summary>
        /// Gets the sixth sensor input.
        /// </summary>
        public SensorInput Sensor6 {
            get { return this._sensor6; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor one.
        /// </summary>
        public AlarmConditions SensorAlarm1 {
            get { return this._sensAlarm1; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor two.
        /// </summary>
        public AlarmConditions SensorAlarm2 {
            get { return this._sensAlarm2; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor three.
        /// </summary>
        public AlarmConditions SensorAlarm3 {
            get { return this._sensAlarm3; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor four.
        /// </summary>
        public AlarmConditions SensorAlarm4 {
            get { return this._sensAlarm4; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor five.
        /// </summary>
        public AlarmConditions SensorAlarm5 {
            get { return this._sensAlarm5; }
        }

        /// <summary>
        /// Gets the alarm condition for sensor six.
        /// </summary>
        public AlarmConditions SensorAlarm6 {
            get { return this._sensAlarm6; }
        }

        /// <summary>
        /// Gets the value of the external variable at ID: 0.
        /// </summary>
        public Double ExternalVariable0 {
            get { return this._extVar0; }
        }

        /// <summary>
        /// Gets the value of the external variable at ID: 1.
        /// </summary>
        public Double ExternalVariable1 {
            get { return this._extVar1; }
        }

        /// <summary>
        /// Gets the value of the external variable at ID: 2.
        /// </summary>
        public Double ExternalVariable2 {
            get { return this._extVar2; }
        }

        /// <summary>
        /// Gets the value of the external variable at ID: 3.
        /// </summary>
        public Double ExternalVariable3 {
            get { return this._extVar3; }
        }

        /// <summary>
        /// Gets the serial number (physical address) of the device.
        /// </summary>
        public PhysicalAddress Serial {
            get { return this._serial; }
        }

        /// <summary>
        /// Gets the Epoch time as reported by the device.
        /// </summary>
        public Epoch Time {
            get { return this._time; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets an analog input by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the input to get (valid IDs are 1 - 4).
        /// </param>
        /// <returns>
        /// If successful, the analog input at the specified ID; Otherwise, null.
        /// </returns>
        public AnalogInput GetAnalogInput(Int32 id) {
            AnalogInput ai = null;
            switch (id) {
                case 1: ai = this._anInput1; break;
                case 2: ai = this._anInput2; break;
                case 3: ai = this._anInput3; break;
                case 4: ai = this._anInput4; break;
            }
            return ai;
        }

        /// <summary>
        /// Assigns an analog input to the input at the specified ID.
        /// </summary>
        /// <param name="id">
        /// The ID to assign an input to. (Valid IDs are 1 - 4)
        /// </param>
        /// <param name="input">
        /// The input to assign to the specified ID.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 4).
        /// </exception>
        public void AssignAnalogInput(Int32 id, AnalogInput input) {
            if ((id < 1) || (id > 4)) {
                throw new ArgumentOutOfRangeException("id", "Value must be greater than 0 and less than 5.");
            }

            // We don't assing null values. The user expects to access the
            // object when getting the property value.
            if (input == null) {
                input = new AnalogInput();
            }

            switch (id) {
                case 1: this._anInput1 = input; break;
                case 2: this._anInput2 = input; break;
                case 3: this._anInput3 = input; break;
                case 4: this._anInput4 = input; break;
            }
        }

        /// <summary>
        /// Gets the alarm condition for the specified analog input.
        /// </summary>
        /// <param name="id">
        /// The ID of the analog input to get the alarm condition of.
        /// </param>
        /// <returns>
        /// The alarm condition for the specified analog input.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 4).
        /// </exception>
        public AlarmConditions GetAnalogInputAlarmCondition(Int32 id) {
            if ((id < 1) || (id > 4)) {
                throw new ArgumentOutOfRangeException("id", "Value must be greater than 0 and less than 5.");
            }

            AlarmConditions ac = AlarmConditions.Normal;
            switch (id) {
                case 1: ac = this._anAlarm1; break;
                case 2: ac = this._anAlarm2; break;
                case 3: ac = this._anAlarm3; break;
                case 4: ac = this._anAlarm4; break;
            }
            return ac;
        }

        /// <summary>
        /// Sets an alarm condition for the specified analog input.
        /// </summary>
        /// <param name="id">
        /// The ID of the analog input to set the alarm condition for.
        /// </param>
        /// <param name="condition">
        /// The alarm condition to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 4).
        /// </exception>
        public void AssignAnalogInputAlarmCondition(Int32 id, AlarmConditions condition) {
            if ((id < 1) || (id > 4)) {
                throw new ArgumentOutOfRangeException("id", "Value must be greater than 0 and less than 5.");
            }

            switch (id) {
                case 1: this._anAlarm1 = condition; break;
                case 2: this._anAlarm2 = condition; break;
                case 3: this._anAlarm3 = condition; break;
                case 4: this._anAlarm4 = condition; break;
            }
        }

        /// <summary>
        /// Gets a sensor input by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the sensor input to get (valid IDs are 1 - 6).
        /// </param>
        /// <returns>
        /// If successful, the sensor input at the specified ID; Otherwise, null.
        /// </returns>
        public SensorInput GetSensorInput(Int32 id) {
            SensorInput si = null;
            switch (id) {
                case 1: si = this._sensor1; break;
                case 2: si = this._sensor2; break;
                case 3: si = this._sensor3; break;
                case 4: si = this._sensor4; break;
                case 5: si = this._sensor5; break;
                case 6: si = this._sensor6; break;
            }
            return si;
        }

        /// <summary>
        /// Assigns a sensor input to the specified input ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the sensor input to assign.
        /// </param>
        /// <param name="sensor">
        /// The sensor to assign.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 6).
        /// </exception>
        public void AssignSensorInput(Int32 id, SensorInput sensor) {
            if ((id < 1) || (id > 6)) {
                throw new ArgumentOutOfRangeException("id", "Must be a valid sensor ID (1 - 6).");
            }

            // Prevent null sensors.
            if (sensor == null) {
                sensor = new SensorInput();
            }

            switch (id) {
                case 1: this._sensor1 = sensor; break;
                case 2: this._sensor2 = sensor; break;
                case 3: this._sensor3 = sensor; break;
                case 4: this._sensor4 = sensor; break;
                case 5: this._sensor5 = sensor; break;
                case 6: this._sensor6 = sensor; break;
            }
        }

        /// <summary>
        /// Gets the alarm condition for the specified sensor input.
        /// </summary>
        /// <param name="id">
        /// The ID of the sensor input to get the alarm condition of.
        /// </param>
        /// <returns>
        /// The alarm condition for the specified sensor.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 6).
        /// </exception>
        public AlarmConditions GetSensorAlarmCondition(Int32 id) {
            if ((id < 1) || (id > 6)) {
                throw new ArgumentOutOfRangeException("id", "Value must be greater than 0 and less than 7.");
            }

            AlarmConditions ac = AlarmConditions.Normal;
            switch (id) {
                case 1: ac = this._sensAlarm1; break;
                case 2: ac = this._sensAlarm2; break;
                case 3: ac = this._sensAlarm3; break;
                case 4: ac = this._sensAlarm4; break;
                case 5: ac = this._sensAlarm5; break;
                case 6: ac = this._sensAlarm6; break;
            }
            return ac;
        }

        /// <summary>
        /// Assigns an alarm condition to the specified sensor input.
        /// </summary>
        /// <param name="id">
        /// The ID of the sensor input to assign the condition to.
        /// </param>
        /// <param name="condition">
        /// The alarm condition to assign to the specified sensor.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (1 - 6).
        /// </exception>
        public void AssignSensorAlarmCondtion(Int32 id, AlarmConditions condition) {
            if ((id < 1) || (id > 6)) {
                throw new ArgumentOutOfRangeException("id", "Must be a valid sensor ID (1 - 6).");
            }

            switch (id) {
                case 1: this._sensAlarm1 = condition; break;
                case 2: this._sensAlarm2 = condition; break;
                case 3: this._sensAlarm3 = condition; break;
                case 4: this._sensAlarm4 = condition; break;
                case 5: this._sensAlarm5 = condition; break;
                case 6: this._sensAlarm6 = condition; break;
            }
        }

        /// <summary>
        /// Gets the value of an external variable by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the external variable to get the value of (valid IDs are
        /// 0 - 3).
        /// </param>
        /// <returns>
        /// If successful, the value of the external variable;
        /// Otherwise, -1.
        /// </returns>
        public Double GetExternalVariableValue(Int32 id) {
            Double ev = -1;
            switch (id) {
                case 0: ev = this._extVar0; break;
                case 1: ev = this._extVar1; break;
                case 2: ev = this._extVar2; break;
                case 3: ev = this._extVar3; break;
            }
            return ev;
        }

        /// <summary>
        /// Sets the value of the specified external variable.
        /// </summary>
        /// <param name="id">
        /// The ID of the external variable to set.
        /// </param>
        /// <param name="value">
        /// The value to assign to the external variable.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="id"/> must be a valid ID (0 - 3).
        /// </exception>
        public void SetExternalVariableValue(Int32 id, Double value) {
            if ((id < 0) || (id > 3)) {
                throw new ArgumentOutOfRangeException("id", "Must be a valid ext var ID (0 - 3).");
            }

            switch (id) {
                case 0: this._extVar0 = value; break;
                case 1: this._extVar1 = value; break;
                case 2: this._extVar2 = value; break;
                case 3: this._extVar3 = value; break;
            }
        }

        /// <summary>
        /// Changes the duration (in seconds) that the specified relay was last
        /// held in the ON state.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to change.
        /// </param>
        /// <param name="highTime">
        /// The amount of time (in seconds) to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relayNum"/> must be either 1 or 2.
        /// </exception>
        public void ChangeRelayHighTime(Int32 relayNum, Double highTime) {
            if ((relayNum < 1) || (relayNum > 2)) {
                throw new ArgumentOutOfRangeException("id", "Must be a valid ID (1 or 2).");
            }

            switch (relayNum) {
                case 1: this._highTime1 = highTime; break;
                case 2: this._highTime2 = highTime; break;
            }
        }

        /// <summary>
        /// Changes the number of times the specified relay transition states
        /// from OFF to ON.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number to change.
        /// </param>
        /// <param name="count">
        /// The number of times the transition occurred.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relayNum"/> must be either 1 or 2.
        /// </exception>
        public void ChangeRelayTransitionCount(Int32 relayNum, Int32 count) {
            if ((relayNum < 1) || (relayNum > 2)) {
                throw new ArgumentOutOfRangeException("id", "Must be a valid ID (1 or 2).");
            }
            
            switch (relayNum) {
                case 1: this._rawCount1 = count; break;
                case 2: this._rawCount2 = count; break;
            }
        }

        /// <summary>
        /// Sets the raw signal frequency on the frequency input.
        /// </summary>
        /// <param name="frequency">
        /// The frequency value to set.
        /// </param>
        public void ChangeRawFrequency(Double frequency) {
            this._rawFrequency = frequency;
        }

        /// <summary>
        /// Sets the scaled frequency reading on the frequency input.
        /// </summary>
        /// <param name="frequency">
        /// The frequency value to set.
        /// </param>
        public void ChangeScaledFrequency(Double frequency) {
            this._frequency = frequency;
        }

        /// <summary>
        /// Sets an alarm condtion on the frequency input.
        /// </summary>
        /// <param name="condition">
        /// The alarm condition to set.
        /// </param>
        public void SetFrequencyAlarmCondition(AlarmConditions condition) {
            this._freqAlarm = condition;
        }

        /// <summary>
        /// Changes the unit of measurement for temperature readings.
        /// </summary>
        /// <param name="units">
        /// The unit of measure to use.
        /// </param>
        public void ChangeTemperatureUnitOfMeasure(TemperatureUnits units) {
            this._units = units;
        }

        /// <summary>
        /// Changes the serial number (MAC address) of the device.
        /// </summary>
        /// <param name="serial">
        /// The serial number to set.
        /// </param>
        public void ChangeSerial(PhysicalAddress serial) {
            this._serial = serial;
        }

        /// <summary>
        /// Changes the serial number (MAC address) of the device.
        /// </summary>
        /// <param name="serial">
        /// The string containing the serial to set.
        /// </param>
        /// <exception cref="FormatException">
        /// <paramref name="serial"/> could not be parsed into a valid
        /// <see cref="PhysicalAddress"/> (serial).
        /// </exception>
        public void ChangeSerial(String serial) {
            try {
                this.ChangeSerial(PhysicalAddress.Parse(serial));
            }
            catch (FormatException) {
                throw;
            }
        }

        /// <summary>
        /// Changes the device time.
        /// </summary>
        /// <param name="time">
        /// A value in Epoch (UNIX) time.
        /// </param>
        public void SetTime(Epoch time) {
            // We don't allow null time.
            if (time == null) {
                time = new Epoch();
            }
            this._time = time;
        }

        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public X320State Clone() {
            X320State state = new X320State();
            for (Int32 i = 1; i <= X320Constants.TOTAL_RELAYS; i++) {
                state.AssignAnalogInput(i, this.GetAnalogInput(i));
                state.AssignAnalogInputAlarmCondition(i, this.GetAnalogInputAlarmCondition(i));
            }

            for (Int32 s = 1; s <= X320Constants.TOTAL_SENSOR_INPUTS; s++) {
                state.AssignSensorInput(s, this.GetSensorInput(s));
                state.AssignSensorAlarmCondtion(s, this.GetSensorAlarmCondition(s));
            }

            for (Int32 ev = 0; ev <= (X320Constants.TOTAL_EXT_VARS - 1); ev++) {
                state.SetExternalVariableValue(ev, this.GetExternalVariableValue(ev));
            }

            state.Relay1 = this._relay1;
            state.Relay2 = this._relay2;
            state.Multiplier = this._multiplier;
            state.Offset = this._offset;
            state.SetFrequencyAlarmCondition(this._freqAlarm);
            state.SetTime(this._time);
            state.ChangeRawFrequency(this._rawFrequency);
            
            Double dVal = 0.0;
            Int32 iVal = 0;
            for (Int32 r = 1; r <= X320Constants.TOTAL_RELAYS; r++) {
                if (r == 1) {
                    dVal = this._highTime1;
                    iVal = this._rawCount1;
                }
                else {
                    dVal = this._highTime2;
                    iVal = this._rawCount2;
                }

                state.ChangeRelayHighTime(r, dVal);
                state.ChangeRelayTransitionCount(r, iVal);
            }

            this.ChangeSerial(this._serial);
            this.ChangeTemperatureUnitOfMeasure(this._units);
            return state;
        }

        /// <summary>
        /// Resets the state back to its original values.
        /// </summary>
        public override void Reset() {
            this._relay1.Reset();
            this._relay2.Reset();
            this._highTime1 = 0.00;
            this._highTime2 = 0.00;
            this._rawCount1 = 0;
            this._rawCount2 = 0;
            this._multiplier = 1;
            this._offset = 0;
            this._rawFrequency = 0.000;
            this._frequency = 0.000;
            this._freqAlarm = AlarmConditions.Normal;
            this._anInput1.Reset();
            this._anInput2.Reset();
            this._anInput3.Reset();
            this._anInput4.Reset();
            this._anAlarm1 = AlarmConditions.Normal;
            this._anAlarm2 = AlarmConditions.Normal;
            this._anAlarm3 = AlarmConditions.Normal;
            this._anAlarm4 = AlarmConditions.Normal;
            this._units = TemperatureUnits.Fahrenheit;
            this._sensor1.Reset();
            this._sensor2.Reset();
            this._sensor3.Reset();
            this._sensor4.Reset();
            this._sensor5.Reset();
            this._sensor6.Reset();
            this._sensAlarm1 = AlarmConditions.Normal;
            this._sensAlarm2 = AlarmConditions.Normal;
            this._sensAlarm3 = AlarmConditions.Normal;
            this._sensAlarm4 = AlarmConditions.Normal;
            this._sensAlarm5 = AlarmConditions.Normal;
            this._sensAlarm6 = AlarmConditions.Normal;
            this._extVar0 = 0.0;
            this._extVar1 = 0.0;
            this._extVar2 = 0.0;
            this._extVar3 = 0.0;
            this._time = new Epoch();
            // We don't reset the serial, since this is tied to the device.
        }
        #endregion
    }
}
