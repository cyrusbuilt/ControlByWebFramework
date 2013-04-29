using System;

namespace CyrusBuilt.ControlByWeb.Inputs
{
    /// <summary>
    /// Represents a temperature sensor input.
    /// </summary>
    public class SensorInput : IResetable
    {
        #region Fields
        private Boolean _hasSensor = false;
        private Double _sensorTemp = 00.00;
        private readonly Guid _id = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.SensorInput</b> class.
        /// This is the default constructor.
        /// </summary>
        public SensorInput() {
            this._id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.SensorInput</b> class
        /// with the temperature reported by the sensor. By using this constructor, it is assumed that there is an
        /// digital temperature sensor associated with this input.
        /// </summary>
        /// <param name="temperature">
        /// The temperature reported by the sensor.
        /// </param>
        public SensorInput(Double temperature) {
            this._id = Guid.NewGuid();
            this._sensorTemp = temperature;
            this._hasSensor = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether or not there is a digital temperature sensor associated with this input.
        /// </summary>
        public Boolean HasSensor {
            get { return this._hasSensor; }
        }

        /// <summary>
        /// Gets the temperature reported by the sensor.
        /// </summary>
        public Double Temperature {
            get { return this._sensorTemp; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the tempurature of the input.
        /// </summary>
        /// <param name="temperature">
        /// The temperature to set.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// An attempt was made to set the tempurature of an input that has no
        /// digital temperature sensor associated with it.
        /// </exception>
        /// <remarks>
        /// This method only sets the temperature for the current SensorInput
        /// instance and does not affect the actual device.
        /// </remarks>
        public void SetTemperature(Double temperature) {
            if (!this._hasSensor) {
                throw new InvalidOperationException("Cannot set the temperature for an input with no sensor.");
            }
            this._sensorTemp = temperature;
        }

        /// <summary>
        /// Removes the sensor from the input.
        /// </summary>
        /// <remarks>
        /// This method only removes the sensor from the current SensorInput
        /// instance and does not affect the actual device.
        /// </remarks>
        public void RemoveSensor() {
            this._hasSensor = false;
            this._sensorTemp = 00.00;
        }

        /// <summary>
        /// Convenience method that resets the current instance properties back 
        /// to their default values.
        /// </summary>
        /// <remarks>
        /// This is the same as calling <see cref="RemoveSensor()"/>.
        /// </remarks>
        public void Reset() {
            this.RemoveSensor();
        }

        /// <summary>
        /// Adds a sensor to the input.
        /// </summary>
        /// <remarks>
        /// This method only adds a sensor to the current SensorInput instance
        /// and does not affect the actual device.
        /// </remarks>
        public void AddSensor() {
            this._hasSensor = true;
            this._sensorTemp = 00.00;
        }

        /// <summary>
        /// Adds a sensor to the input.
        /// </summary>
        /// <param name="sensorTemp">
        /// The tempurature reported by the sensor.
        /// </param>
        /// <remarks>
        /// This method only adds a sensor to the current SensorInput instance
        /// and does not affect the actual device.
        /// </remarks>
        public void AddSensor(Double sensorTemp) {
            this._hasSensor = true;
            this._sensorTemp = sensorTemp;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the
        /// current instance.
        /// </summary>
        /// <param name="obj">
        /// The System.Object to compare to the current instance.
        /// </param>
        /// <returns>
        /// true if the specified System.Object is equal to the current
        /// CyrusBuilt.ControlByWeb.Inputs.SensorInput; otherwise, false.
        /// </returns>
        public override Boolean Equals(Object obj) {
            // If parameter is null, then fail.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to SensorInput, then fail.
            SensorInput si = obj as SensorInput;
            if ((SensorInput)si == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.HasSensor == si.HasSensor) && 
                    (this.Temperature == si.Temperature));
        }

        /// <summary>
        /// Determines whether the specified CyrusBuilt.ControlByWeb.Inputs.SensorInput
        /// is equal to the current instance.
        /// </summary>
        /// <param name="input">
        /// The CyrusBuilt.ControlByWeb.Inputs.SensorInput
        /// to compare to the current instance.
        /// </param>
        /// <returns>
        /// true if the specified CyrusBuilt.ControlByWeb.Inputs.SensorInput is equal to the current 
        /// CyrusBuilt.ControlByWeb.Inputs.SensorInput; otherwise, false.
        /// </returns>
        public Boolean Equals(SensorInput input) {
            // If param is null then fail.
            if ((Object)input == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.HasSensor == input.HasSensor) &&
                    (this.Temperature == input.Temperature));
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override Int32 GetHashCode() {
            // Since we override Equals(), we need to override GetHashCode().
            // Since the fields used to calculate the hash code are supposed to
            // be immutable, and thus never change during the life of the
            // object, we return the hashcode of the randomly generated GUID
            // that was created in the constructor.
            return this._id.GetHashCode();
        }

        /// <summary>
        /// Returns as <see cref="System.String"/> that represents the current
        /// <see cref="CyrusBuilt.ControlByWeb.Inputs.SensorInput"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current
        /// <see cref="CyrusBuilt.ControlByWeb.Inputs.SensorInput"/>.
        /// </returns>
        public override String ToString() {
            return String.Format("Has sensor: {0}. Sensor temp: {1}.",
                                        this._hasSensor.ToString(),
                                        this._sensorTemp.ToString());
        }
        #endregion
    }
}
