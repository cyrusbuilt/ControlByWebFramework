using System;
using CyrusBuilt.ControlByWeb.Relays;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Represents the state of a ControlByWeb device.
    /// </summary>
    public interface IDeviceState<T> : IResetable
    {
        /// <summary>
        /// Clones this state instance to a new duplicate instance using a deep
        /// copy of the current instance.
        /// </summary>
        /// <returns>
        /// A duplicate (but separate) copy of this state instance.
        /// </returns>
        T Clone();

        /// <summary>
        /// Gets the specified relay.
        /// </summary>
        /// <param name="relayNum">
        /// The relay number (ID) to get.
        /// </param>
        /// <returns>
        /// If successful, a reference to the specified relay instance;
        /// Otherwise, null.
        /// </returns>
        Relay GetRelay(Int32 relayNum);
    }
}
