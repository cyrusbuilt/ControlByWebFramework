using CyrusBuilt.ControlByWeb.Inputs;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.AnalogModule
{
    /// <summary>
    /// An implementation of <see cref="AnalogInput"/> specific to the 
    /// ControlByWeb Analog Module. This implementation adds the
    /// <see cref="IsOutOfRange"/> property used to indicate that the input
    /// value has exceeded the maximum allowed by the Analog Module.
    /// </summary>
    public class AMInput : AnalogInput
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AMInput</b>
        /// class. This is the default constructor.
        /// </summary>
        public AMInput()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AMInput</b>
        /// class with the input value.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        public AMInput(Double value)
            : base(value) {
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule.AMInput</b>
        /// class with the input value and mode.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        /// <param name="mode">
        /// The mode of this input.
        /// </param>
        public AMInput(Double value, AnalogInputModes mode)
            : base(value, mode) {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether or not the input value is greater than the maximum
        /// allowed (<see cref="AnalogModuleConstants.MAX_INPUT_VALUE"/>).
        /// </summary>
        public Boolean IsOutOfRange {
            get { return (base.Value > AnalogModuleConstants.MAX_INPUT_VALUE); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts the current value instance to a string.
        /// </summary>
        /// <returns>
        /// A string representation of the current instance value.
        /// </returns>
        public override String ToString() {
            String mode = "Single-ended";
            if (base.Mode == AnalogInputModes.Differential) {
                mode = "Differential";
            }

            String value = base.Value.ToString();
            if (this.IsOutOfRange) {
                value = AnalogModuleConstants.ERR_OUT_OF_RANGE;
            }
            return String.Format("Input mode: {0}. Value: {1}.", mode, value);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override Int32 GetHashCode() {
            return base.GetHashCode();
        }
        #endregion
    }
}
