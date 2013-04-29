using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Security;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.WebRelay;
using System;
using System.Security;
using System.Xml;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Common utilities and constants that can be used with one or more modules.
    /// </summary>
    public static class Common
    {
        #region Relay State Constants
        /// <summary>
        /// The actual state value of a relay in the "off" position (de-energized).
        /// </summary>
        public const Int32 RELAY_STATE_OFF = 0;

        /// <summary>
        /// The actual state value of a relay in the "on" position (energized).
        /// </summary>
        public const Int32 RELAY_STATE_ON = 1;

        /// <summary>
        /// The actual state value of a relay that is pulsing (switching on and off).
        /// If in Automatic Reboot mode, this will reboot the device being controlled.
        /// </summary>
        public const Int32 RELAY_STATE_PULSE_OR_REBOOT = 2;

        /// <summary>
        /// The actual state value of a realy that has its auto-reboot feature
        /// disabled.
        /// </summary>
        public const Int32 RELAY_STATE_DISABLE_AUTO_REBOOT = 3;

        /// <summary>
        /// The actual state value of a relay that has its auto-reboot feature
        /// enabled.
        /// </summary>
        public const Int32 RELAY_STATE_ENABLE_AUTO_REBOOT = 4;

        /// <summary>
        /// The actual state value of a relay that is set to toggle.
        /// </summary>
        public const Int32 RELAY_STATE_TOGGLE = 5;

        /// <summary>
        /// The maximum allowable relay pulse duration.
        /// </summary>
        public const Double MAX_PULSE_DURATION = 86400;
        #endregion

        #region Input State Constants
        /// <summary>
        /// The actual state value of an input that is off or driven
        /// low (less than 5 volts).
        /// </summary>
        public const Int32 INPUT_STATE_OFF = 0;

        /// <summary>
        /// The actual state value of an input that is on or driven
        /// high (greater than 5 volts).
        /// </summary>
        public const Int32 INPUT_STATE_ON = 1;
        #endregion

        #region Reboot State Constants
        /// <summary>
        /// The actual value of a relay that has its reboot controller
        /// (auto-reboot feature) disabled (0).
        /// </summary>
        public const Int32 REBOOT_STATE_AUTO_REBOOT_OFF = 0;

        /// <summary>
        /// The actual value of a relay that is periodically pinging
        /// the device (1). This is normal operation.
        /// </summary>
        public const Int32 REBOOT_STATE_PINGING = 1;

        /// <summary>
        /// The actual value of a relay that is waiting for a response
        /// on a ping request to the device (2).
        /// </summary>
        public const Int32 REBOOT_STATE_WAITING_FOR_RESPONSE = 2;

        /// <summary>
        /// The actual value of a relay that is currently in a reboot cycle (3).
        /// </summary>
        public const Int32 REBOOT_STATE_REBOOTING = 3;

        /// <summary>
        /// The actual value of a relay that is powered on and booting up (4).
        /// </summary>
        public const Int32 REBOOT_STATE_WAITING_FOR_BOOT = 4;
        #endregion

        #region Module Constants
		/// <summary>
		/// The time (in seconds) between poll cycles.
		/// </summary>
		public const Int32 DEFAULT_POLL_INTERVAL = 5;
		
        /// <summary>
        /// The default communication port used on ControlByWeb modules.
        /// This port is defined as 80.
        /// </summary>
        public const Int32 DEFAULT_PORT = 80;

        /// <summary>
        /// The default amount of time to pulse a relay (1.5 seconds).
        /// </summary>
        public const Double DEFAULT_PULSE_TIME = 1.5;

        /// <summary>
        /// Indicates normal operation for a given power up indicator state.
        /// </summary>
        public const Int32 POWERUP_FLAG_OFF = 0;

        /// <summary>
        /// Indicates power loss for a give power up indicator state.
        /// </summary>
        public const Int32 POWERUP_FLAG_ON = 1;
        #endregion

        #region Alarm Constants
        /// <summary>
        /// Normal (off).
        /// </summary>
        public const Int32 ALARM_COND_NORMAL = 0;

        /// <summary>
        /// Alarm condition 1.
        /// </summary>
        public const Int32 ALARM_COND_HIGH = 1;

        /// <summary>
        /// Alarm condition 2.
        /// </summary>
        public const Int32 ALARM_COND_LOW = 2;
        #endregion

        #region Methods
        /// <summary>
        /// Appends an authorization string to a module command or request.
        /// </summary>
        /// <param name="command">
        /// The command to append the authorization string to.
        /// </param>
        /// <param name="password">
        /// The password to encode in the authorization string.
        /// </param>
        /// <returns>
        /// If command is null or empty, an empty string is returned. If password 
        /// is null, the specified command is returned as-is. Otherwise, a new 
        /// command  string with the command and authorization string is returned. 
        /// The credentials are Base64-encoded in the authorization string.
        /// </returns>
        public static String AppendAuthToCommand(String command, SecureString password) {
            if (String.IsNullOrEmpty(command)) {
                return String.Empty;
            }
            if (password == null) {
                return command;
            }
            String auth = String.Format("none:{0}", Crypto.ConvertToPlainString(password));
            String encAuth = Crypto.Base64Encode(auth);
            auth = null;
            return String.Format("{0}Authorization: Basic {1}\r\n\r\n", command, encAuth);
        }

        /// <summary>
        /// Converts an integer value representing the state of a relay to a
        /// <see cref="RelayState"/>.
        /// </summary>
        /// <param name="stateCode">
        /// An integer value representing the state of the relay.
        /// </param>
        /// <param name="autoRebootEnabled">
        /// A flag indicating whether or not the auto-reboot feature is enabled.
        /// </param>
        /// <returns>
        /// The RelayState associated with the specified code.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The stateCode is less than 0 or greater than 5.
        /// </exception>
        public static RelayState GetRelayState(Int32 stateCode, Boolean autoRebootEnabled) {
            if ((stateCode < 0) || (stateCode > 5)) {
                throw new ArgumentOutOfRangeException("stateCode", "Cannot be less than 0 or greater than 5.");
            }

            RelayState state = RelayState.Off;
            switch (stateCode) {
                case RELAY_STATE_OFF:
                    state = RelayState.Off;
                    break;
                case RELAY_STATE_ON:
                    state = RelayState.On;
                    break;
                case RELAY_STATE_PULSE_OR_REBOOT:
                    state = RelayState.Pulse;
                    if (autoRebootEnabled) {
                        state = RelayState.Reboot;
                    }
                    break;
                case 3:
                    state = RelayState.DisableAutoReboot;
                    break;
                case 4:
                    state = RelayState.EnableAutoReboot;
                    break;
                case 5:
                    state = RelayState.Toggle;
                    break;
                default:
                    break;
            }
            return state;
        }

        /// <summary>
        /// Converts an integer value representing the state of a relay to a
        /// <see cref="RelayState"/>.
        /// </summary>
        /// <param name="stateCode">
        /// An integer value representing the state of the relay.
        /// </param>
        /// <returns>
        /// The RelayState associated with the specified code.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The stateCode is less than 0 or greater than 5.
        /// </exception>
        public static RelayState GetRelayState(Int32 stateCode) {
            return GetRelayState(stateCode, false);
        }

        /// <summary>
        /// Gets a state code associated with the specified state.
        /// </summary>
        /// <param name="state">
        /// The current state of the WebRelay module.
        /// </param>
        /// <returns>
        /// The integer value associated with the specified state.
        /// </returns>
        public static Int32 GetRelayStateCode(RelayState state) {
            Int32 code = RELAY_STATE_OFF;
            switch (state) {
                case RelayState.Off:
                    code = RELAY_STATE_OFF;
                    break;
                case RelayState.On:
                    code = RELAY_STATE_ON;
                    break;
                case RelayState.Pulse:
                    code = RELAY_STATE_PULSE_OR_REBOOT;
                    break;
                case RelayState.Reboot:
                    code = RELAY_STATE_PULSE_OR_REBOOT;
                    break;
                case RelayState.DisableAutoReboot:
                    code = RELAY_STATE_DISABLE_AUTO_REBOOT;
                    break;
                case RelayState.EnableAutoReboot:
                    code = RELAY_STATE_ENABLE_AUTO_REBOOT;
                    break;
                case RelayState.Toggle:
                    code = RELAY_STATE_TOGGLE;
                    break;
            }
            return code;
        }

        /// <summary>
        /// Enumerates the specified parent node and returns the named child
        /// node if found.
        /// </summary>
        /// <param name="parentNode">
        /// The parent node to enumerate.
        /// </param>
        /// <param name="name">
        /// The name of the child node to search for.
        /// </param>
        /// <returns>
        /// If successful, the located child node. If the parent node is null
        /// of the specified child node could not be located, then returns null.
        /// </returns>
        public static XmlNode GetNamedChildNode(XmlNode parentNode, String name) {
            if ((parentNode == null) || (String.IsNullOrEmpty(name))) {
                return null;
            }

            // If there are child nodes, search for a match.
            XmlNode childNode = null;
            if (parentNode.HasChildNodes) {
                foreach (XmlNode child in parentNode.ChildNodes) {
                    if (child.Name == name) {
                        childNode = child;
                        break;
                    }
                }
            }
            return childNode;
        }

        /// <summary>
        /// Trims extraneous null terminations from the XML response which
        /// invalidates the XML as a document. The the response from the
        /// device is not trimmed, it cannot be loaded or parsed by
        /// <see cref="System.Xml.XmlDocument"/>.
        /// </summary>
        /// <param name="input">
        /// The response message string to trim.
        /// </param>
        /// <returns>
        /// The trimmed version of the message. If a null or empty string is
        /// provided, an empty string is returned. If no null termination
        /// strings are found, the original string is returned as-is.
        /// </returns>
        public static String TrimResponse(String input) {
            if (String.IsNullOrEmpty(input)) {
                return String.Empty;
            }
            String trimStr = "\0";
            return input.TrimEnd(trimStr.ToCharArray()).Trim();
        }

        /// <summary>
        /// Gets the corresponding <see cref="RebootState"/> from the specified
        /// state code.
        /// </summary>
        /// <param name="stateCode">
        /// An integer value representing the reboot state of the WebRelay
        /// module.
        /// </param>
        /// <returns>
        /// If successful, the corresponding <see cref="RebootState"/>;
        /// Otherise, an exception is thrown.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid integer was specified. Must be a value 1 - 4.
        /// </exception>
        public static RebootState GetRebootState(Int32 stateCode) {
            if ((stateCode < 0) || (stateCode > 4)) {
                throw new ArgumentOutOfRangeException("stateCode", "Must be a value 0 - 4.");
            }

            RebootState state = RebootState.Pinging;
            switch (stateCode) {
                case REBOOT_STATE_AUTO_REBOOT_OFF:
                    state = RebootState.AutoRebootOff;
                    break;
                case REBOOT_STATE_PINGING:
                    state = RebootState.Pinging;
                    break;
                case REBOOT_STATE_REBOOTING:
                    state = RebootState.Rebooting;
                    break;
                case REBOOT_STATE_WAITING_FOR_BOOT:
                    state = RebootState.WaitingForBoot;
                    break;
                case REBOOT_STATE_WAITING_FOR_RESPONSE:
                    state = RebootState.WaitingForResponse;
                    break;
                default:
                    break;
            }
            return state;
        }

        /// <summary>
        /// Gets the integer value of the specified <see cref="RebootState"/>.
        /// </summary>
        /// <param name="state">
        /// The state to convert into an integer.
        /// </param>
        /// <returns>
        /// The integer value of the specified state.
        /// </returns>
        public static Int32 GetRebootStateCode(RebootState state) {
            Int32 code = REBOOT_STATE_PINGING;
            switch (state) {
                case RebootState.AutoRebootOff:
                    code = REBOOT_STATE_AUTO_REBOOT_OFF;
                    break;
                case RebootState.Pinging:
                    code = REBOOT_STATE_PINGING;
                    break;
                case RebootState.Rebooting:
                    code = REBOOT_STATE_REBOOTING;
                    break;
                case RebootState.WaitingForBoot:
                    code = REBOOT_STATE_WAITING_FOR_BOOT;
                    break;
                case RebootState.WaitingForResponse:
                    code = REBOOT_STATE_WAITING_FOR_RESPONSE;
                    break;
                default:
                    break;
            }
            return code;
        }

        /// <summary>
        /// Gets the corresponding <see cref="PowerUpFlag"/> from the specified
        /// flag code.
        /// </summary>
        /// <param name="value">
        /// An integer value representing a power up flag state. Should be either
        /// <see cref="Common.POWERUP_FLAG_OFF"/> or <see cref="Common.POWERUP_FLAG_ON"/>.
        /// </param>
        /// <returns>
        /// The corresponding flag.
        /// </returns>
        public static PowerUpFlag GetPowerUpFlag(Int32 value) {
            if (value == POWERUP_FLAG_OFF) {
                return PowerUpFlag.Off;
            }
            return PowerUpFlag.On;
        }

        /// <summary>
        /// Gets the integer value of the specified <see cref="PowerUpFlag"/>.
        /// </summary>
        /// <param name="flag">
        /// The flag to convert into an integer.
        /// </param>
        /// <returns>
        /// Then integer value of the specified flag.
        /// </returns>
        public static Int32 GetPowerUpFlagCode(PowerUpFlag flag) {
            if (flag == PowerUpFlag.Off) {
                return POWERUP_FLAG_OFF;
            }
            return POWERUP_FLAG_ON;
        }

        /// <summary>
        /// Gets an alarm condition from the specified value.
        /// </summary>
        /// <param name="input">
        /// The value to get the associated alarm condition from.
        /// </param>
        /// <returns>
        /// The alarm condition associated with the specified value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Value must be of one of the alarm condition constants.
        /// </exception>
        public static AlarmConditions GetAlarmCondition(Int32 input) {
            if ((input < ALARM_COND_NORMAL) ||
                (input > ALARM_COND_LOW)) {
                String err = String.Format("Must be a value {0} - {1}.",
                                            ALARM_COND_NORMAL.ToString(),
                                            ALARM_COND_LOW.ToString());
                throw new ArgumentOutOfRangeException("input", err);
            }

            AlarmConditions condition = AlarmConditions.Normal;
            switch (input) {
                case ALARM_COND_NORMAL:
                    break;
                case ALARM_COND_HIGH:
                    condition = AlarmConditions.High;
                    break;
                case ALARM_COND_LOW:
                    condition = AlarmConditions.Low;
                    break;
            }
            return condition;
        }

        /// <summary>
        /// Gets an alarm code from the specified condition.
        /// </summary>
        /// <param name="condition">
        /// The condition to get the code of.
        /// </param>
        /// <returns>
        /// The corresponding alarm code.
        /// </returns>
        public static Int32 GetAlarmCode(AlarmConditions condition) {
            Int32 retCond = ALARM_COND_NORMAL;
            switch (condition) {
                case AlarmConditions.Normal:
                    break;
                case AlarmConditions.High:
                    retCond = ALARM_COND_HIGH;
                    break;
                case AlarmConditions.Low:
                    retCond = ALARM_COND_LOW;
                    break;
            }
            return retCond;
        }
        #endregion
    }
}
