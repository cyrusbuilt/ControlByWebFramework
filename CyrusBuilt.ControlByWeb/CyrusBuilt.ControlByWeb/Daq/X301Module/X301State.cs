using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using System;
using System.Net.NetworkInformation;

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
	/// <summary>
	/// The state of a ControlByWeb X-301 module.
	/// </summary>
	public class X301State : DeviceStateBase
	{
		#region Fields
		private StandardInput _input1 = null;
		private StandardInput _input2 = null;
		private Relay _relay1 = null;
		private Relay _relay2 = null;
		private Double _highTime1 = 0.00;
		private Double _highTime2 = 0.00;
		private Double _extVar0 = 0.00;
		private Double _extVar1 = 0.00;
		private Double _extVar2 = 0.00;
		private Double _extVar3 = 0.00;
		private Double _extVar4 = 0.00;
		private PhysicalAddress _serial = null;
		private Epoch _time = null;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.ControlByWeb.Daq.X301Module.X301State"/>
		/// class. This is the default constructor.
		/// </summary>
		public X301State()
            : base() {
			this._input1 = new StandardInput();
			this._input2 = new StandardInput();
			this._relay1 = new Relay();
			this._relay2 = new Relay();
			this._time = new Epoch();
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets the first input.
		/// </summary>
		public StandardInput Input1 {
			get { return this._input1; }
		}
		
		/// <summary>
		/// Gets the second input.
		/// </summary>
		public StandardInput Input2 {
			get { return this._input2; }
		}
		
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
		/// Gets the amount of time elapsed, in seconds, since voltage was
		/// applied to the first input.
		/// </summary>
		public Double HighTime1 {
			get { return this._highTime1; }
		}
		
		/// <summary>
		/// Gets the amount of time elapsed, in seconds, since voltage was
		/// applied to the second input.
		/// </summary>
		public Double HighTime2 {
			get { return this._highTime2; }
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
		/// ets the time displayed in "epoch time" (Unix time - number of
		/// seconds since January 1, 1970).
		/// </summary>
		public Epoch Time {
			get { return this._time; }
		}
		
		/// <summary>
		/// Gets the serial number (MAC address) of the device.
		/// </summary>
		public PhysicalAddress Serial {
			get { return this._serial; }
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Explicitly sets or overrides the first input.
		/// </summary>
		/// <param name="input">
		/// The input to set.
		/// </param>
		public void SetInput1(StandardInput input) {
			this._input1 = input;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the second input.
		/// </summary>
		/// <param name="input">
		/// The input to set.
		/// </param>
		public void SetInput2(StandardInput input) {
			this._input2 = input;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the time elapsed, in seconds, since
		/// voltage was applied to input one.
		/// </summary>
		/// <param name="time">
		/// The time in seconds to set.
		/// </param>
		public void SetHighTime1(Double time) {
			if (time >= 0.0) {
				this._highTime1 = time;
			}
		}
		
		/// <summary>
		/// Explicitly sets or overrides the time elapsed, in seconds, since
		/// voltage was applied to input two.
		/// </summary>
		/// <param name="time">
		/// The time in seconds to set.
		/// </param>
		public void SetHighTime2(Double time) {
			if (time >= 0.0) {
				this._highTime2 = time;	
			}
		}
		
		/// <summary>
		/// Explicitly sets or overrides the value of external variable zero.
		/// </summary>
		/// <param name="val">
		/// The value to assign.
		/// </param>
		public void SetExtVar0(Double val) {
			this._extVar0 = val;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the value of external variable one.
		/// </summary>
		/// <param name="val">
		/// The value to assign
		/// </param>
		public void SetExtVar1(Double val) {
			this._extVar1 = val;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the value of external variable two.
		/// </summary>
		/// <param name="val">
		/// The value to assign.
		/// </param>
		public void SetExtVar2(Double val) {
			this._extVar2 = val;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the value of external variable three.
		/// </summary>
		/// <param name="val">
		/// The value to assign.
		/// </param>
		public void SetExtVar3(Double val) {
			this._extVar3 = val;
		}
		
		/// <summary>
		/// Explicitly sets or overrides the value of external variable four.
		/// </summary>
		/// <param name="val">
		/// The value to assign.
		/// </param>
		public void SetExtVar4(Double val) {
			this._extVar4 = val;
		}

        /// <summary>
        /// Explicitly sets or overrides the device's serial number.
        /// </summary>
        /// <param name="serial">
        /// The serial number (MAC address) to set.
        /// </param>
        public void SetSerial(PhysicalAddress serial) {
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
        public void SetSerial(String serial) {
            try {
                this.SetSerial(PhysicalAddress.Parse(serial));
            }
            catch (FormatException) {
                throw;
            }
        }

        /// <summary>
        /// Explicitly sets or overrides the Epoch (Unix) time value.
        /// </summary>
        /// <param name="time">
        /// The Epoch (Unix) time.
        /// </param>
        public void SetTime(Epoch time) {
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
        public void SetTime(Int64 seconds) {
            if (seconds >= 0) {
                this._time = new Epoch(seconds);
            }
        }
		
		/// <summary>
		/// Gets the value of an external variable.
		/// </summary>
		/// <param name="id">
		/// The ID of the external variable. Must be a value of <see cref="X301Constants.EXT_VAR_MIN_ID"/>
		/// through <see cref="X301Constants.EXT_VAR_MAX_ID"/>.
		/// </param>
		/// <returns>
		/// The value of the requested external variable.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// id is invalid.
		/// </exception>
		public Double GetExternalVar(Int32 id) {
			if ((id < X301Constants.EXT_VAR_MIN_ID) ||
				(id > X301Constants.EXT_VAR_MAX_ID)) {
				String err = String.Format("Must be an ID {0} - {1}.",
											X301Constants.EXT_VAR_MIN_ID.ToString(),
											X301Constants.EXT_VAR_MAX_ID.ToString());
				throw new ArgumentOutOfRangeException("id", err);
			}
			
			Double extVar = 0.00;
			switch (id) {
				case 0: extVar = this._extVar0; break;
				case 1: extVar = this._extVar1; break;
				case 2: extVar = this._extVar2; break;
				case 3: extVar = this._extVar3; break;
				case 4: extVar = this._extVar4; break;
			}
			return extVar;
		}
		
		/// <summary>
		/// Sets the value of an external variable.
		/// </summary>
		/// <param name="id">
		/// The ID of the external variable to set.
		/// </param>
		/// <param name="val">
		/// The value to assign to the external variable.
		/// </param>
		public void SetExternalVar(Int32 id, Double val) {
			switch (id) {
				case 0: this._extVar0 = val; break;
				case 1: this._extVar1 = val; break;
				case 2: this._extVar2 = val; break;
				case 3: this._extVar3 = val; break;
				case 4: this._extVar4 = val; break;
			}
		}
		
		/// <summary>
		/// Resets all state values back to their defaults.
		/// </summary>
		public override void Reset() {
			if (this._input1 == null) {
				this._input1 = new StandardInput();
			}
			else {
				this._input1.Reset();
			}
			
			if (this._input2 == null) {
				this._input2 = new StandardInput();
			}
			else {
				this._input2.Reset();
			}
			
			if (this._relay1 == null) {
				this._relay1 = new Relay();
			}
			else {
				this._relay1.Reset();
			}
			
			if (this._relay2 == null) {
				this._relay2 = new Relay();
			}
			else {
				this._relay2.Reset();
			}
			
			this._time = new Epoch();
			this._highTime1 = 0.00;
			this._highTime2 = 0.00;
			for (Int32 i = X301Constants.EXT_VAR_MIN_ID; i <= X301Constants.EXT_VAR_MAX_ID; i++) {
				this.SetExternalVar(i, 0.00);
			}
		}

        /// <summary>
        /// Creates a new X301State that is a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new X301State that is a copy of this instance.
        /// </returns>
        public X301State Clone() {
            X301State clone = new X301State();
            Double varVal = 0;
            for (Int32 i = X301Constants.EXT_VAR_MIN_ID; i <= X301Constants.EXT_VAR_MAX_ID; i++) {
                varVal = this.GetExternalVar(i);
                clone.SetExternalVar(i, varVal);
            }
            clone.SetHighTime1(this.HighTime1);
            clone.SetHighTime2(this.HighTime2);
            clone.SetInput1(this.Input1);
            clone.SetInput2(this.Input2);
            clone.Relay1 = this.Relay1;
            clone.Relay2 = this.Relay2;
            clone.SetSerial(this.Serial);
            clone.SetTime(this.Time);
            return clone;
        }
		#endregion
	}
}

