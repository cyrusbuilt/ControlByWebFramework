using CyrusBuilt.ControlByWeb.Relays;
using System;
using System.Net.NetworkInformation;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// The state of a ControlByWeb X-300 module when in thermostat mode.
    /// </summary>
    public class X300ThermostatState : DeviceStateBase
    {
        #region Fields
        private TemperatureUnits _unit = TemperatureUnits.Fahrenheit;
        private Double _indoorTemp = X300Constants.MIN_TEMP_ABSOLUTE;
        private Double _outdoorTemp = X300Constants.MIN_TEMP_ABSOLUTE;
        private Double _setTemp = X300Constants.MIN_TEMP_ABSOLUTE;
        private RelayState _heat = RelayState.Off;
        private RelayState _cool = RelayState.Off;
        private RelayState _fan = RelayState.Off;
        private Double _minTemp = 00.0;
        private Double _maxTemp = 00.0;
        private Double _minTempYesterday = 00.0;
        private Double _maxTempYesterday = 00.0;
        private HeatMode _heatMode = HeatMode.Off;
        private FanMode _fanMode = FanMode.Auto;
        private Int32 _filterChange = X300Constants.DEFAULT_FILTER_CHANGE_DAYS;
        private Double _minSetTemp = 00.0;
        private Double _maxSetTemp = 00.0;
        private readonly X300OperationMode _mode = X300OperationMode.Thermostat;
        private Epoch _epochTime = null;
        private PhysicalAddress _serial = null;
        private Boolean _holding = false;
        private Boolean _reqFiltRst = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300ThermostatState</b>
        /// class. This is the default constructor.
        /// </summary>
        public X300ThermostatState()
            : base() {
            this._epochTime = new Epoch();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unit of measure being used for temperature.
        /// </summary>
        public TemperatureUnits Units {
            get { return this._unit; }
        }

        /// <summary>
        /// Gets the indoor temperature.
        /// </summary>
        public Double IndoorTemperature {
            get { return this._indoorTemp; }
        }

        /// <summary>
        /// Gets the outdoor temperature.
        /// </summary>
        public Double OutdoorTemperature {
            get { return this._outdoorTemp; }
        }

        /// <summary>
        /// Gets or sets the temperature to regulate to. When setting this value,
        /// the valid range must be with the <see cref="MinSetTemperature"/> and
        /// <see cref="MaxSetTemperature"/> values.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified value is not within <see cref="MinSetTemperature"/>
        /// and <see cref="MaxSetTemperature"/>.
        /// </exception>
        public Double SetTemperature {
            get { return this._setTemp; }
            set {
                if (value >= X300Constants.MIN_TEMP_ABSOLUTE) {
                    if ((value < this._minSetTemp) || (value > this._maxTemp)) {
                        String err = String.Format("Must be within {0} and {1}.",
                                                    this._minSetTemp.ToString(),
                                                    this._maxSetTemp.ToString());
                        throw new ArgumentOutOfRangeException("SetTemperature", err);
                    }
                    this._setTemp = value;
                }
            }
        }

        /// <summary>
        /// Gets the current state of the heat relay.
        /// </summary>
        public RelayState Heat {
            get { return this._heat; }
        }

        /// <summary>
        /// Gets the current state of the cool relay.
        /// </summary>
        public RelayState Cool {
            get { return this._cool; }
        }

        /// <summary>
        /// Gets the current state of the fan relay.
        /// </summary>
        public RelayState Fan {
            get { return this._fan; }
        }

        /// <summary>
        /// Gets the minimum temperature reading for the last 24 hours.
        /// </summary>
        public Double Min24HourTempReading {
            get { return this._minTemp; }
        }

        /// <summary>
        /// Gets the maximum temperature reading for the last 24 hours.
        /// </summary>
        public Double Max24HourTempReading {
            get { return this._maxTemp; }
        }

        /// <summary>
        /// Gets the minimum temperature reading for yesterday.
        /// </summary>
        public Double MinYesterdayTempReading {
            get { return this._minTempYesterday; }
        }

        /// <summary>
        /// Gets the maximum temperature reading for yesterday.
        /// </summary>
        public Double MaxYesterdayTempReading {
            get { return this._maxTempYesterday; }
        }

        /// <summary>
        /// Gets or sets the heat mode (temperature regulation mode).
        /// </summary>
        public HeatMode HeatMode {
            get { return this._heatMode; }
            set { this._heatMode = value; }
        }

        /// <summary>
        /// Gets or sets the fan mode.
        /// </summary>
        public FanMode FanMode {
            get { return this._fanMode; }
            set { this._fanMode = value; }
        }

        /// <summary>
        /// Gets the number of days until the filter should be changed.
        /// </summary>
        public Int32 FilterChangeInDays {
            get { return this._filterChange; }
        }

        /// <summary>
        /// Gets the minimum allowable temperature that can be set.
        /// </summary>
        public Double MinSetTemperature {
            get { return this._minSetTemp; }
        }

        /// <summary>
        /// Gets the maximum allowable temperature that can be set.
        /// </summary>
        public Double MaxSetTemperature {
            get { return this._maxSetTemp; }
        }

        /// <summary>
        /// Gets the current operational mode of the device.
        /// </summary>
        public X300OperationMode Mode {
            get { return this._mode; }
        }

        /// <summary>
        /// Gets the time displayed in "epoch time" (Unix time - number of
        /// seconds since January 1, 1970).
        /// </summary>
        public Epoch Time {
            get { return this._epochTime; }
        }

        /// <summary>
        /// Gets the serial number (MAC address) of the device.
        /// </summary>
        public PhysicalAddress Serial {
            get { return this._serial; }
        }

        /// <summary>
        /// Gets whether or not the device is currently holding the set
        /// temperature. This is used by <see cref="X300ModuleController"/>
        /// to determine if it should hold the set temp when setting
        /// the state of the device.
        /// </summary>
        public Boolean Holding {
            get { return this._holding; }
        }

        /// <summary>
        /// Gets whether or not a filter counter reset was requested.
        /// </summary>
        public Boolean FilterResetRequested {
            get { return this._reqFiltRst; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicitly sets the unit of measure to be used for measuring
        /// temperature.
        /// </summary>
        /// <param name="units">
        /// The unit of measure to use for temperature (Celcius or Fahrenheit).
        /// </param>
        public void SetTemperatureUnits(TemperatureUnits units) {
            this._unit = units;
        }

        /// <summary>
        /// Explicitly sets or overrides the indoor temperature value.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetIndoorTemperature(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._indoorTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the outdoor temperature value.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetOutdoorTemperature(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._outdoorTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the state of the heat relay.
        /// </summary>
        /// <param name="state">
        /// The relay state to set.
        /// </param>
        public void SetHeat(RelayState state) {
            this._heat = state;
        }

        /// <summary>
        /// Explicitly sets or overrides the state of the cool relay.
        /// </summary>
        /// <param name="state">
        /// The relay state to set.
        /// </param>
        public void SetCool(RelayState state) {
            this._cool = state;
        }

        /// <summary>
        /// Explicitly sets or overrides the state of the fan relay.
        /// </summary>
        /// <param name="state">
        /// The relay state to set.
        /// </param>
        public void SetFan(RelayState state) {
            this._fan = state;
        }

        /// <summary>
        /// Explicitly sets or overrides the minimum temperature reading for
        /// the last 24 hours.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMin24HourTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._minTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the maximum temperature reading for
        /// the last 24 hours.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMax24HourTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._maxTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the minimum temperature reading for
        /// yesterday.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMinYesterdayTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._minTempYesterday = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the maximum temperature reading for
        /// yesterday.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMaxYesterdayTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._maxTempYesterday = temp;
            }
        }

        /// <summary>
        /// Explicityly sets or overrides the number of days until the filter
        /// should be changed.
        /// </summary>
        /// <param name="days">
        /// The number of days until the filter should be changed.
        /// </param>
        public void SetFilterChangeDays(Int32 days) {
            if (days >= 0) {
                this._filterChange = days;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the minimum allowable temperature to
        /// set.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMinimumSetTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._minSetTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the maximum allowable temperature to
        /// set.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set.
        /// </param>
        public void SetMaximumSetTemp(Double temp) {
            if (temp >= X300Constants.MIN_TEMP_ABSOLUTE) {
                this._maxSetTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the Epoch (Unix) time value.
        /// </summary>
        /// <param name="time">
        /// The Epoch (Unix) time.
        /// </param>
        public void SetTime(Epoch time) {
            if (time == null) {
                this._epochTime = new Epoch();
                return;
            }
            this._epochTime = time;
        }

        /// <summary>
        /// Explicitly sets or overrides the Epoch (Unix) time value.
        /// </summary>
        /// <param name="seconds">
        /// The number of seconds elapsed since January 1, 1970 (epoch).
        /// </param>
        public void SetTime(Int64 seconds) {
            if (seconds >= 0) {
                this._epochTime = new Epoch(seconds);
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the device's serial number.
        /// </summary>
        /// <param name="serial">
        /// The serial number (MAC address) to set.
        /// </param>
        public void SetSerial(PhysicalAddress serial) {
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
        public void SetSerial(String serial) {
            try {
                this.SetSerial(PhysicalAddress.Parse(serial));
            }
            catch (FormatException) {
                throw;
            }
        }

        /// <summary>
        /// Hold the set temperature.
        /// </summary>
        public void HoldTemp() {
            this._holding = true;
        }

        /// <summary>
        /// Release the hold on the set temperature.
        /// </summary>
        public void ReleaseHold() {
            this._holding = false;
        }

        /// <summary>
        /// Request a filter counter reset.
        /// </summary>
        /// <param name="requesting">
        /// true to request a filter counter reset, false to cancel the request.
        /// </param>
        public void RequestFilterReset(Boolean requesting) {
            this._reqFiltRst = requesting;
        }

        /// <summary>
        /// Creates a new X300ThermostatState that is a deep copy of the
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new X300ThermostatState that is a copy of this instance.
        /// </returns>
        public X300ThermostatState Clone() {
            X300ThermostatState clone = new X300ThermostatState();
            clone.SetTemperature = this.SetTemperature;
            clone.HeatMode = this.HeatMode;
            clone.FanMode = this.FanMode;
            clone.SetCool(this.Cool);
            clone.SetFan(this.Fan);
            clone.SetHeat(this.Heat);
            clone.SetTemperatureUnits(this.Units);
            clone.SetFilterChangeDays(this.FilterChangeInDays);
            clone.SetIndoorTemperature(this.IndoorTemperature);
            clone.SetMax24HourTemp(this.Max24HourTempReading);
            clone.SetMaximumSetTemp(this.MaxSetTemperature);
            clone.SetMaxYesterdayTemp(this.MaxYesterdayTempReading);
            clone.SetMin24HourTemp(this.Min24HourTempReading);
            clone.SetMinimumSetTemp(this.MinSetTemperature);
            clone.SetMinYesterdayTemp(this.MinYesterdayTempReading);
            clone.SetOutdoorTemperature(this.OutdoorTemperature);
            clone.SetTime(this.Time);
            clone.SetSerial(this.Serial);
            if (this.Holding) {
                clone.HoldTemp();
            }
            clone.RequestFilterReset(this.FilterResetRequested);
            return clone;
        }

        /// <summary>
        /// Resets the state back to its default values.
        /// </summary>
        public override void Reset() {
            this._unit = TemperatureUnits.Fahrenheit;
            this._indoorTemp = X300Constants.MIN_TEMP_ABSOLUTE;
            this._outdoorTemp = X300Constants.MIN_TEMP_ABSOLUTE;
            this._setTemp = X300Constants.MIN_TEMP_ABSOLUTE;
            this._heat = RelayState.Off;
            this._cool = RelayState.Off;
            this._fan = RelayState.Off;
            this._minTemp = 00.0;
            this._maxTemp = 00.0;
            this._minTempYesterday = 00.0;
            this._maxTempYesterday = 00.0;
            this._heatMode = HeatMode.Off;
            this._fanMode = FanMode.Auto;
            this._filterChange = X300Constants.DEFAULT_FILTER_CHANGE_DAYS;
            this._minSetTemp = 00.0;
            this._maxSetTemp = 00.0;
            this._epochTime = new Epoch();
            this._serial = null;
            this._holding = false;
            this._reqFiltRst = false;
        }
        #endregion
    }
}
