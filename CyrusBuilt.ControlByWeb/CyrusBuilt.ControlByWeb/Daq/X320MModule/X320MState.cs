using System;
using System.Net.NetworkInformation;
using CyrusBuilt.ControlByWeb.Inputs;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Represents the state of an X-320M module.
    /// </summary>
    public class X320MState : DeviceStateBase
    {
        #region Fields
        private Double _windSpeed = 0.0;
        private Double _windDirection = 0.0;
        private Double _rainTotal = 0.0;
        private SensorInput _tempSensor = null;
        private Double _humidity = 0.0;
        private Double _solarRadiation = 0.0;
        private Double _barometricPressure = 0.0;
        private Double _aux1 = 0.0;
        private Double _aux2 = 0.0;
        private Double _rainTotalOneHour = 0.0;
        private Double _rainTotalToday = 0.0;
        private Double _rainTotalSevenDays = 0.0;
        private Epoch _rainTotalResetTime = null;
        private AlarmConditions _rainAlarm = AlarmConditions.Normal;
        private Double _tempHigh = 0.0;
        private Double _tempLow = 0.0;
        private Double _tempHighYesterday = 0.0;
        private Double _tempLowYesterday = 0.0;
        private Double _heatIndex = 0.0;
        private Double _windChill = 0.0;
        private Double _dewPoint = 0.0;
        private AlarmConditions _tempAlarm = AlarmConditions.Normal;
        private Double _humidityHigh = 0.0;
        private Double _humidityLow = 0.0;
        private Double _humidityHighYesterday = 0.0;
        private Double _humidityLowYesterday = 0.0;
        private AlarmConditions _humidityAlarm = AlarmConditions.Normal;
        private Double _baromPresLastHour = 0.0;
        private Double _baromPresLastThreeHours = 0.0;
        private Double _baromPresLastSixHours = 0.0;
        private Double _baromPresLastNineHours = 0.0;
        private Double _baromPresLastTwelveHours = 0.0;
        private Double _baromPresLastFifteenHours = 0.0;
        private Double _baromPresLastTwentyFourHours = 0.0;
        private AlarmConditions _baromAlarm = AlarmConditions.Normal;
        private Double _windGustSpeed = 0.0;
        private Double _windGustDirection = 0.0;
        private AlarmConditions _windGustAlarm = AlarmConditions.Normal;
        private Epoch _powerUpTime = null;
        private PhysicalAddress _serial = null;
        private Epoch _time = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320MModule.X320MState</b>
        /// class. This is the default constructor.
        /// </summary>
        public X320MState() 
            : base() {
            // Construct epochs.
            this._rainTotalResetTime = new Epoch();
            this._powerUpTime = new Epoch();
            this._time = new Epoch();

            // Construct temperature sensors.
            this._tempSensor = new SensorInput();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current wind speed in miles-per-hour.
        /// </summary>
        public Double WindSpeed {
            get { return this._windSpeed; }
        }

        /// <summary>
        /// Gets the current wind direction (in compass degrees).
        /// </summary>
        public Double WindDirection {
            get { return this._windDirection; }
        }

        /// <summary>
        /// Gets the current total rainfall (in inches).
        /// </summary>
        public Double RainTotal {
            get { return this._rainTotal; }
        }

        /// <summary>
        /// Gets the temperature sensor.
        /// </summary>
        public SensorInput TemperatureSensor {
            get { return this._tempSensor; }
        }

        /// <summary>
        /// Gets the current relative humidity (percent).
        /// </summary>
        public Double Humidity {
            get { return this._humidity; }
        }

        /// <summary>
        /// Gets the current solar radiation (in Watts-per-square-meter).
        /// </summary>
        public Double SolarRadition {
            get { return this._solarRadiation; }
        }

        /// <summary>
        /// Gets the current barometric pressure (in inches of Mercury).
        /// </summary>
        public Double BarometricPressure {
            get { return this._barometricPressure; }
        }

        /// <summary>
        /// Gets the reading measured by auxiliary sensor 1.
        /// </summary>
        public Double AuxReading1 {
            get { return this._aux1; }
        }

        /// <summary>
        /// Gets the reading measured by auxiliary sensor 2.
        /// </summary>
        public Double AuxReading2 {
            get { return this._aux2; }
        }

        /// <summary>
        /// Gets the total rainfall (in inches) for the last 1 hour.
        /// </summary>
        public Double RainTotalLastHour {
            get { return this._rainTotalOneHour; }
        }

        /// <summary>
        /// Gets the total rainfal (in inches) since midnight.
        /// </summary>
        public Double RainTotalToday {
            get { return this._rainTotalToday; }
        }

		/// <summary>
		/// Gets the total rainfall (in inches) for the last 7 days.
		/// </summary>
		public Double RainTotalLastSevenDays {
			get { return this._rainTotalSevenDays; }
		}

        /// <summary>
        /// Gets the time that the rain total was reset.
        /// </summary>
        public Epoch RainTotalResetTime {
            get { return this._rainTotalResetTime; }
        }

        /// <summary>
        /// Gets the state of the rain sensor alarms.
        /// </summary>
        public AlarmConditions RainAlarm {
            get { return this._rainAlarm; }
        }

        /// <summary>
        /// Gets today's current high temperature (in degrees Fahrenheit).
        /// </summary>
        public Double TemperatureHighToday {
            get { return this._tempHigh; }
        }

        /// <summary>
        /// Gets today's current low temperature (in degrees Fahrenheit).
        /// </summary>
        public Double TemperatureLowToday {
            get { return this._tempLow; }
        }

        /// <summary>
        /// Gets yesterday's high temperature (in degrees Fahrenheit).
        /// </summary>
        public Double TemperatureHighYesterday {
            get { return this._tempHighYesterday; }
        }

        /// <summary>
        /// Gets yesterday's low temperature (in degrees Fahrenheit).
        /// </summary>
        public Double TemperatureLowYesterday {
            get { return this._tempLowYesterday; }
        }

        /// <summary>
        /// Gets the calculated heat index value (in degrees Fahrenheit).
        /// </summary>
        public Double HeatIndex {
            get { return this._heatIndex; }
        }

        /// <summary>
        /// Gets the calculated wind chill value (in degrees Fahrenheit).
        /// </summary>
        public Double WindChill {
            get { return this._windChill; }
        }

        /// <summary>
        /// Gets the calculated temperature at which water vapor in the air
        /// will condense into liquid water (dew point) in degrees Fahrenheit.
        /// </summary>
        public Double DewPoint {
            get { return this._dewPoint; }
        }

        /// <summary>
        /// Gets the current state of the temperature sensor alarms.
        /// </summary>
        public AlarmConditions TemperatureAlarm {
            get { return this._tempAlarm; }
        }

        /// <summary>
        /// Gets today's highest humidity reading (percent).
        /// </summary>
        public Double HumidityHighToday {
            get { return this._humidityHigh; }
        }

        /// <summary>
        /// Gets today's lowest humidity reading (percent).
        /// </summary>
        public Double HumidityLowToday {
            get { return this._humidityLow; }
        }

        /// <summary>
        /// Gets yesterday's highest humidity reading (percent).
        /// </summary>
        public Double HumidityHighYesterday {
            get { return this._humidityHighYesterday; }
        }

        /// <summary>
        /// Gets yesterday's lowest humidity reading (percent).
        /// </summary>
        public Double HumidityLowYesterday {
            get { return this._humidityLowYesterday; }
        }

        /// <summary>
        /// Gets the current state of the humidity alarms.
        /// </summary>
        public AlarmConditions HumidityAlarm {
            get { return this._humidityAlarm; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last hour (in inches of
        /// Mercury).
        /// </summary>
        public Double BarometricPressureLastHour {
            get { return this._baromPresLastHour; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last three hours (in inches of
        /// Mercury).
        /// </summary>
        public Double BarometricPressureLastThreeHours {
            get { return this._baromPresLastThreeHours; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last six hours (in inches of
        /// Mercury).
        /// </summary>
        public Double BarometricPressureLastSixHours {
            get { return this._baromPresLastSixHours; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last nine hours (in inches of
        /// Mercury).
        /// </summary>
        public Double BarometricPressureLastNineHours {
            get { return this._baromPresLastNineHours; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last twelve hours (in inches
        /// of Mercury).
        /// </summary>
        public Double BarometricPressureLastTwelveHours {
            get { return this._baromPresLastTwelveHours; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last fifteen hours.
        /// </summary>
        public Double BarometricPressureLastFifteenHours {
            get { return this._baromPresLastFifteenHours; }
        }

        /// <summary>
        /// Gets the barometric pressure for the last 24 hours.
        /// </summary>
        public Double BarometricPressureLastTwentyFourHours {
            get { return this._baromPresLastTwentyFourHours; }
        }

        /// <summary>
        /// Gets the current state of the barometric pressure alarms.
        /// </summary>
        public AlarmConditions BarometerAlarm {
            get { return this._baromAlarm; }
        }

        /// <summary>
        /// Gets the current speed of wind gusts (in miles-per-hour).
        /// </summary>
        public Double WindGustSpeed {
            get { return this._windGustSpeed; }
        }

        /// <summary>
        /// Gets the current direction of the wind gusts (in compass degrees).
        /// </summary>
        public Double WindGustDirection {
            get { return this._windGustDirection; }
        }

        /// <summary>
        /// Gets the current state of the wind speed alarms.
        /// </summary>
        public AlarmConditions WindGustAlarm {
            get { return this._windGustAlarm; }
        }

        /// <summary>
        /// Gets the time the device was powered on.
        /// </summary>
        public Epoch PowerUpTime {
            get { return this._powerUpTime; }
        }

        /// <summary>
        /// Gets the serial number of the device (MAC address).
        /// </summary>
        public PhysicalAddress Serial {
            get { return this._serial; }
        }

        /// <summary>
        /// Gets the current time as reported by the device.
        /// </summary>
        public Epoch Time {
            get { return this._time; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes the current wind speed.
        /// </summary>
        /// <param name="windSpeed">
        /// Wind speed in miles per hour.
        /// </param>
        public void ChangeWindSpeed(Double windSpeed) {
            this._windSpeed = windSpeed;
        }

        /// <summary>
        /// Changes the current wind direction.
        /// </summary>
        /// <param name="direction">
        /// Wind direction in compass degrees.
        /// </param>
        public void ChangeWindDirection(Double direction) {
            this._windDirection = direction;
        }

        /// <summary>
        /// Changes the current total rainfall.
        /// </summary>
        /// <param name="rainfall">
        /// The amount of rainfall in inches.
        /// </param>
        public void ChangeRainTotal(Double rainfall) {
            this._rainTotal = rainfall;
        }

        /// <summary>
        /// Changes the temperature sensor.
        /// </summary>
        /// <param name="sensor">
        /// The sensor to set.
        /// </param>
        public void ChangeTemperatureSensor(SensorInput sensor) {
            if (sensor == null) {
                sensor = new SensorInput();
            }
            this._tempSensor = sensor;
        }

        /// <summary>
        /// Changes the current humidity.
        /// </summary>
        /// <param name="humidity">
        /// The percentage of humidity.
        /// </param>
        public void ChangeHumidty(Double humidity) {
            this._humidity = humidity;
        }

        /// <summary>
        /// Changes the solar radation level.
        /// </summary>
        /// <param name="radiation">
        /// The amount of radiation in Watts per square meter.
        /// </param>
        public void ChangeSolarRadition(Double radiation) {
            this._solarRadiation = radiation;
        }

        /// <summary>
        /// Changes the barometric pressure.
        /// </summary>
        /// <param name="inHg">
        /// The amount of pressure in inches of Mercury.
        /// </param>
        public void ChangeBarometricPressure(Double inHg) {
            this._barometricPressure = inHg;
        }

        /// <summary>
        /// Sets the value of the specified auxiliary input.
        /// </summary>
        /// <param name="sensorNum">
        /// The sensor number to change (1 or 2).
        /// </param>
        /// <param name="value">
        /// The value to assign to the input.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sensorNum"/> must be either 1 or 2.
        /// </exception>
        public void ChangeAuxiliarySensorValue(Int32 sensorNum, Double value) {
            switch (sensorNum) {
                case 1: this._aux1 = value; break;
                case 2: this._aux2 = value; break;
                default: throw new ArgumentOutOfRangeException("sensorNum", "Value must be 1 or 2.");
            }
        }

        /// <summary>
        /// Changes the rainfall history values.
        /// </summary>
        /// <param name="increment">
        /// The increment to assign the value to.
        /// </param>
        /// <param name="value">
        /// The rainfall in inches.
        /// </param>
        public void ChangeRainfallHistory(RainfallReadings increment, Double value) {
            switch (increment) {
                case RainfallReadings.LastHour: 
                    this._rainTotalOneHour = value;
                    break;
                case RainfallReadings.TotalSevenDays: 
                    this._rainTotalSevenDays = value;
                    break;
                case RainfallReadings.TotalToday:
                    this._rainTotalToday = value;
                    break;
            }
        }

        /// <summary>
        /// Change the time that the total rain counts were reset.
        /// </summary>
        /// <param name="time">
        /// The time to set.
        /// </param>
        public void ChangeRainTotalResetTime(Epoch time) {
            if (time == null) {
                time = new Epoch();
            }
            this._rainTotalResetTime = time;
        }

        /// <summary>
        /// Changes the state of the rainfall alarm.
        /// </summary>
        /// <param name="condition">
        /// The alarm condition to set.
        /// </param>
        public void ChangeRainAlarmCondition(AlarmConditions condition) {
            this._rainAlarm = condition;
        }

        /// <summary>
        /// Changes the value of a historical or calculated temperature.
        /// </summary>
        /// <param name="type">
        /// The temperature to change.
        /// </param>
        /// <param name="value">
        /// The temperature value to set (in Fahrenheit).
        /// </param>
        public void ChangeTemperatureHistory(TemperatureReadings type, Double value) {
            switch (type) {
                case TemperatureReadings.DewPoint:
                    this._dewPoint = value;
                    break;
                case TemperatureReadings.HeatIndex:
                    this._heatIndex = value;
                    break;
                case TemperatureReadings.HighTempToday:
                    this._tempHigh = value;
                    break;
                case TemperatureReadings.HighTempYesterday:
                    this._tempHighYesterday = value;
                    break;
                case TemperatureReadings.LowTempToday:
                    this._tempLow = value;
                    break;
                case TemperatureReadings.LowTempYesterday:
                    this._tempLowYesterday = value;
                    break;
                case TemperatureReadings.WindChill:
                    this._windChill = value;
                    break;
            }
        }

        /// <summary>
        /// Changes the current state of the temperature alarms.
        /// </summary>
        /// <param name="condition">
        /// The condition to set.
        /// </param>
        public void ChangeTemperatureAlarmCondition(AlarmConditions condition) {
            this._tempAlarm = condition;
        }

		/// <summary>
		/// Changes the value of a historical humidity reading.
		/// </summary>
		/// <param name="type">
		/// The type of humidity history to change.
		/// </param>
		/// <param name="value">
		/// The humidity value to set (percentage).
		/// </param>
		public void ChangeHumidityHistory(HumidityReadings type, Double value) {
			switch (type) {
				case HumidityReadings.HighToday:
					this._humidityHigh = value; 
					break;
				case HumidityReadings.HighYesterday:
					this._humidityHighYesterday = value;
					break;
				case HumidityReadings.LowToday:
					this._humidityLow = value;
					break;
				case HumidityReadings.LowYesterday:
					this._humidityLowYesterday = value;
					break;
			}
		}

		/// <summary>
		/// Changes the current state of the humidity alarms.
		/// </summary>
		/// <param name="condition">
		/// Condition.
		/// </param>
		public void ChangeHumidityAlarmCondition(AlarmConditions condition) {
			this._humidityAlarm = condition;
		}

		/// <summary>
		/// Changes the value of a historical barometer reading.
		/// </summary>
		/// <param name="type">
		/// The type of barometer history to change.
		/// </param>
		/// <param name="value">
		/// The value of the barometer history to set.
		/// </param>
		public void ChangeBarometerHistory(BarometerReadings type, Double value) {
			switch (type) {
				case BarometerReadings.LastFifteenHours:
					this._baromPresLastFifteenHours = value;
					break;
				case BarometerReadings.LastHour:
					this._baromPresLastHour = value;
					break;
				case BarometerReadings.LastNineHours:
					this._baromPresLastNineHours = value;
					break;
				case BarometerReadings.LastSixHours:
					this._baromPresLastSixHours = value;
					break;
				case BarometerReadings.LastThreeHours:
					this._baromPresLastThreeHours = value;
					break;
				case BarometerReadings.LastTwelveHours:
					this._baromPresLastTwelveHours = value;
					break;
				case BarometerReadings.LastTwentyFourHours:
					this._baromPresLastTwentyFourHours = value;
					break;
			}
		}

		/// <summary>
		/// Changes the current state of the barometer alarms.
		/// </summary>
		/// <param name="condition">
		/// The condition to set.
		/// </param>
		public void ChangeBarometerAlarmCondition(AlarmConditions condition) {
			this._baromAlarm = condition;
		}

		/// <summary>
		/// Changes the wind gust speed.
		/// </summary>
		/// <param name="speed">
		/// Speed in miles-per-hour.
		/// </param>
		public void ChangeWindGustSpeed(Double speed) {
			this._windGustSpeed = speed;
		}

		/// <summary>
		/// Changes the wind gust direction.
		/// </summary>
		/// <param name="degrees">
		/// The direction in compass degrees.
		/// </param>
		public void ChangeWindGustDirection(Double degrees) {
			this._windGustDirection = degrees;
		}

		/// <summary>
		/// Changes the state of the wind gust alarms.
		/// </summary>
		/// <param name="condition">
		/// The alarm condition to set.
		/// </param>
		public void ChangeWindGustAlarmCondition(AlarmConditions condition) {
			this._windGustAlarm = condition;
		}

		/// <summary>
		/// Changes the time the device was powered on.
		/// </summary>
		/// <param name="time">
		/// The time to set.
		/// </param>
		public void ChangePowerUpTime(Epoch time) {
			if (time == null) {
				time = new Epoch();
			}
			this._powerUpTime = time;
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
		public void ChangeTime(Epoch time) {
			if (time == null) {
				time = new Epoch();
			}
			this._time = time;
		}

        /// <summary>
        /// Resets the state back to its original values.
        /// </summary>
        public override void Reset() {
            this._windSpeed = 0.0;
            this._windDirection = 0.0;
            this._rainTotal = 0.0;
            this._tempSensor.Reset();
            this._humidity = 0.0;
            this._solarRadiation = 0.0;
            this._barometricPressure = 0.0;
            this._aux1 = 0.0;
            this._aux2 = 0.0;
            this._rainTotalOneHour = 0.0;
            this._rainTotalToday = 0.0;
            this._rainTotalSevenDays = 0.0;
            this._rainTotalResetTime = new Epoch();
			this._rainAlarm = AlarmConditions.Normal;
			this._dewPoint = 0.0;
			this._heatIndex = 0.0;
			this._windChill = 0.0;
			this._tempHigh = 0.0;
			this._tempHighYesterday = 0.0;
			this._tempLow = 0.0;
			this._tempLowYesterday = 0.0;
			this._humidityHigh = 0.0;
			this._humidityHighYesterday = 0.0;
			this._humidityLow = 0.0;
			this._humidityLowYesterday = 0.0;
			this._humidityAlarm = AlarmConditions.Normal;
			this._baromPresLastFifteenHours = 0.0;
			this._baromPresLastHour = 0.0;
			this._baromPresLastNineHours = 0.0;
			this._baromPresLastSixHours = 0.0;
			this._baromPresLastThreeHours = 0.0;
			this._baromPresLastTwelveHours = 0.0;
			this._baromPresLastTwentyFourHours = 0.0;
			this._baromAlarm = AlarmConditions.Normal;
			this._windGustSpeed = 0.0;
			this._windGustDirection = 0.0;
			this._windGustAlarm = AlarmConditions.Normal;
			this._powerUpTime = new Epoch();
			this._time = new Epoch();
        }

        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public X320MState Clone() {
            X320MState state = new X320MState();
            state.ChangeAuxiliarySensorValue(1, this._aux1);
            state.ChangeAuxiliarySensorValue(2, this._aux2);
            state.ChangeBarometerAlarmCondition(this._baromAlarm);
            state.ChangeBarometerHistory(BarometerReadings.LastFifteenHours, this._baromPresLastFifteenHours);
            state.ChangeBarometerHistory(BarometerReadings.LastHour, this._baromPresLastHour);
            state.ChangeBarometerHistory(BarometerReadings.LastNineHours, this._baromPresLastNineHours);
            state.ChangeBarometerHistory(BarometerReadings.LastSixHours, this._baromPresLastSixHours);
            state.ChangeBarometerHistory(BarometerReadings.LastThreeHours, this._baromPresLastThreeHours);
            state.ChangeBarometerHistory(BarometerReadings.LastTwelveHours, this._baromPresLastTwelveHours);
            state.ChangeBarometerHistory(BarometerReadings.LastTwentyFourHours, this._baromPresLastTwentyFourHours);
            state.ChangeBarometricPressure(this._barometricPressure);
            state.ChangeHumidityAlarmCondition(this._humidityAlarm);
            state.ChangeHumidityHistory(HumidityReadings.HighToday, this._humidityHigh);
            state.ChangeHumidityHistory(HumidityReadings.HighYesterday, this._humidityHighYesterday);
            state.ChangeHumidityHistory(HumidityReadings.LowToday, this._humidityLow);
            state.ChangeHumidityHistory(HumidityReadings.LowYesterday, this._humidityLowYesterday);
            state.ChangeHumidty(this._humidity);
            state.ChangePowerUpTime(this._powerUpTime);
            state.ChangeRainAlarmCondition(this._rainAlarm);
            state.ChangeRainfallHistory(RainfallReadings.LastHour, this._rainTotalOneHour);
            state.ChangeRainfallHistory(RainfallReadings.TotalSevenDays, this._rainTotalSevenDays);
            state.ChangeRainfallHistory(RainfallReadings.TotalToday, this._rainTotalToday);
            state.ChangeRainTotal(this._rainTotal);
            state.ChangeRainTotalResetTime(this._rainTotalResetTime);
            state.ChangeSerial(this._serial);
            state.ChangeSolarRadition(this._solarRadiation);
            state.ChangeTemperatureAlarmCondition(this._tempAlarm);
            state.ChangeTemperatureHistory(TemperatureReadings.DewPoint, this._dewPoint);
            state.ChangeTemperatureHistory(TemperatureReadings.HeatIndex, this._heatIndex);
            state.ChangeTemperatureHistory(TemperatureReadings.HighTempToday, this._tempHigh);
            state.ChangeTemperatureHistory(TemperatureReadings.HighTempYesterday, this._tempHighYesterday);
            state.ChangeTemperatureHistory(TemperatureReadings.LowTempToday, this._tempLow);
            state.ChangeTemperatureHistory(TemperatureReadings.LowTempYesterday, this._tempLowYesterday);
            state.ChangeTemperatureHistory(TemperatureReadings.WindChill, this._windChill);
            state.ChangeTemperatureSensor(this._tempSensor);
            state.ChangeTime(this._time);
            state.ChangeWindDirection(this._windDirection);
            state.ChangeWindGustAlarmCondition(this._windGustAlarm);
            state.ChangeWindGustDirection(this._windGustDirection);
            state.ChangeWindGustSpeed(this._windGustSpeed);
            state.ChangeWindSpeed(this._windSpeed);
            return state;
        }
        #endregion
    }
}
