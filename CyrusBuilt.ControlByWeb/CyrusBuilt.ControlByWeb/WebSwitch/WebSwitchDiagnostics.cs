using System;
using CyrusBuilt.ControlByWeb.Diagnostics;

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// WebSwitch diagnostics.
    /// </summary>
    public class WebSwitchDiagnostics : DiagnosticsBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchDiagnostics"/> 
        /// class. This is the default constructor.
        /// </summary>
        public WebSwitchDiagnostics()
            : base() {
            // Since everything we need is already in DiagnosticsBase, we just
            // do a base implementation here.
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
