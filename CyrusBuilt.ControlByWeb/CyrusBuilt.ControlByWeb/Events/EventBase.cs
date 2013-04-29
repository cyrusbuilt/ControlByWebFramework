using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Base class that represents a device event. This base class contains all
    /// of the properties of a device event except for the action, because the
    /// action can very between devices. In a derived class, it will be necessary
    /// to add this propeerty and any required logic.
    /// </summary>
    public abstract class EventBase : IResetable
    {
        #region Fields
        private Int32 _id = EventConstants.EVENT_MIN_ID;
        private Boolean _active = false;
        private DateTime _currentTime = DateTime.MinValue;
        private DateTime _nextEvent = DateTime.MinValue;
        private String _period = String.Empty;
        private Int32 _count = 0;
        private Int32 _relayId = 1;
        private Double _pulseDuration = Common.DEFAULT_PULSE_TIME;
        private String _description = String.Empty;
        private readonly Guid _guid = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Events.EventBase</b>
        /// class. This is the default constructor.
        /// </summary>
        protected EventBase() {
            this._guid = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this event.
        /// </summary>
        public Int32 Id {
            get { return this._id; }
        }

        /// <summary>
        /// Indicates whether or not this event is active.
        /// </summary>
        public Boolean Active {
            get { return this._active; }
        }

        /// <summary>
        /// Gets the current date and time as reported by the device.
        /// </summary>
        public DateTime CurrentTime {
            get { return this._currentTime; }
        }

        /// <summary>
        /// Gets the date and time when this event will occur next.
        /// </summary>
        public DateTime NextEvent {
            get { return this._nextEvent; }
        }

        /// <summary>
        /// For events that occur more than one time, this property indicates the
        /// period of the event (time between occurrences). The unit of time is
        /// indicated after the value (seconds(s), minutes(m), hours(h), days(d),
        /// or weeks(w)). If this property is set to 0, then the event has been
        /// disabled. For example, 1d would be a period of one day.
        /// </summary>
        public String Period {
            get { return this._period; }
        }

        /// <summary>
        /// Gets the number of remaining times the event will occur. If this
        /// property is zero and the event is active, then the event is always
        /// on.
        /// </summary>
        public Int32 Count {
            get { return this._count; }
        }

        /// <summary>
        /// Gets whether or not this event is always on.
        /// </summary>
        public Boolean AlwaysOn {
            get { return ((this._active) && (this._count == 0)); }
        }

        /// <summary>
        /// Gets the relay number (output) the event will apply to (1 or 2).
        /// </summary>
        public Int32 RelayNumber {
            get { return this._relayId; }
        }

        /// <summary>
        /// Gets the time (in seconds) at the relay (output) will be turned on
        /// (if <see cref="Action"/> is <see cref="X301Action.PulseRelay"/>).
        /// </summary>
        public Double PulseDuration {
            get { return this._pulseDuration; }
        }

        /// <summary>
        /// Gets whether or not this event is disabled.
        /// </summary>
        public Boolean IsDisabled {
            get { return (this._period == "0"); }
        }

        /// <summary>
        /// Gets or sets the description of the event.
        /// </summary>
        /// <remarks>
        /// The length of the value string cannot be greater than
        /// <see cref="EventConstants.DESCRIPTION_MAX_LENGTH"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// value is greater than <see cref="EventConstants.DESCRIPTION_MAX_LENGTH"/>
        /// in length.
        /// </exception>
        public String Description {
            get { return this._description; }
            set {
                if (value == null) {
                    this._description = String.Empty;
                    return;
                }

                if (value.Length > EventConstants.DESCRIPTION_MAX_LENGTH) {
                    String err = String.Format("Value cannot greater than {0} in length",
                                                EventConstants.DESCRIPTION_MAX_LENGTH.ToString());
                    throw new ArgumentException(err);
                }
                this._description = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets or overrides the ID of this event.
        /// </summary>
        /// <param name="id">
        /// The ID to set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// id must be a value between <see cref="EventConstants.EVENT_MIN_ID"/>
        /// and <see cref="EventConstants.EVENT_MAX_ID"/>.
        /// </exception>
        public void SetOrOverrideId(Int32 id) {
            if ((id < EventConstants.EVENT_MIN_ID) ||
                (id > EventConstants.EVENT_MAX_ID)) {
                String err = String.Format("Must be an ID of {0} - {1}.",
                                            EventConstants.EVENT_MIN_ID.ToString(),
                                            EventConstants.EVENT_MAX_ID.ToString());
                throw new ArgumentOutOfRangeException("id", err);
            }
            this._id = id;
        }

        /// <summary>
        /// Sets or overrides the current active state.
        /// </summary>
        /// <param name="active">
        /// Whether or not the state should be active.
        /// </param>
        public void SetOrOverrideActiveState(Boolean active) {
            this._active = active;
        }

        /// <summary>
        /// Sets or overrides the current time as reported by the device.
        /// </summary>
        /// <param name="time">
        /// The date/time to set.
        /// </param>
        public void SetOrOverrideCurrentTime(DateTime time) {
            this._currentTime = time;
        }

        /// <summary>
        /// Sets or overrides the time that the next event will occur.
        /// </summary>
        /// <param name="eventTime">
        /// The time of the next event.
        /// </param>
        public void SetOrOverrideNextEvent(DateTime eventTime) {
            this._nextEvent = eventTime;
        }

        /// <summary>
        /// Sets or overrides the period of the event (time between occurrences).
        /// </summary>
        /// <param name="period">
        /// The unit of time is indicated after the value (seconds(s), 
        /// minutes(m), hours(h), days(d), or weeks(w)). Setting this to "0"
        /// will disable the event.
        /// </param>
        public void SetOrOverridePeriod(String period) {
            this._period = period;
        }

        /// <summary>
        /// Convenience method for disabling this event.
        /// </summary>
        public void DisableEvent() {
            this.SetOrOverridePeriod("0");
        }

        /// <summary>
        /// Sets or overrides the number of remaining times this event will
        /// occur.
        /// </summary>
        /// <param name="count">
        /// The number of times the this event will occur. If this field is set
        /// to zero and the event is active, then this event is always on.
        /// </param>
        public void SetOrOverrideCount(Int32 count) {
            if (count < 0) {
                count = 0;
            }
            this._count = count;
        }

        /// <summary>
        /// Sets or overrides the ID of the relay that the action applies to.
        /// </summary>
        /// <param name="relayId">
        /// The relay number the action applies to.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// relayId must be either 1 or 2.
        /// </exception>
        public void SetOrOverrideRelay(Int32 relayId) {
            if ((relayId < 1) || (relayId > 2)) {
                throw new ArgumentOutOfRangeException("relayId", "Must be an ID of 1 or 2.");
            }
            this._relayId = relayId;
        }

        /// <summary>
        /// Sets or overrides the duration of the relay pulse.
        /// </summary>
        /// <param name="duration">
        /// The duration (in seconds) that the relay should be pulsed. Default
        /// is <see cref="Common.DEFAULT_PULSE_TIME"/>. If the value is zero
        /// or less, no pulse occurs.
        /// </param>
        public void SetOrOverridePulseDuration(Double duration) {
            if (duration < 0) {
                duration = 0;
            }
            this._pulseDuration = duration;
        }

        /// <summary>
        /// Resets all state values back to their defaults except for
        /// <see cref="Id"/> because the ID of the event instance should never
        /// change during the lifetime of the event unless there is need to 
        /// override the ID for some reason.
        /// </summary>
        protected void Reset() {
            this._active = false;
            this._currentTime = DateTime.MinValue;
            this._nextEvent = DateTime.MinValue;
            this._period = String.Empty;
            this._count = 0;
            this._relayId = 1;
            this._pulseDuration = Common.DEFAULT_PULSE_TIME;
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
        /// CyrusBuilt.ControlByWeb.Events.EventBase; otherwise, false.
        /// </returns>
        public override Boolean Equals(Object obj) {
            // If parameter is null, then fail.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to X320Event, then fail.
            EventBase evt = obj as EventBase;
            if ((EventBase)evt == null) {
                return false;
            }

            // Success if the fields match.
            Boolean isMatch = ((this.Active == evt.Active) &&
                                (this.AlwaysOn == evt.AlwaysOn) &&
                                (this.Count == evt.Count) &&
                                (this.CurrentTime == evt.CurrentTime) &&
                                (this.Id == evt.Id) &&
                                (this.IsDisabled == evt.IsDisabled) &&
                                (this.NextEvent == evt.NextEvent) &&
                                (this.Period == evt.Period) &&
                                (this.PulseDuration == evt.PulseDuration) &&
                                (this.RelayNumber == evt.RelayNumber));
            return isMatch;
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
            return this._guid.GetHashCode();
        }
        #endregion
    }
}
