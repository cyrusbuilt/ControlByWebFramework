using System;

namespace CyrusBuilt.ControlByWeb.Diagnostics
{
    /// <summary>
    /// This is a base class for other diagnostics classes to abstract from
    /// for ControlByWeb devices that support diagnostic output.
    /// </summary>
    public abstract class DiagnosticsBase : IResetable
    {
        #region Fields
        private PowerUpFlag _memPowerUpFlag = PowerUpFlag.On;
        private PowerUpFlag _devPowerUpFlag = PowerUpFlag.On;
        private Int32 _powerLossCount = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Diagnostics.DiagnosticsBase</b>
        /// class. This is the default protected constructor.
        /// </summary>
        protected DiagnosticsBase() {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the flag indicating a loss of power to the capacitor-backed
        /// real-time clock. A value of <see cref="Common.POWERUP_FLAG_ON"/>
        /// indicates that the real-time clock lost power. This should only
        /// happen if the device has lost power for several days. There is
        /// an internal capacitor that can power the real-time clock for an
        /// extended period of time, even if the main power is lost. If the
        /// real-time clock loses power, the time will have to be reset. By
        /// default, this value will be <see cref="Common.POWERUP_FLAG_ON"/>
        /// until it is set to <see cref="Common.POWERUP_FLAG_OFF"/>.
        /// </summary>
        public virtual PowerUpFlag MemoryPowerUpFlag {
            get { return this._memPowerUpFlag; }
        }

        /// <summary>
        /// Gets the flag indicating a loss of power to the device itself.
        /// This can be set to <see cref="Common.POWERUP_FLAG_OFF"/>. A
        /// value of <see cref="Common.POWERUP_FLAG_ON"/> means the X-320
        /// has lost power at least one time since the flag was set to
        /// <see cref="Common.POWERUP_FLAG_OFF"/>.
        /// </summary>
        public virtual PowerUpFlag DevicePowerUpFlag {
            get { return this._devPowerUpFlag; }
        }

        /// <summary>
        /// Gets the count of how many times the X-320 has lost main power.
        /// </summary>
        public virtual Int32 PowerLossCount {
            get { return this._powerLossCount; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicitly sets the memory power up flag state.
        /// </summary>
        /// <param name="state">
        /// The flag state to set.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public virtual void SetMemoryPowerUpFlag(PowerUpFlag state) {
            this._memPowerUpFlag = state;
        }

        /// <summary>
        /// Explicitly sets the device power up flag state.
        /// </summary>
        /// <param name="state">
        /// The flag state to set.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public virtual void SetDevicePowerUpFlag(PowerUpFlag state) {
            this._devPowerUpFlag = state;
        }

        /// <summary>
        /// Explicitly sets the number of times the device has lost main power.
        /// </summary>
        /// <param name="count">
        /// The number of times the device has lost main power.
        /// </param>
        /// <remarks>
        /// Calling this method does not commit the change to the device itself.
        /// </remarks>
        public virtual void SetPowerLossCount(Int32 count) {
            if (count >= 0) {
                this._powerLossCount = count;
            }
        }

        /// <summary>
        /// Increments the power loss count by one.
        /// </summary>
        public virtual void IncrementPowerLossCount() {
            this._powerLossCount++;
        }

        /// <summary>
        /// Resets the power loss count back to zero (default).
        /// </summary>
        public virtual void ResetPowerLossCount() {
            this._powerLossCount = 0;
        }

        /// <summary>
        /// Resets the state of this instance. This will return all property
        /// values back to their initial default values.
        /// </summary>
        public virtual void Reset() {
            this._memPowerUpFlag = PowerUpFlag.On;
            this._devPowerUpFlag = PowerUpFlag.On;
            this._powerLossCount = 0;
        }
        #endregion
    }
}
