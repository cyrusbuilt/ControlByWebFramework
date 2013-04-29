using System;

namespace CyrusBuilt.ControlByWeb.Relays
{
    /// <summary>
    /// Represents a relay in a ControlByWeb module.
    /// </summary>
    public class Relay : IResetable
    {
        #region Fields
        private RelayState _state = RelayState.Off;
        private Double _pulseTime = Common.DEFAULT_PULSE_TIME;
        private readonly Guid _id = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Relay</b> class.
        /// This is the default constructor.
        /// </summary>
        public Relay() {
            this._id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Relay</b> class
        /// with the state of the relay.
        /// </summary>
        /// <param name="state">
        /// The state of the relay.
        /// </param>
        public Relay(RelayState state) {
            this._id = Guid.NewGuid();
            this._state = state;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Relay</b> class
        /// with the state of the relay and the pulse time in seconds. The pulse time
        /// is only useful when state is <see cref="RelayState.Pulse"/>.
        /// </summary>
        /// <param name="state">
        /// The state of the relay.
        /// </param>
        /// <param name="pulseTime">
        /// The duration of time (in seconds) to pulse the relay. Default is
        /// <see cref="Common.DEFAULT_PULSE_TIME"/> seconds. Max allowable is
        /// <see cref="Common.MAX_PULSE_DURATION"/> seconds.
        /// </param>
        /// <exception cref="ArgumentException">
        /// pulseTime must not exceed <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        public Relay(RelayState state, Double pulseTime) {
            this._id = Guid.NewGuid();
            this._state = state;
            if (pulseTime < 0) {
                this._pulseTime = Common.DEFAULT_PULSE_TIME;
            }
            else {
                if (pulseTime > Common.MAX_PULSE_DURATION) {
                    String err = String.Format("Pulse time cannot exceed {0}.",
                                                Common.MAX_PULSE_DURATION.ToString());
                    throw new ArgumentException(err);
                }
                this._pulseTime = pulseTime;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the state of the Relay.
        /// </summary>
        public RelayState State {
            get { return this._state; }
            set { this._state = value; }
        }

        /// <summary>
        /// Gets or sets the pulse time in seconds. Default is <see cref="Common.DEFAULT_PULSE_TIME"/> 
        /// seconds. Max allowable is <see cref="Common.MAX_PULSE_DURATION"/>. 
        /// </summary>
        /// <remarks>
        /// This is only useful if <see cref="State"/> is <see cref="RelayState.Pulse"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// value must not exceed <see cref="Common.MAX_PULSE_DURATION"/>.
        /// </exception>
        public Double PulseTime {
            get { return this._pulseTime; }
            set {
                if (value < 0) {
                    this._pulseTime = Common.DEFAULT_PULSE_TIME;
                }
                else {
                    if (value > Common.MAX_PULSE_DURATION) {
                        String err = String.Format("Pulse time cannot exceed {0}.",
                                                    Common.MAX_PULSE_DURATION.ToString());
                        throw new ArgumentException(err);
                    }
                    this._pulseTime = value;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the relay back to its default state.
        /// </summary>
        public void Reset() {
            this._state = RelayState.Off;
            this._pulseTime = Common.DEFAULT_PULSE_TIME;
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
        /// CyrusBuilt.ControlByWeb.Relay; otherwise, false.
        /// </returns>
        public override Boolean Equals(Object obj) {
            // If parameter is null, then fail.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to Relay, then fail.
            Relay r = obj as Relay;
            if ((Relay)r == null) {
                return false;
            }

            // Success if the fields match.
            return ((this._state == r.State) && (this.PulseTime == r.PulseTime));
        }

        /// <summary>
        /// Determines whether the specified CyrusBuilt.ControlByWeb.Relay is
        /// equal to the current instance.
        /// </summary>
        /// <param name="rel">
        /// The CyrusBuilt.ControlByWeb.Relay to compare to the current
        /// instance.
        /// </param>
        /// <returns>
        /// true if the specified CyrusBuilt.ControlByWeb.Relay is equal to the
        /// current CyrusBuilt.ControlByWeb.Relay; otherwise, false.
        /// </returns>
        public Boolean Equals(Relay rel) {
            // If param is null then fail.
            if ((Object)rel == null) {
                return false;
            }

            // Success if the fields match.
            return ((this.State == rel.State) && (this.PulseTime == rel.PulseTime));
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
