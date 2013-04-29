using System;
using System.Net.NetworkInformation;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    /// <summary>
    /// Represents the state of a WebRelay10 module.
    /// </summary>
    public class WebRelay10State : DeviceStateBase
    {
        #region Fields
        private Relay _relay1 = null;
        private Relay _relay2 = null;
        private Relay _relay3 = null;
        private Relay _relay4 = null;
        private Relay _relay5 = null;
        private Relay _relay6 = null;
        private Relay _relay7 = null;
        private Relay _relay8 = null;
        private Relay _relay9 = null;
        private Relay _relay10 = null;
        private Double _extVar0 = 0.0;
        private Double _extVar1 = 0.0;
        private Double _extVar2 = 0.0;
        private Double _extVar3 = 0.0;
        private Double _extVar4 = 0.0;
        private PhysicalAddress _serial = null;
        private Epoch _time = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebRelay.WebRelay10State</b>
        /// class. This is the default constructor.
        /// </summary>
        public WebRelay10State()
            : base() {
            // Construct relays.
            this._relay1 = new Relay();
            this._relay2 = new Relay();
            this._relay3 = new Relay();
            this._relay4 = new Relay();
            this._relay5 = new Relay();
            this._relay6 = new Relay();
            this._relay7 = new Relay();
            this._relay8 = new Relay();
            this._relay9 = new Relay();
            this._relay10 = new Relay();
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

        /// <summary>
        /// Gets or sets the fifth relay.
        /// </summary>
        public Relay Relay5 {
            get { return this._relay5; }
            set { this._relay5 = value; }
        }

        /// <summary>
        /// Gets or sets the sixth relay.
        /// </summary>
        public Relay Relay6 {
            get { return this._relay6; }
            set { this._relay6 = value; }
        }

        /// <summary>
        /// Gets or sets the seventh relay.
        /// </summary>
        public Relay Relay7 {
            get { return this._relay7; }
            set { this._relay7 = value; }
        }

        /// <summary>
        /// Gets or sets the eigth relay.
        /// </summary>
        public Relay Relay8 {
            get { return this._relay8; }
            set { this._relay8 = value; }
        }

        /// <summary>
        /// Gets or sets the ninth relay.
        /// </summary>
        public Relay Relay9 {
            get { return this._relay9; }
            set { this._relay9 = value; }
        }

        /// <summary>
        /// Gets or sets the tenth relay.
        /// </summary>
        public Relay Relay10 {
            get { return this._relay10; }
            set { this._relay10 = value; }
        }

        /// <summary>
        /// Gets the value of external variable zero.
        /// </summary>
        public Double ExtVar0 {
            get { return this._extVar0; }
        }

        /// <summary>
        /// Gets the value of external variable one.
        /// </summary>
        public Double ExtVar1 {
            get { return this._extVar1; }
        }

        /// <summary>
        /// Gets the value of external variable two.
        /// </summary>
        public Double ExtVar2 {
            get { return this._extVar2; }
        }

        /// <summary>
        /// Gets the value of external variable three.
        /// </summary>
        public Double ExtVar3 {
            get { return this._extVar3; }
        }

        /// <summary>
        /// Gets the value of external variable four.
        /// </summary>
        public Double ExtVar4 {
            get { return this._extVar4; }
        }

        /// <summary>
        /// Gets the serial number (MAC) of the device.
        /// </summary>
        public PhysicalAddress Serial {
            get { return this._serial; }
        }

        /// <summary>
        /// Gets the current time as reported by the device in Unix time (epoch
        /// time) which is the number of seconds elapsed since January 1, 1970
        /// (the epoch).
        /// </summary>
        public Epoch Time {
            get { return this._time; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convenience method for getting a relay by number.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to get (1 - 10).
        /// </param>
        /// <returns>
        /// The requested relay, or null if <paramref name="relayNum"/> is not
        /// a value of 1 - 10, or if the requested relay is currently set null
        /// somehow.
        /// </returns>
        public Relay GetRelay(Int32 relayNum) {
            // This is ugly, but I don't know of a better way to do this.
            Relay rel = null;
            switch (relayNum) {
                case 1: rel = this._relay1; break;
                case 2: rel = this._relay2; break;
                case 3: rel = this._relay3; break;
                case 4: rel = this._relay4; break;
                case 5: rel = this._relay5; break;
                case 6: rel = this._relay6; break;
                case 7: rel = this._relay7; break;
                case 8: rel = this._relay8; break;
                case 9: rel = this._relay9; break;
                case 10: rel = this._relay10; break;
            }
            return rel;
        }

        /// <summary>
        /// Convenience method for setting a relay by number.
        /// </summary>
        /// <param name="relayNum">
        /// The number of the relay to set (1 - 10).
        /// </param>
        /// <param name="rel">
        /// The <see cref="Relay"/> object to apply to the specified relay.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="relayNum"/> must be an integer value 1 - 10.
        /// </exception>
        public void SetRelay(Int32 relayNum, Relay rel) {
            if (rel == null) {
                rel = new Relay();
            }

            switch (relayNum) {
                case 1: this._relay1 = rel; break;
                case 2: this._relay2 = rel; break;
                case 3: this._relay3 = rel; break;
                case 4: this._relay4 = rel; break;
                case 5: this._relay5 = rel; break;
                case 6: this._relay6 = rel; break;
                case 7: this._relay7 = rel; break;
                case 8: this._relay8 = rel; break;
                case 9: this._relay9 = rel; break;
                case 10: this._relay10 = rel; break;
                default: throw new ArgumentOutOfRangeException("relayNum", "Must be 1 - 10.");
            }
        }

        /// <summary>
        /// Convenience method for getting the value of an external variable by
        /// ID.
        /// </summary>
        /// <param name="varId">
        /// The ID of the external variable to get the value of (0 - 4).
        /// </param>
        /// <returns>
        /// The value. Returns -1 if <paramref name="varId"/> is invalid.
        /// </returns>
        public Double GetExternalVar(Int32 varId) {
            Double varValue = -1;
            switch (varId) {
                case 0: varValue = this._extVar0; break;
                case 1: varValue = this._extVar1; break;
                case 2: varValue = this._extVar2; break;
                case 3: varValue = this._extVar3; break;
                case 4: varValue = this._extVar4; break;
            }
            return varValue;
        }

        /// <summary>
        /// Explicitly sets or overrides the value of an external variable.
        /// </summary>
        /// <param name="varId">
        /// The ID of the variable to set.
        /// </param>
        /// <param name="val">
        /// The value to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="varId"/> is not a valid ID.
        /// </exception>
        public void SetOrOverrideExternalVar(Int32 varId, Double val) {
            switch (varId) {
                case 0: this._extVar0 = val; break;
                case 1: this._extVar1 = val; break;
                case 2: this._extVar2 = val; break;
                case 3: this._extVar3 = val; break;
                case 4: this._extVar4 = val; break;
                default: throw new ArgumentOutOfRangeException("varId", "Must be 0 - 4.");
            }
        }

        /// <summary>
        /// Explictly sets or overrides the serial number (MAC) of the device.
        /// </summary>
        /// <param name="serial">
        /// The serial to set.
        /// </param>
        public void SetOrOverrideSerial(PhysicalAddress serial) {
            this._serial = serial;
        }

        /// <summary>
        /// Explictly sets or overrides the serial number (MAC) of the device.
        /// </summary>
        /// <param name="serial">
        /// The string containing the serial to set.
        /// </param>
        /// <exception cref="FormatException">
        /// <paramref name="serial"/> could not be parsed into a valid
        /// <see cref="PhysicalAddress"/> (serial).
        /// </exception>
        public void SetOrOverrideSerial(String serial) {
            try {
                this.SetOrOverrideSerial(PhysicalAddress.Parse(serial));
            }
            catch (FormatException) {
                throw;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the time on the device.
        /// </summary>
        /// <param name="time">
        /// The time to set (Unix/Epoch time).
        /// </param>
        public void SetOrOverrideTime(Epoch time) {
            if (time == null) {
                this._time = new Epoch();
                return;
            }
            this._time = time;
        }

        /// <summary>
        /// Explicitly sets or overrides the Epoch (Unix) time value.
        /// </summary>
        /// <param name="seconds">
        /// The number of seconds elapsed since January 1, 1970 (epoch).
        /// </param>
        public void SetOrOverrideTime(Int64 seconds) {
            if (seconds >= 0) {
                this._time = new Epoch(seconds);
            }
        }

        /// <summary>
        /// Resets all the state values back to their defaults.
        /// </summary>
        public override void Reset() {
            // Relays.
            Relay rel = null;
            for (Int32 r = 1; r <= WebRelay10Constants.TOTAL_RELAYS; r++) {
                rel = this.GetRelay(r);
                if (rel != null) {
                    rel.Reset();
                    continue;
                }
                rel = new Relay();
            }

            // External vars.
            Double extVar = 0.0;
            for (Int32 e = WebRelay10Constants.EXT_VAR_MIN_ID; e <= WebRelay10Constants.EXT_VAR_MAX_ID; e++) {
                this.SetOrOverrideExternalVar(e, extVar);
            }

            // Everything else.
            this._serial = null;
            this._time = null;
        }

        /// <summary>
        /// Creates a new WebRelay10State that is a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A WebRelay10State that is a copy of this instance.
        /// </returns>
        public WebRelay10State Clone() {
            WebRelay10State clone = new WebRelay10State();
            
            // Relays.
            for (Int32 r = 1; r <= WebRelay10Constants.TOTAL_RELAYS; r++) {
                clone.SetRelay(r, this.GetRelay(r));
            }

            // External vars.
            for (Int32 e = WebRelay10Constants.EXT_VAR_MIN_ID; e <= WebRelay10Constants.EXT_VAR_MAX_ID; e++) {
                clone.SetOrOverrideExternalVar(e, this.GetExternalVar(e));
            }

            // Everything else.
            clone.SetOrOverrideSerial(this.Serial);
            clone.SetOrOverrideTime(this.Time);
            return clone;
        }
        #endregion
    }
}
