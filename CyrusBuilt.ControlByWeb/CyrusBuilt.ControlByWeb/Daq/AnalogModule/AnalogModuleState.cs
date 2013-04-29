using CyrusBuilt.ControlByWeb.Inputs;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.AnalogModule
{
    /// <summary>
    /// Represents the state of a ControlByWeb Analog Module device.
    /// </summary>
    public class AnalogModuleState : DeviceStateBase
    {
        #region Fields
        private AMInput _input0 = null;
        private AMInput _input1 = null;
        private AMInput _input2 = null;
        private AMInput _input3 = null;
        private AMInput _input4 = null;
        private AMInput _input5 = null;
        private AMInput _input6 = null;
        private AMInput _input7 = null;
        private PowerUpFlag _upFlag = PowerUpFlag.Off;
        private Double _resolution = AnalogModuleConstants.MAX_RESOLUTION;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.AnalogModule</b>
        /// class. This is the default constructor.
        /// </summary>
        public AnalogModuleState()
            : base() {
            this._input0 = new AMInput();
            this._input1 = new AMInput();
            this._input2 = new AMInput();
            this._input3 = new AMInput();
            this._input4 = new AMInput();
            this._input5 = new AMInput();
            this._input6 = new AMInput();
            this._input7 = new AMInput();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the analog input at ID zero.
        /// </summary>
        public AMInput Input0 {
            get { return this._input0; }
        }

        /// <summary>
        /// Gets the analog input at ID one.
        /// </summary>
        public AMInput Input1 {
            get { return this._input1; }
        }

        /// <summary>
        /// Gets the analog input at ID two.
        /// </summary>
        public AMInput Input2 {
            get { return this._input2; }
        }

        /// <summary>
        /// Gets the analog input at ID three.
        /// </summary>
        public AMInput Input3 {
            get { return this._input3; }
        }

        /// <summary>
        /// Gets the analog input at ID four.
        /// </summary>
        public AMInput Input4 {
            get { return this._input4; }
        }

        /// <summary>
        /// Gets the analog input at ID five.
        /// </summary>
        public AMInput Input5 {
            get { return this._input5; }
        }

        /// <summary>
        /// Gets the analog input at ID six.
        /// </summary>
        public AMInput Input6 {
            get { return this._input6; }
        }

        /// <summary>
        /// Gets the analog input at ID seven.
        /// </summary>
        public AMInput Input7 {
            get { return this._input7; }
        }

        /// <summary>
        /// Gets whether or not the device experienced a loss of power.
        /// </summary>
        public PowerUpFlag PowerUp {
            get { return this._upFlag; }
        }

        /// <summary>
        /// Gets or sets the resolution of the analog-to-digital converter
        /// (ADC). Default is <see cref="AnalogModuleConstants.MAX_RESOLUTION"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// value cannot be greater than <see cref="AnalogModuleConstants.MAX_RESOLUTION"/>
        /// bits.
        /// </exception>
        public Double Resolution {
            get { return this._resolution; }
            set {
                if (value > AnalogModuleConstants.MAX_RESOLUTION) {
                    String err = String.Format("Resolution cannot be greater than {0} bits.",
                                                AnalogModuleConstants.MAX_RESOLUTION.ToString());
                    throw new ArgumentException(err, "AnalogModuleState.Resolution");
                }
                this._resolution = value; 
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicitly sets or overrides the specified input.
        /// </summary>
        /// <param name="inputNum">
        /// The ID of the input to set.
        /// </param>
        /// <param name="input">
        /// The input to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputNum"/> must be an integer value from 
        /// <see cref="AnalogModuleConstants.MIN_INPUT_ID"/> to <see cref="AnalogModuleConstants.MAX_INPUT_ID"/>.
        /// </exception>
        public void SetOrOverrideInput(Int32 inputNum, AMInput input) {
            if ((inputNum < AnalogModuleConstants.MIN_INPUT_ID) ||
                (inputNum > AnalogModuleConstants.MAX_INPUT_ID)) {
                    String err = String.Format("Must be a value {0} - {1}.",
                                                AnalogModuleConstants.MIN_INPUT_ID.ToString(),
                                                AnalogModuleConstants.MAX_INPUT_ID.ToString());
                    throw new ArgumentOutOfRangeException(err, "inputNum");
            }

            if (input == null) {
                input = new AMInput();
            }

            switch (inputNum) {
                case 0: this._input0 = input; break;
                case 1: this._input1 = input; break;
                case 2: this._input2 = input; break;
                case 3: this._input3 = input; break;
                case 4: this._input4 = input; break;
                case 5: this._input5 = input; break;
                case 6: this._input6 = input; break;
                case 7: this._input7 = input; break;
            }
        }

        /// <summary>
        /// Gets the input at the specified ID.
        /// </summary>
        /// <param name="inputNum">
        /// The ID of the input to get.
        /// </param>
        /// <returns>
        /// The requested input, or null if <paramref name="inputNum"/> is invalid.
        /// </returns>
        public AMInput GetInput(Int32 inputNum) {
            AMInput retInp = null;
            switch (inputNum) {
                case 0: retInp = this._input0; break;
                case 1: retInp = this._input1; break;
                case 2: retInp = this._input2; break;
                case 3: retInp = this._input3; break;
                case 4: retInp = this._input4; break;
                case 5: retInp = this._input5; break;
                case 6: retInp = this._input6; break;
                case 7: retInp = this._input7; break;
            }
            return retInp;
        }

        /// <summary>
        /// Explicitly sets or overrides the state of the power up flag,
        /// indicating whether or not the device experienced power loss.
        /// </summary>
        /// <param name="powerUp">
        /// The flag value to set.
        /// </param>
        public void SetOrOverridePowerUpState(PowerUpFlag powerUp) {
            this._upFlag = powerUp;
        }

        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public AnalogModuleState Clone() {
            AnalogModuleState clone = new AnalogModuleState();
            for (Int32 i = AnalogModuleConstants.MIN_INPUT_ID; i <= AnalogModuleConstants.MAX_INPUT_ID; i++) {
                clone.SetOrOverrideInput(i, this.GetInput(i));
            }
            clone.SetOrOverridePowerUpState(this._upFlag);
            clone.Resolution = this._resolution;
            return clone;
        }

        /// <summary>
        /// Resets the state values back to default.
        /// </summary>
        public override void Reset() {
            for (Int32 i = AnalogModuleConstants.MIN_INPUT_ID; i <= AnalogModuleConstants.MAX_INPUT_ID; i++) {
                this.GetInput(i).Reset();
            }
            this._upFlag = PowerUpFlag.Off;
            this._resolution = AnalogModuleConstants.MAX_RESOLUTION;
        }

        /// <summary>
        /// Converts the current instance of <see cref="AnalogModuleState"/>
        /// to a string.
        /// </summary>
        /// <returns>
        /// The string representation of the current <see cref="AnalogModuleState"/>
        /// instance.
        /// </returns>
        public override String ToString() {
            String s = "Power up flag: ";
            String flag = "Off";
            if (this.PowerUp == PowerUpFlag.On) {
                flag = "On";
            }

            s = String.Concat(s, flag, Environment.NewLine);
            String resolution = String.Concat("Resolution: ", this._resolution.ToString(), " bits.");
            s = String.Concat(s, resolution, Environment.NewLine);

            String inputState = String.Empty;
            String mode = "Single-ended";
            AMInput inp = null;
            for (Int32 i = AnalogModuleConstants.MIN_INPUT_ID; i <= AnalogModuleConstants.MAX_INPUT_ID; i++) {
                inp = this.GetInput(i);
                if (inp.Mode == AnalogInputModes.Differential) {
                    mode = "Differential";
                }

                inputState = String.Format("Input {0} mode: {1}. Value: ", i.ToString(), mode);
                if (inp.Value > AnalogModuleConstants.MAX_INPUT_VALUE) {
                    inputState = String.Concat(inputState, AnalogModuleConstants.ERR_OUT_OF_RANGE);
                }
                else {
                    inputState = String.Concat(inputState, inp.Value.ToString());
                }
                s = String.Concat(s, inputState, Environment.NewLine);
            }
            return s;
        }
        #endregion
    }
}
