using System;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// The state of a ControlByWeb WebRelay Quad module. This is essentially
    /// the same as a WebRelay, except there are four relays instead of one.
    /// </summary>
    public class WebRelayQuadState : DeviceStateBase
    {
        #region Fields
        private Relay _relay1 = null;
        private Relay _relay2 = null;
        private Relay _relay3 = null;
        private Relay _relay4 = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelayQuadState</b>
        /// class. This is the default constructor.
        /// </summary>
        public WebRelayQuadState()
            : base() {
            this._relay1 = new Relay();
            this._relay2 = new Relay();
            this._relay3 = new Relay();
            this._relay4 = new Relay();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first relay.
        /// </summary>
        public Relay Relay1 {
            get { return this._relay1; }
            set { this._relay1 = value; }
        }

        /// <summary>
        /// Gets or sets the second relay.
        /// </summary>
        public Relay Relay2 {
            get { return this._relay2; }
            set { this._relay2 = value; }
        }

        /// <summary>
        /// Gets or sets the third relay.
        /// </summary>
        public Relay Relay3 {
            get { return this._relay3; }
            set { this._relay3 = value; }
        }

        /// <summary>
        /// Gets or sets the fourth relay.
        /// </summary>
        public Relay Relay4 {
            get { return this._relay4; }
            set { this._relay4 = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clones the current state instance into a new instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        public WebRelayQuadState Clone() {
            WebRelayQuadState clone = new WebRelayQuadState();
            clone.Relay1 = this.Relay1;
            clone.Relay2 = this.Relay2;
            clone.Relay3 = this.Relay3;
            clone.Relay4 = this.Relay4;
            return clone;
        }

        /// <summary>
        /// Convenience method for getting a relay reference by number.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to get.
        /// </param>
        /// <returns>
        /// The requested relay.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayNum must be an integer value not less than one and not greater
        /// than four (because there are exactly four relays).
        /// </exception>
        public Relay GetRelay(Int32 relayNum) {
            if ((relayNum < 1) || (relayNum > 4)) {
                throw new ArgumentOutOfRangeException("relayNum", "Must be a value 1 - 4.");
            }

            Relay rel = null;
            switch (relayNum) {
                case 1: rel = this.Relay1; break;
                case 2: rel = this.Relay2; break;
                case 3: rel = this.Relay3; break;
                case 4: rel = this.Relay4; break;
            }
            return rel;
        }

        /// <summary>
        /// Resets the state back to defaults.
        /// </summary>
        public override void Reset() {
            // For each relay, do a null check. If the relay is null,
            // reinstaniate. New instances automatically init to the
            // default values. Otherwise, reset the relay.
            Relay rel = null;
            for (Int32 i = 1; i <= 4; i++) {
                rel = this.GetRelay(i);
                if (rel == null) {
                    rel = new Relay();
                    continue;
                }
                rel.Reset();
            }
        }

        /// <summary>
        /// Converts the current instance of <see cref="WebRelayQuadState"/>
        /// to a string.
        /// </summary>
        /// <returns>
        /// The string representation of the current <see cref="WebRelayQuadState"/>
        /// instance.
        /// </returns>
        public override String ToString() {
            String s = String.Empty;
            String relState = String.Empty;
            Relay rel = null;
            for (Int32 i = 1; i <= 4; i++) {
                rel = this.GetRelay(i);
                if (rel == null) {
                    relState = String.Format("Relay {0} state: Disassociated.", i.ToString());
                }
                else {
                    // For the WebRelay Quad, auto-reboot does not apply.
                    relState = String.Format("Relay {0} state: ", i.ToString());
                    switch (rel.State) {
                        case RelayState.DisableAutoReboot:
                            relState = String.Concat("DisableAutoReboot");
                            break;
                        case RelayState.EnableAutoReboot:
                            relState = String.Concat("EnableAutoReboot");
                            break;
                        case RelayState.Off:
                            relState = String.Concat("Off");
                            break;
                        case RelayState.On:
                            relState = String.Concat("On");
                            break;
                        case RelayState.Pulse:
                            relState = String.Concat("Pulse");
                            break;
                        case RelayState.Reboot:
                            relState = String.Concat("Reboot");
                            break;
                        case RelayState.Toggle:
                            relState = String.Concat("Toggle");
                            break;
                    }
                }
                s = String.Concat(s, relState, Environment.NewLine);
            }
            return s;
        }
        #endregion
    }
}
