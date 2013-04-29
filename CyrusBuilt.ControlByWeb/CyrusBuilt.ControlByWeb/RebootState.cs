using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// Provides possible states of the automatic reboot controller.
    /// </summary>
    public enum RebootState
    {
        /// <summary>
        /// This indicates that the user has turned the auto reboot feature off.
        /// Note that the auto reboot feature will automatically be re-enabled if
        /// power is lost and restored to the WebRelay(tm).
        /// </summary>
        AutoRebootOff,

        /// <summary>
        /// The WebRelay(tm) is periodically pinging the device. This indicates
        /// normal operation.
        /// </summary>
        Pinging,

        /// <summary>
        /// This indicates that a ping request has been sent out and WebRelay(tm)
        /// is waiting for a reply from the device.  Under normal circumstances,
        /// the device reply very quickly and there will be no time for this
        /// state to be used.
        /// </summary>
        WaitingForResponse,

        /// <summary>
        /// The WebRelay(tm) is currently performing a reboot. This may be a
        /// single pulse or two pulses with a delay between them depending upon
        /// how the WebRelay(tm) is configured.
        /// </summary>
        Rebooting,

        /// <summary>
        /// After power-up, a WebRelay(tm) will wait for the device to boot
        /// before sending any ping requests.
        /// </summary>
        WaitingForBoot
    }
}
