using System;
using System.Net;
using System.Xml;
using CyrusBuilt.ControlByWeb.Events;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb.WebRelay
{
    public class WebRelay10PlusController : ModuleBase
    {
        public WebRelay10PlusController()
            : base() {
        }

        public WebRelay10PlusController(IPAddress ipAddr)
            : base(ipAddr) {
        }

        public WebRelay10PlusController(IPAddress ipAddr, Int32 port)
            : base(ipAddr, port) {
        }

        public WebRelay10PlusController(IPEndPoint endpoint)
            : base(endpoint) {
        }

        private void Connect() {
            base.Connect();
        }

        private void Disconnect() {
            base.Disconnect();
        }

        private XmlDocument SendCommand(String command) {
            return base.SendCommand(command);
        }

        private override WebRelay10PlusState GetStateFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            WebRelay10PlusState state = new WebRelay10PlusState();
            String itemText = String.Empty;
            String attrib = String.Empty;
            Double highTime = 0.0;
            Int32 tempState = 0;
            Int32 tempCount = 0;
            Int32 count = 0;
            RelayState relState = RelayState.Off;
            Double temp = 0.0;
            Double extVar = 0.0;
            InputState inState = InputState.Off;
            SensorInput sensor = null;
            Int64 time = 0;

            // Standard inputs.
            for (Int32 i = 1; i <= WebRelay10Constants.TOTAL_STANDARD_INPUTS; i++) {
                // State.
                tempState = Common.INPUT_STATE_OFF;
                inState = InputState.Off;
                attrib = String.Format("input{0}state", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out tempState)) {
                        if (tempState == Common.INPUT_STATE_ON) {
                            inState = InputState.On;
                        }
                    }
                }

                // Count.
                tempCount = 0;
                count = 0;
                attrib = String.Format("count{0}", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out tempCount)) {
                        count = tempCount;
                    }
                }

                // High time.
                temp = 0.000;
                highTime = 0.000;
                attrib = String.Format("hightime{0}", i.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out temp)) {
                        highTime = temp;
                    }
                }

                state.SetOrOverrideInput(i, new StandardInput(inState, count));
                state.SetOrOverrideHighTime(i, highTime);
            }

            // Relays.
            for (Int32 r = 1; r <= WebRelay10Constants.TOTAL_RELAYS; r++) {
                // State.
                relState = RelayState.Off;
                tempState = Common.RELAY_STATE_OFF;
                attrib = String.Format("relay{0}state", r.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Int32.TryParse(itemText, out tempState)) {
                        relState = Common.GetRelayState(tempState);
                    }
                }
                state.SetRelay(r, new Relay(relState));
            }

            // Units.
            itemText = Common.GetNamedChildNode(node, "units").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                // Default is farenheit.
                if (itemText.Trim().ToLower() == "C") {
                    state.SetOrOverrideTempUnits(TemperatureUnits.Celcius);
                }
            }

            // Sensors.
            for (Int32 s = 1; s <= WebRelay10Constants.TOTAL_SENSOR_INPUTS; s++) {
                temp = 0.0;
                sensor = null;
                attrib = String.Format("sensor{0}temp", s.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    sensor = new SensorInput();
                    if (Double.TryParse(itemText, out temp)) {
                        sensor.AddSensor(temp);
                    }
                    state.SetOrOverrideSensor(s, sensor);
                }
            }

            // External vars.
            for (Int32 e = WebRelay10Constants.EXT_VAR_MIN_ID; e <= WebRelay10Constants.EXT_VAR_MAX_ID; e++) {
                extVar = 0.0;
                attrib = String.Format("extvar{0}", e.ToString());
                itemText = Common.GetNamedChildNode(node, attrib).InnerText;
                if (!String.IsNullOrEmpty(itemText)) {
                    if (Double.TryParse(itemText, out extVar)) {
                        state.SetOrOverrideExternalVar(e, extVar);
                    }
                }
            }

            // Serial.
            itemText = Common.GetNamedChildNode(node, "serialNumber").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                try {
                    state.SetOrOverrideSerial(itemText);
                }
                catch (FormatException) {
                    // Ignore. Serial will just be null, which is preferable.
                }
            }

            // Time.
            itemText = Common.GetNamedChildNode(node, "time").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int64.TryParse(itemText, out time)) {
                    state.SetOrOverrideTime(time);
                }
            }
            return state;
        }

        private override WebRelay10PlusEvent GetEventFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            WebRelay10PlusEvent evt = new WebRelay10PlusEvent();
            String itemText = String.Empty;
            Int32 value = 0;
            DateTime time = DateTime.MinValue;
            Double duration = 0.0;
            WebRelay10Action action = WebRelay10Action.None;

            // TODO need to test this somehow.
            // Get the ID of the event.  We'll have to go up one level and parse
            // it from the parent node. <event0> or <event1> for example.
            String parentName = node.ParentNode.Name;
            if (parentName.Contains("event")) {
                Int32 startPos = parentName.IndexOf("event");
                itemText = parentName.Substring((startPos + 5), 1);
                if (Int32.TryParse(itemText, out value)) {
                    evt.SetOrOverrideId(value);
                }
            }
            else {
                // TODO Return null or throw exception?
            }

            // Get whether or not this event is active.
            itemText = Common.GetNamedChildNode(node, "active").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (itemText == "yes") {
                    evt.SetOrOverrideActiveState(true);
                }
            }

            // Get the current time as reported by the device.
            itemText = Common.GetNamedChildNode(node, "currentTime").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (DateTime.TryParse(itemText, out time)) {
                    evt.SetOrOverrideCurrentTime(time);
                }
            }

            // Get the time of the next event occurrance.
            itemText = Common.GetNamedChildNode(node, "nextEvent").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (DateTime.TryParse(itemText, out time)) {
                    evt.SetOrOverrideNextEvent(time);
                }
            }

            // Get the period of time between event occurrences (only for
            // recurring events).
            itemText = Common.GetNamedChildNode(node, "period").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                evt.SetOrOverridePeriod(itemText);
            }

            // Gets the count of remaining times this event will occur.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "count").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    evt.SetOrOverrideCount(value);
                }
            }

            // Get the relay of this event.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "relay").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    if ((value > 0) && (value < (WebRelay10Constants.TOTAL_RELAYS + 1))) {
                        evt.SetOrOverrideRelay(value);
                    }
                }
            }

            // Get action of this event.
            itemText = Common.GetNamedChildNode(node, "action").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                action = WebRelay10Utils.GetActionFromString(itemText);
                evt.SetOrOverrideAction(action);
            }

            // Get the pulse duration.
            itemText = Common.GetNamedChildNode(node, "pulseDuration").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Double.TryParse(itemText, out duration)) {
                    evt.SetOrOverridePulseDuration(duration);
                }
            }
            return evt;
        }

        private override WebRelay10PlusDiagnostics GetDiagsFromXmlNode(XmlNode node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            // Memory power up flag.
            WebRelay10PlusDiagnostics diags = new WebRelay10PlusDiagnostics();
            Int32 value = 0;
            String itemText = Common.GetNamedChildNode(node, "memoryPowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    diags.SetMemoryPowerUpFlag(Common.GetPowerUpFlag(value));
                }
            }

            // Device power up flag.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "devicePowerUpFlag").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    diags.SetDevicePowerUpFlag(Common.GetPowerUpFlag(value));
                }
            }

            // Power loss count.
            value = 0;
            itemText = Common.GetNamedChildNode(node, "powerLossCounter").InnerText;
            if (!String.IsNullOrEmpty(itemText)) {
                if (Int32.TryParse(itemText, out value)) {
                    if (value > 0) {
                        diags.SetPowerLossCount(value);
                    }
                }
            }
            return diags;
        }

        public WebRelay10PlusDiagnostics GetDiagnostics() {
            try {
                XmlNode node = base.GetDiagnostics();
                return this.GetDiagsFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public WebRelay10PlusEvent GetEvent(Int32 eventId) {
            try {
                XmlNode node = base.GetEvent(eventId);
                return this.GetEventFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public EventCollection GetEvents() {
            WebRelay10PlusEvent evt = null;
            EventCollection eventColl = new EventCollection();
            for (Int32 i = EventConstants.EVENT_MIN_ID; i <= EventConstants.EVENT_MAX_ID; i++) {
                evt = this.GetEvent(i);
                if (evt != null) {
                    eventColl.Add(evt);
                }
            }
            evt = null;
            return eventColl;
        }

        public WebRelay10PlusState GetState() {
            try {
                XmlNode node = base.GetState();
                return this.GetStateFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public void PulseRelay(Int32 relayNum, Double pulseTime) {
            base.PulseRelay(relayNum, pulseTime, WebRelay10Constants.TOTAL_RELAYS);
        }

        public void PulseRelay(Int32 relayNum) {
            base.PulseRelay(relayNum, WebRelay10Constants.TOTAL_RELAYS);
        }

        public WebRelay10PlusState ChangeRelayState(Int32 relayNum, RelayState state) {
            try {
                XmlNode node = base.ChangeRelayState(relayNum, state, WebRelay10Constants.TOTAL_RELAYS);
                return this.GetStateFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public WebRelay10PlusState SwitchRelayOn(Int32 relayNum) {
            try {
                XmlNode node = base.SwitchRelayOn(relayNum, WebRelay10Constants.TOTAL_RELAYS);
                return this.GetStateFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public WebRelay10PlusState SwitchRelayOff(Int32 relayNum) {
            try {
                XmlNode node = base.SwitchRelayOff(relayNum, WebRelay10Constants.TOTAL_RELAYS);
                return this.GetStateFromXmlNode(node);
            }
            catch (ArgumentNullException) {
                throw new BadResponseFromDeviceException();
            }
        }

        public void ClearPowerLossCounter() {
            base.ClearPowerLossCounter();
        }

        public void ClearMemPowerUpFlag() {
            base.ClearMemPowerUpFlag();
        }

        public void ClearDevicePowerUpFlag() {
            base.ClearDevicePowerUpFlag();
        }

        public void ClearPowerUpFlags() {
            base.ClearPowerUpFlags();
        }
    }
}
