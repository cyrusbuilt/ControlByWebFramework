using System;
using CyrusBuilt.ControlByWeb.Inputs;

namespace CyrusBuilt.ControlByWeb.Daq.FiveInputModule
{
    /// <summary>
    /// The state of a ControlByWeb Five Input Module.
    /// </summary>
    public class FiveInputModuleState : DeviceStateBase
    {
        #region Fields
        private StandardInput _input1 = null;
        private StandardInput _input2 = null;
        private StandardInput _input3 = null;
        private StandardInput _input4 = null;
        private StandardInput _input5 = null;
        private PowerUpFlag _flag = PowerUpFlag.Off;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.FiveInputModule.FiveInputModuleState</b> class.
        /// This is the default constructor.
        /// </summary>
        public FiveInputModuleState()
            : base() {
            this._input1 = new StandardInput();
            this._input2 = new StandardInput();
            this._input3 = new StandardInput();
            this._input4 = new StandardInput();
            this._input5 = new StandardInput();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first input a Five Input Module.
        /// </summary>
        public StandardInput Input1 {
            get { return this._input1; }
            set { this._input1 = value; }
        }

        /// <summary>
        /// Gets or sets the second input of a Five Input Module.
        /// </summary>
        public StandardInput Input2 {
            get { return this._input2; }
            set { this._input2 = value; }
        }

        /// <summary>
        /// Gets or sets the third input of a Five Input Module.
        /// </summary>
        public StandardInput Input3 {
            get { return this._input3; }
            set { this._input3 = value; }
        }

        /// <summary>
        /// Gets or sets the four input of a Five Input Module.
        /// </summary>
        public StandardInput Input4 {
            get { return this._input4; }
            set { this._input4 = value; }
        }

        /// <summary>
        /// Gets or sets the five input of Five Input Module.
        /// </summary>
        public StandardInput Input5 {
            get { return this._input5; }
            set { this._input5 = value; }
        }

        /// <summary>
        /// Gets or sets whether or not there was a loss of power.
        /// </summary>
        public PowerUpFlag PowerUp {
            get { return this._flag; }
            set { this._flag = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public FiveInputModuleState Clone() {
            FiveInputModuleState clone = new FiveInputModuleState();
            clone.Input1 = this.Input1;
            clone.Input2 = this.Input2;
            clone.Input3 = this.Input3;
            clone.Input4 = this.Input4;
            clone.Input5 = this.Input5;
            clone.PowerUp = this.PowerUp;
            return clone;
        }

        /// <summary>
        /// Convenience method for getting an input reference by number.
        /// </summary>
        /// <param name="inputNum">
        /// The number of the input to get.
        /// </param>
        /// <returns>
        /// The requested input.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// inputNum must be an integer value not less than one and not greater
        /// than five (because there are excactly five inputs).
        /// </exception>
        public StandardInput GetInput(Int32 inputNum) {
            if ((inputNum < 1) || (inputNum > FimConstants.TOTAL_STANDARD_INPUTS)) {
                String err = String.Format("Must be a value 1 - {0}.",
                                            FimConstants.TOTAL_STANDARD_INPUTS.ToString());
                throw new ArgumentOutOfRangeException("inputNum", err);
            }

            StandardInput input = null;
            switch (inputNum) {
                case 1: input = this.Input1; break;
                case 2: input = this.Input2; break;
                case 3: input = this.Input3; break;
                case 4: input = this.Input4; break;
                case 5: input = this.Input5; break;
            }
            return input;
        }

        /// <summary>
        /// Resets the state values back to default.
        /// </summary>
        public override void Reset() {
            StandardInput input = null;
            for (Int32 i = 1; i <= FimConstants.TOTAL_STANDARD_INPUTS; i++) {
                input = this.GetInput(i);
                if (input == null) {
                    input = new StandardInput();
                    continue;
                }
                input.Reset();
            }
            this._flag = PowerUpFlag.Off;
        }

        /// <summary>
        /// Converts the current instance of <see cref="FiveInputModuleState"/>
        /// to a string.
        /// </summary>
        /// <returns>
        /// The string representation of the current <see cref="FiveInputModuleState"/>
        /// instance.
        /// </returns>
        public override String ToString() {
            String s = "Power up flag: ";
            String flag = "Off";
            if (this.PowerUp == PowerUpFlag.On) {
                flag = "On";
            }

            s = String.Concat(s, flag, Environment.NewLine);
            String inputState = String.Empty;
            StandardInput inp = null;
            for (Int32 i = 1; i <= FimConstants.TOTAL_STANDARD_INPUTS; i++) {
                inp = this.GetInput(i);
                if (inp == null) {
                    // This could happen if we've been disposed.
                    inputState = String.Format("Input {0} state: No input.", i.ToString());
                }
                else {
                    String inpState = "Off";
                    if (inp.State == InputState.On) {
                        inpState = "On";
                    }

                    inputState = String.Format("Input {0} state: {1}. Trigger count: {2}.",
                                                i.ToString(),
                                                inpState,
                                                inp.TriggerCount.ToString());
                }
                s = String.Concat(s, inputState, Environment.NewLine);
            }
            return s;
        }
        #endregion
    }
}
