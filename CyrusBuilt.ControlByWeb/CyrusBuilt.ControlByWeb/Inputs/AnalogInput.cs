using System;

namespace CyrusBuilt.ControlByWeb.Inputs
{
    /// <summary>
    /// Represents an analog input.
    /// </summary>
    public class AnalogInput : IResetable
    {
        #region Fields
        private AnalogInputModes _mode = AnalogInputModes.SingleEnded;
        private Double _value = 0.0;
        private readonly Guid _id = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.AnalogInput</b>
        /// class. This is the default constructor.
        /// </summary>
        public AnalogInput() {
            this._id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.AnalogInput</b>
        /// class with the input value.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        public AnalogInput(Double value) {
            this._id = Guid.NewGuid();
            this._value = 0.0;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.AnalogInput</b>
        /// class with the input value and mode.
        /// </summary>
        /// <param name="value">
        /// The input value.
        /// </param>
        /// <param name="mode">
        /// The mode of this input.
        /// </param>
        public AnalogInput(Double value, AnalogInputModes mode) {
            this._id = Guid.NewGuid();
            this._value = value;
            this._mode = mode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the mode of this input.
        /// </summary>
        public AnalogInputModes Mode {
            get { return this._mode; }
        }

        /// <summary>
        /// Gets the input value.
        /// </summary>
        public Double Value {
            get { return this._value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the state of this input back its default value.
        /// </summary>
        public void Reset() {
            this._mode = AnalogInputModes.SingleEnded;
            this._value = 0.0;
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
        /// CyrusBuilt.ControlByWeb.Inputs.AnalogInput; otherwise, false.
        /// </returns>
        public override Boolean Equals(Object obj) {
            // If parameter is null, then fail.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to AnalogInput, then fail.
            AnalogInput ai = obj as AnalogInput;
            if ((AnalogInput)ai == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.Value == ai.Value) && (this.Mode == ai.Mode));
        }

        /// <summary>
        /// Determines whether the specified CyrusBuilt.ControlByWeb.Inputs.AnalogInput
        /// is equal to the current instance.
        /// </summary>
        /// <param name="input">
        /// The CyrusBuilt.ControlByWeb.Inputs.AnalogInput
        /// to compare to the current instance.
        /// </param>
        /// <returns>
        /// true if the specified CyrusBuilt.ControlByWeb.Inputs.AnalogInput is equal to the current 
        /// CyrusBuilt.ControlByWeb.Inputs.AnalogInput; otherwise, false.
        /// </returns>
        protected Boolean Equals(AnalogInput input) {
            // If param is null then fail.
            if ((Object)input == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.Value == input.Value) && (this.Mode == input.Mode));
        }

        /// <summary>
        /// Converts the current value instance to a string.
        /// </summary>
        /// <returns>
        /// A string representation of the current instance value.
        /// </returns>
        public override String ToString() {
            String mode = "Single-ended";
            if (this._mode == AnalogInputModes.Differential) {
                mode = "Differential";
            }

            String sRet = String.Format("Input mode: {0}. Value: {1}.",
                                        mode, this._value.ToString());
            return sRet;
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
        #endregion
    }
}
