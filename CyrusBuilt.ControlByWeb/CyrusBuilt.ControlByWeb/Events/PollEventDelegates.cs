using System;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// Handler delegate for poll failure events.
    /// </summary>
    /// <param name="sender">
    /// The object sending the event call.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    public delegate void PollFailEventHandler(Object sender, PollFailedEventArgs e);

    /// <summary>
    /// Handler delegate for poll status events.
    /// </summary>
    /// <param name="sender">
    /// The object sending the event call.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    public delegate void PollStatusEventHandler(Object sender, PollStatusEventArgs e);
}
