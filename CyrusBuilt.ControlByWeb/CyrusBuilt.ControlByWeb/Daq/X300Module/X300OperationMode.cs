using System;

namespace CyrusBuilt.ControlByWeb.Daq.X300Module
{
    /// <summary>
    /// Operation modes for the X-300 module.
    /// </summary>
    public enum X300OperationMode
    {
        /// <summary>
        /// The device will operate like a high-end digital thermostat.
        /// </summary>
        Thermostat,

        /// <summary>
        /// The module will act more like a thermometer, used mostly to monitor
        /// temperatures and monitor/control relay states.
        /// </summary>
        TemperatureMonitor
    }
}
