using CyrusBuilt.ControlByWeb.Diagnostics;
using System;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Used to store diagnostic information about a ControlByWeb WebRelay-10
    /// module. The properties in this class are meant to be read-only. So if
    /// the values MUST be changed (such as by a method that is retrieving 
    /// the diagnostic info), then you must call the appropriate method for 
    /// explicitly setting the value.
    /// </summary>
    public class WebRelay10Diagnostics : DiagnosticsBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelay10Diagnostics</b>
        /// class. This is the default constructor.
        /// </summary>
        public WebRelay10Diagnostics() 
            : base() {
            // The base class contains everything we need to implement for this
            // class. Therefore, we don't need to implement anything else except
            // the default constructor.
        }
        #endregion

        #region Properties
        /// <summary>
        /// Because the MemoryPowerUpFlag is only supported in the WebSwitch
        /// Plus, this property will always return <see cref="PowerUpFlag.Off"/>,
        /// indicating normal operation.
        /// </summary>
        public override PowerUpFlag MemoryPowerUpFlag {
            get { return PowerUpFlag.Off; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// NOT SUPPORTED. DO NOT USE.
        /// </summary>
        /// <param name="state">
        /// None, as this method is not supported.
        /// </param>
        /// <exception cref="UnsupportedMethodException">
        /// This method is unsupported and should not be used.
        /// </exception>
        public override void SetMemoryPowerUpFlag(PowerUpFlag state) {
            throw new UnsupportedMethodException("SetMemoryPowerUpFlag", "Method only supported on WebSwitch Plus device.");
        }
        #endregion
    }
}
