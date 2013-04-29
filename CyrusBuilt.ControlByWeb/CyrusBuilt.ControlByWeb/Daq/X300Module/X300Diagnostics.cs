using CyrusBuilt.ControlByWeb.Diagnostics;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Used to store diagnostic information about a ControlByWeb X-300 module.
    /// The properties in this class are meant to be read-only. So if the
    /// values MUST be changed (such as by a method that is retrieving the
    /// diagnostic info), then you must call the appropriate method for 
    /// explicitly setting the value.
    /// </summary>
    public class X300Diagnostics : DiagnosticsBase
    {
        #region Fields
        private Double _internalTemp = 00.0;
        private Double _vin = 00.0;
        private Double _internal5Volt = 00.0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X300Module.X300Diagnostics</b>
        /// class. This is the default constructor.
        /// </summary>
        public X300Diagnostics()
            : base() {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the internal temperature if the device.
        /// </summary>
        public Double InternalTemperature {
            get { return this._internalTemp; }
        }

        /// <summary>
        /// Gets the DC voltage applied to the VIn+ and VIn- terminals.
        /// </summary>
        public Double VoltageIn {
            get { return this._vin; }
        }

        /// <summary>
        /// Gets the DC voltage of the internal 5V power supply.
        /// </summary>
        public Double Internal5Volt {
            get { return this._internal5Volt; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicitly sets the internal temperature of the device.
        /// </summary>
        /// <param name="temp">
        /// The temperature to set. This value must be greater than or equal to
        /// 0.0 or it will be ignored.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public void SetInternalTemp(Double temp) {
            if (temp >= 0.0) {
                this._internalTemp = temp;
            }
        }

        /// <summary>
        /// Explicitly sets the voltage being applied to the VIn+ and VIn-
        /// terminals.
        /// </summary>
        /// <param name="volts">
        /// The voltage to set.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public void SetVoltageIn(Double volts) {
            if (volts >= 0.0) {
                this._vin = volts;
            }
        }

        /// <summary>
        /// Explicitly sets the voltage of the internal 6 volt power supply.
        /// </summary>
        /// <param name="volts">
        /// The voltage to set.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public void SetInternalVoltage(Double volts) {
            if (volts >= 0.0) {
                this._internal5Volt = volts;
            }
        }

        /// <summary>
        /// Resets the state of this instance. This will return all property
        /// values back to their initial default values.
        /// </summary>
        public override void Reset() {
            this._internalTemp = 00.0;
            this._vin = 00.0;
            this._internal5Volt = 00.0;
            base.Reset();
        }
        #endregion
    }
}
