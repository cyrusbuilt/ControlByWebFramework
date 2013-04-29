using CyrusBuilt.ControlByWeb;
using CyrusBuilt.ControlByWeb.Security;
using CyrusBuilt.ControlByWeb.Inputs;
using CyrusBuilt.ControlByWeb.Relays;
using CyrusBuilt.ControlByWeb.WebRelay;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    /// <summary>
    /// A front-end to the WebRelay controller module.
    /// </summary>
    public partial class WebRelayControllerForm : ModuleFormBase
    {
        // TODO: Possibly refactor modules like this to be implemented as plugins.
        // TODO: Will need a plugin architecture implemented by both this host
        // app and the plugin modules to accomplish this.

        #region Fields
        private WebRelayController _controller = null;
        #endregion

        #region Delegates
        private delegate void UpdateTextBoxCallback(TextBox tb, String text);
        private delegate void UpdateTitleCallback(String text);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.WebRelayControllerForm</b>
        /// class with the parent form that is the container for this form.
        /// </summary>
        /// <param name="parent">
        /// The parent form that is the container for this form.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parent"/> cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="parent"/> MUST be an MDI container.
        /// </exception>
        public WebRelayControllerForm(Form parent) 
            : base(parent) {
            this.InitializeComponent();
            this._controller = new WebRelayController();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Determines whether or not the device controller is operational.
        /// </summary>
        /// <returns>
        /// true if operational; Otherwise, false.
        /// </returns>
        private Boolean IsControllerOperable() {
            if ((this._controller != null) && (!this._controller.IsDisposed)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers device controller events.
        /// </summary>
        private void RegisterControllerEvents() {
            if (this.IsControllerOperable()) {
                this._controller.Polled += new WebRelayPollEventHandler(this.Controller_Polled);
                this._controller.PollFailed += new WebRelayPollFailedEventHandler(this.Controller_PollFailed);
            }
        }

        /// <summary>
        /// Unregisters device controller events.
        /// </summary>
        private void UnRegisterControllerEvents() {
            if (this.IsControllerOperable()) {
                this._controller.Polled -= new WebRelayPollEventHandler(this.Controller_Polled);
                this._controller.PollFailed -= new WebRelayPollFailedEventHandler(this.Controller_PollFailed);
            }
        }

        /// <summary>
        /// Thread-safe method for updating the text in the specified TextBox
        /// control.
        /// </summary>
        /// <param name="tb">
        /// The TextBox control to update.
        /// </param>
        /// <param name="text">
        /// The text to update the textbox with.
        /// </param>
        private void DoTextUpdate(TextBox tb, String text) {
            if ((tb == null) || (String.IsNullOrEmpty(text))) {
                return;
            }

            if (tb.InvokeRequired) {
                UpdateTextBoxCallback d = new UpdateTextBoxCallback(this.DoTextUpdate);
                tb.Invoke(d, new Object[] { tb, text });
            }
            else {
                tb.Text = text;
            }
        }

        /// <summary>
        /// Thread-safe method for updating the title bar text.
        /// </summary>
        /// <param name="text">
        /// The text to set.
        /// </param>
        private void DoUpdateTitle(String text) {
            if (String.IsNullOrEmpty(text)) {
                return;
            }

            if (this.InvokeRequired) {
                UpdateTitleCallback d = new UpdateTitleCallback(this.DoUpdateTitle);
                this.Invoke(d, new Object[] { text });
            }
            else {
                this.Text = text;
            }
        }

        /// <summary>
        /// Initializes the error provider.
        /// </summary>
        private void InitErrorProvider() {
            this.errorProviderMain.Clear();
            this.errorProviderMain.SetError(this.textBoxInterval, String.Empty);
            this.errorProviderMain.SetError(this.textBoxAddress, String.Empty);
            this.errorProviderMain.SetError(this.textBoxPort, String.Empty);
            this.errorProviderMain.SetError(this.textBoxPulseTime, String.Empty);
        }

        /// <summary>
        /// Checks the input required to connect to the device and marks the
        /// fields that are in error.
        /// </summary>
        /// <returns>
        /// true if preflight passed; Otherwise, false.
        /// </returns>
        private Boolean Preflight() {
            this.InitErrorProvider();
            if (String.IsNullOrEmpty(this.textBoxAddress.Text)) {
                this.errorProviderMain.SetError(this.textBoxAddress, "Specify an IP address.");
                this.textBoxAddress.Select();
                return false;
            }

            IPAddress addr = null;
            if (!IPAddress.TryParse(this.textBoxAddress.Text.Trim(), out addr)) {
                this.errorProviderMain.SetError(this.textBoxAddress, "Invalid address.");
                this.textBoxAddress.Select();
                return false;
            }

            Int32 port = Common.DEFAULT_PORT;
            if (!String.IsNullOrEmpty(this.textBoxPort.Text)) {
                if (!Int32.TryParse(this.textBoxPort.Text.Trim(), out port)) {
                    this.errorProviderMain.SetError(this.textBoxPort, "Invalid port.");
                    this.textBoxPort.Select();
                    return false;
                }
            }

            if (this.checkBoxPassword.Checked) {
                if (String.IsNullOrEmpty(this.textBoxPassword.Text)) {
                    this.errorProviderMain.SetError(this.textBoxPassword, "Specify a password.");
                    this.textBoxPassword.Select();
                    return false;
                }
                this._controller.BasicAuthEnabled = true;
                this._controller.Password = Crypto.ConvertToSecureString(this.textBoxPassword.Text);
            }

            this._controller.ModuleAddress = addr;
            this._controller.Port = port;
            return true;
        }

        /// <summary>
        /// A specialized exception handler. This handles the number of
        /// exceptions thrown by many of the methods of the controller. Rather
        /// than repeating the same series of catch() { } blocks, we just catch
        /// "Exception" and then pass that exception to this method to be
        /// handled. This method determines the type of exception, an then
        /// handles it appropriately.
        /// </summary>
        /// <param name="ex">
        /// The exception to handle.
        /// </param>
        private void HandleException(Exception ex) {
            if (ex == null) {
                return;
            }

            var socEx = ex as SocketException;
            if (socEx != null) {
                if (MessageBox.Show("A socket error occurred.",
                                    "Socket Error",
                                    MessageBoxButtons.RetryCancel,
                                    MessageBoxIcon.Error) == DialogResult.Retry) {
                    this.buttonRefresh.PerformClick();
                }
                return;
            }

            var cfEx = ex as ConnectionFailedException;
            if (cfEx != null) {
                MessageBox.Show(cfEx.Message, 
                                "Connection Failed",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
                return;
            }

            var ioEx = ex as IOException;
            if (ioEx != null) {
                MessageBox.Show("Could not read/write the underlying network stream.", 
                                "I/O Error",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
                return;
            }

            var uaEx = ex as UnauthorizedAccessException;
            if (uaEx != null) {
                MessageBox.Show(uaEx.Message, 
                                "Access Denied",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Exclamation);
                return;
            }

            var respEx = ex as BadResponseFromDeviceException;
            if (respEx != null) {
                if (MessageBox.Show("Bad response from device.\r\n\r\nWould you like to see the response?",
                                    "Bad Response",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                    IPEndPoint endpoint = new IPEndPoint(this._controller.ModuleAddress, this._controller.Port);
                    using (RawResponseForm resp = new RawResponseForm(respEx.Response, endpoint)) {
                        resp.ShowDialog();
                    }
                    endpoint = null;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Aborts a currently running status poll cycle.
        /// </summary>
        protected override void AbortPoll() {
            if (base.IsPolling) {
                if (this.IsControllerOperable()) {
                    this._controller.EndPollCycle();
                }
                base.AbortPoll();
            }
        }

        /// <summary>
        /// Starts a status poll cycle.
        /// </summary>
        protected override void StartPoll() {
            if (!base.IsPolling) {
                if (this.IsControllerOperable()) {
                    this._controller.BeginPollCycle();
                }
                base.StartPoll();
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the controller polled event. Updates the appropriate
        /// controls with device status information.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void Controller_Polled(Object sender, WebRelayPollEventArgs e) {
            if (e.State == null) {
                return;
            }
            this.DoUpdateTitle("WebRelay Controller: Polling...");
            String relState = Enum.GetName(typeof(RelayState), e.State.Relay.State);
            String inpState = Enum.GetName(typeof(InputState), e.State.InputState);
            String rebState = Enum.GetName(typeof(RebootState), e.State.RebootState);
            String reboots = e.State.TotalReboots.ToString();
            this.SuspendLayout();
            this.DoTextUpdate(this.textBoxRelState, relState);
            this.DoTextUpdate(this.textBoxInpState, inpState);
            this.DoTextUpdate(this.textBoxRebState, rebState);
            this.DoTextUpdate(this.textBoxTotalRebs, reboots);
            this.ResumeLayout();
        }

        /// <summary>
        /// Handles the poll failed event. Aborts the poll cycle and displays
        /// an error describing the poll failure.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void Controller_PollFailed(Object sender, WebRelayPollFailedEventArgs e) {
            this.AbortPoll();
            this.DoUpdateTitle("WebRelay Controller");
            if (e.FailureCause == null) {
                MessageBox.Show("Poll cycle failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            String err = String.Format("Failed to poll status: {0}", e.FailureCause.Message);
            MessageBox.Show(err, "Get Status Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Handles the form load event. Registers events for the device
        /// controller.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void WebRelayControllerForm_Load(Object sender, EventArgs e) {
            this.SuspendLayout();
            this.Text = "WebRelay Controller";
            this.RegisterControllerEvents();
            this.InitErrorProvider();
            this.textBoxPort.Text = Common.DEFAULT_PORT.ToString();
            this.textBoxPulseTime.Text = Common.DEFAULT_PULSE_TIME.ToString();
            this.textBoxAddress.Select();
            this.ResumeLayout();
        }

        /// <summary>
        /// Handles the form closing event. This aborts an active poll cycle
        /// and destroys the device controller.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void WebRelayControllerForm_FormClosing(Object sender, FormClosingEventArgs e) {
            // Abort the poll cycle if running.
            if (this.IsPolling) {
                this.AbortPoll();
            }

            // Unregister events and dispose the controller.
            this.UnRegisterControllerEvents();
            if (this.IsControllerOperable()) {
                this._controller.EndPollCycle();
                this._controller.Dispose();
            }
        }

        /// <summary>
        /// Handles the refresh button click event. This gets the state of the
        /// device and populates the appropriate fields with the data received.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonRefresh_Click(Object sender, EventArgs e) {
            if (this.IsControllerOperable()) {
                if (!this.Preflight()) {
                    return;
                }

                WebRelayState state = null;
                try {
                    state = this._controller.GetState();
                    if (state != null) {
                        this.SuspendLayout();
                        this.textBoxRelState.Text = Enum.GetName(typeof(RelayState), state.Relay.State);
                        this.textBoxInpState.Text = Enum.GetName(typeof(InputState), state.InputState);
                        this.textBoxRebState.Text = Enum.GetName(typeof(RebootState), state.RebootState);
                        this.textBoxTotalRebs.Text = state.TotalReboots.ToString();
                        this.ResumeLayout();
                    }
                }
                catch (Exception ex) {
                    this.HandleException(ex);
                }
                finally {
                    state = null;
                }
            }
        }

        /// <summary>
        /// Handles the "Go!" button click event. This checks the input required
        /// to poll status then starts the status poll cycle.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonGo_Click(Object sender, EventArgs e) {
            this.InitErrorProvider();
            Int32 interval = 0;
            if (!Int32.TryParse(this.textBoxInterval.Text.Trim(), out interval)) {
                this.errorProviderMain.SetError(this.textBoxInterval, "Invalid interval.");
                this.textBoxInterval.Select();
                return;
            }

            if (interval <= 0) {
                this.errorProviderMain.SetError(this.textBoxInterval, "Must be greater than zero.");
                this.textBoxInterval.Select();
                return;
            }

            if ((this.Preflight()) && (this.IsControllerOperable())) {
                this.buttonGo.Enabled = false;
                this.buttonStop.Enabled = true;
                this._controller.PollInterval = interval;
                this.StartPoll();
            }
        }

        /// <summary>
        /// Stops the status poll cycle.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonStop_Click(Object sender, EventArgs e) {
            this.SuspendLayout();
            this.buttonStop.Enabled = false;
            this.buttonGo.Enabled = true;
            this.DoUpdateTitle("WebRelay Controller");
            this.AbortPoll();
            this.ResumeLayout();
        }

        /// <summary>
        /// Enables/disables the controls needed for auto-refresh (poll cycle).
        /// If a poll cycle is currently running when the checkbox is unchecked
        /// then the current poll cycle will be cancelled.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void checkBoxAuto_CheckedChanged(Object sender, EventArgs e) {
            Boolean isChecked = this.checkBoxAuto.Checked;
            this.SuspendLayout();
            this.buttonRefresh.Enabled = !isChecked;
            this.textBoxInterval.Enabled = isChecked;
            this.buttonGo.Enabled = isChecked;
            this.buttonStop.Enabled = false;
            if (isChecked) {
                this.textBoxInterval.Select();
            }
            else {
                this.AbortPoll();
            }
            this.ResumeLayout();
        }

        /// <summary>
        /// Enables/disables the password field based on the state of the
        /// checkbox.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void checkBoxPassword_CheckedChanged(Object sender, EventArgs e) {
            this.textBoxPassword.Enabled = this.checkBoxPassword.Checked;
            if (this.textBoxPassword.Enabled) {
                this.textBoxPassword.Select();
            }
        }

        /// <summary>
        /// Clears the reboot counter on the device itself.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonClear_Click(Object sender, EventArgs e) {
            if ((this.Preflight()) && (this.IsControllerOperable())) {
                try {
                    this._controller.ClearRebootCounter();
                }
                catch (Exception ex) {
                    this.HandleException(ex);
                }
            }
            this.SuspendLayout();
            this.textBoxTotalRebs.Clear();
            this.textBoxTotalRebs.Text = "0";
            this.ResumeLayout();
        }

        /// <summary>
        /// Display the device viewer module and point it at the WebRelay device
        /// we are controlling.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonView_Click(Object sender, EventArgs e) {
            if (this.Preflight()) {
                IPAddress addr = this._controller.ModuleAddress;
                Int32 port = this._controller.Port;
                DeviceViewer viewer = new DeviceViewer(new IPEndPoint(addr, port));
                viewer.MdiParent = this.ParentForm;
                viewer.Show();
            }
        }

        /// <summary>
        /// Handles the mouse hover event for the view button control. This
        /// displays a tooltip for the button describing what it does. This
        /// is because the button itself only has an icon, and no text on it.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonView_MouseHover(Object sender, EventArgs e) {
            this.toolTipMain.SetToolTip(this.buttonView, "View device web interface.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonOn_Click(Object sender, EventArgs e) {
            if (this.IsControllerOperable()) {
                try {
                    this._controller.SwitchRelayOn();
                }
                catch (Exception ex) {
                    this.HandleException(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonOff_Click(Object sender, EventArgs e) {
            if (this.IsControllerOperable()) {
                try {
                    this._controller.SwitchRelayOff();
                }
                catch (Exception ex) {
                    this.HandleException(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonToggle_Click(Object sender, EventArgs e) {
            if (this.IsControllerOperable()) {
                try {
                    this._controller.ChangeRelayState(RelayState.Toggle);
                }
                catch (Exception ex) {
                    this.HandleException(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void buttonPulse_Click(Object sender, EventArgs e) {
            this.InitErrorProvider();
            Double pulse = Common.DEFAULT_PULSE_TIME;
            if (Double.TryParse(this.textBoxPulseTime.Text.Trim(), out pulse)) {
                if (this.IsControllerOperable()) {
                    try {
                        this._controller.PulseRelay(pulse);
                    }
                    catch (Exception ex) {
                        this.HandleException(ex);
                    }
                }
            }
            else {
                this.errorProviderMain.SetError(this.textBoxPulseTime, "Invalid pulse time.");
                this.textBoxPulseTime.Select();
            }
        }
        #endregion
    }
}
