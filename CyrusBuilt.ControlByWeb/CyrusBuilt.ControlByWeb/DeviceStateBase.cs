using System;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Base class for all device state classes. This class contains no real
    /// logic of its own and is meant to be derived from. This is a base
    /// implementation if the IDeviceState interface.
    /// </summary>
    public abstract class DeviceStateBase : IDeviceState<DeviceStateBase>
    {
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.DeviceStateBase</b>
        /// class. This is the default constructor.
        /// </summary>
        protected DeviceStateBase() {
        }

        /// <summary>
        /// In a derived class, this method is used to reset the state back to
        /// its default values. This method should be overriden in the derived
        /// class.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// In a derived class, this method clones this state instance to a new
        /// duplicate instance using a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        protected DeviceStateBase IDeviceState<DeviceStateBase>.Clone() {
            return this;
        }

        /// <summary>
        /// In a derived class, this method gets the specified relay.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number (ID) to get.
        /// </param>
        /// <returns>
        /// If successful, a reference to the specified relay instance;
        /// Otherwise, null.
        /// </returns>
        public abstract Relay GetRelay(Int32 relayNum);
    }
}
