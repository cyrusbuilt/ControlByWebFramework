using System;

namespace CyrusBuilt.ControlByWeb.Inputs
{
    /// <summary>
    /// Represents an input on a ControlByWeb Five Input Module.
    /// </summary>
    public class StandardInput : IResetable
    {
        #region Fields
        private InputState _inState = InputState.Off;
        private Int32 _count = 0;
        private readonly Guid _id = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.StandardInput</b> class.
        /// This is the default constructor.
        /// </summary>
        public StandardInput() {
            this._id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.StandardInput</b> class
        /// with the state of the input.
        /// </summary>
        /// <param name="state">
        /// The state of the input.
        /// </param>
        public StandardInput(InputState state) {
            this._id = Guid.NewGuid();
            this._inState = state;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Inputs.StandardInput</b> class
        /// with the state of the input and the number of times the input has been triggered.
        /// </summary>
        /// <param name="state">
        /// The state of the input.
        /// </param>
        /// <param name="triggerCount">
        /// The number of times the input has been triggered.
        /// </param>
        public StandardInput(InputState state, Int32 triggerCount) {
            if (triggerCount < 0) {
                throw new ArgumentException("Count cannot be initialized to less than zero.", "triggerCount");
            }
            this._id = Guid.NewGuid();
            this._inState = state;
            this._count = triggerCount;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the state of the input.
        /// </summary>
        public InputState State {
            get { return this._inState; }
        }

        /// <summary>
        /// Gets the number of times the input has been triggered.
        /// </summary>
        public Int32 TriggerCount {
            get { return this._count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes the state of the input. If the input is currently off and is being triggered
        /// on, then the trigger count will increment by one.
        /// </summary>
        /// <param name="state">
        /// The state of the input (off or on).
        /// </param>
        public void ChangeState(InputState state) {
            if ((this._inState == InputState.Off) && (state == InputState.On)) {
                this._count++;
            }
            this._inState = state;
        }

        /// <summary>
        /// Triggers the input (on).
        /// </summary>
        /// <remarks>
        /// This is equivalent to calling ChangeState(InputState.On).
        /// </remarks>
        public void Trigger() {
            this.ChangeState(InputState.On);
        }

        /// <summary>
        /// Resets the state of the input (off) and trigger count.
        /// </summary>
        public void Reset() {
            this._inState = InputState.Off;
            this._count = 0;
        }

        /// <summary>
        /// Resets the number of times the input has been triggered. This sets the value
        /// of <see cref="TriggerCount"/> back to zero.
        /// </summary>
        public void ResetTriggerCount() {
            this._count = 0;
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
        /// CyrusBuilt.ControlByWeb.Daq.FiveInputModule.Input; otherwise, 
        /// false.
        /// </returns>
        public override Boolean Equals(Object obj) {
            // If parameter is null, then fail.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to Input, then fail.
            StandardInput theObj = obj as StandardInput;
            if ((StandardInput)theObj == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.State == theObj.State) && 
                    (this.TriggerCount == theObj.TriggerCount));
        }

        /// <summary>
        /// Determines whether the specified CyrusBuilt.ControlByWeb.Daq.FiveInputModule.Input
        /// is equal to the current instance.
        /// </summary>
        /// <param name="moduleInput">
        /// The CyrusBuilt.ControlByWeb.Daq.FiveInputModule.Input
        /// to compare to the current instance.
        /// </param>
        /// <returns>
        /// true if the specified CyrusBuilt.ControlByWeb.Daq.FiveInputModule.Input
        /// is equal to the current CyrusBuilt.ControlByWeb.Daq.FiveInputModule.Input;
        /// otherwise, false.
        /// </returns>
        public Boolean Equals(StandardInput moduleInput) {
            // If param is null then fail.
            if ((Object)moduleInput == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.State == moduleInput.State) &&
                    (this.TriggerCount == moduleInput.TriggerCount));
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
