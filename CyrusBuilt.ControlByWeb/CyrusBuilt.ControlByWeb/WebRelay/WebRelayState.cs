using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using System;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// The state of a ControlByWeb WebRelay module.
    /// </summary>
    public class WebRelayState : DeviceStateBase
    {
        #region Fields
        private Relay _relay = null;
        private InputState _inputState = InputState.Off;
        private RebootState _rebootState = RebootState.Pinging;
        private Int32 _totalReboots = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayState</b>
        /// class.  This is the default constructor.
        /// </summary>
        public WebRelayState()
            : base() {
            this._relay = new Relay();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the actual relay.
        /// </summary>
        public Relay Relay {
            get { return this._relay; }
            set { this._relay = value; }
        }

        /// <summary>
        /// Gets or sets the state of the input.
        /// </summary>
        public InputState InputState {
            get { return this._inputState; }
            set { this._inputState = value; }
        }

        /// <summary>
        /// Gets or sets the state of the reboot flag.
        /// </summary>
        public RebootState RebootState {
            get { return this._rebootState; }
            set { this._rebootState = value; }
        }

        /// <summary>
        /// Gets or sets the total number of reboots.
        /// </summary>
        public Int32 TotalReboots {
            get { return this._totalReboots; }
            set {
                if (value < 0) {
                    this._totalReboots = 0;
                }
                else {
                    this._totalReboots = value;
                }
            }
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
        public WebRelayState Clone() {
            WebRelayState clone = new WebRelayState();
            clone.InputState = this.InputState;
            clone.RebootState = this.RebootState;
            clone.Relay = this.Relay;
            clone.TotalReboots = this.TotalReboots;
            return clone;
        }

        /// <summary>
        /// Resets the total reboot count back to zero.
        /// </summary>
        public void ResetRebootCount() {
            this._totalReboots = 0;
        }

        /// <summary>
        /// Resets the state back to defaults.
        /// </summary>
        public override void Reset() {
            if (this._relay == null) {
                this._relay = new Relay();
            }
            else {
                this._relay.Reset();
            }
            this._inputState = InputState.Off;
            this._rebootState = RebootState.Pinging;
            this._totalReboots = 0;
        }

        /// <summary>
        /// Converts the current instance of <see cref="WebRelayState"/>
        /// to a string.
        /// </summary>
        /// <returns>
        /// The string representation of the current <see cref="WebRelayState"/>
        /// instance.
        /// </returns>
        public override String ToString()
        {
            // Get the relay state.
            String s = "Relay state: ";
            if (this._relay == null) {
                s = String.Concat(s, "Disassociated.");
            }
            else {
                switch (this._relay.State) {
                    case RelayState.DisableAutoReboot:
                        s = String.Concat(s, "DisableAutoReboot");
                        break;
                    case RelayState.EnableAutoReboot:
                        s = String.Concat(s, "EnableAutoReboot");
                        break;
                    case RelayState.Off:
                        s = String.Concat(s, "Off");
                        break;
                    case RelayState.On:
                        s = String.Concat(s, "On");
                        break;
                    case RelayState.Pulse:
                        String pulseTime = this._relay.PulseTime.ToString();
                        s = String.Concat(s, "Pulse. Pulse time: ", pulseTime);
                        break;
                    case RelayState.Reboot:
                        s = String.Concat(s, "Reboot");
                        break;
                    case RelayState.Toggle:
                        s = String.Concat(s, "Toggle");
                        break;
                    default:
                        break;
                }
            }

            // Get input state.
            s = String.Concat(s, Environment.NewLine);
            String inpState = "Off";
            if (this._inputState == InputState.On) {
                inpState = "On";
            }

            // Get the reboot state.
            s = String.Concat(s, inpState, Environment.NewLine);
            String rState = "Reboot state: ";
            switch (this._rebootState) {
                case RebootState.AutoRebootOff:
                    rState = String.Concat(rState, "AutoRebootOff");
                    break;
                case RebootState.Pinging:
                    rState = String.Concat(rState, "Pinging");
                    break;
                case RebootState.Rebooting:
                    rState = String.Concat(rState, "Rebooting");
                    break;
                case RebootState.WaitingForBoot:
                    rState = String.Concat(rState, "WaitingForBoot");
                    break;
                case RebootState.WaitingForResponse:
                    rState = String.Concat(rState, "WaitingForResponse");
                    break;
            }

            // Get reboot count.
            s = String.Concat(s, rState, Environment.NewLine);
            s = String.Concat(s, "Total Reboots: ", this._totalReboots.ToString());
            return s;
        }
        #endregion
    }
}
