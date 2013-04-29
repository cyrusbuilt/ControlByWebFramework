using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    /// <summary>
    /// A custom web browser-like component used to directly access the web
    /// interface on the device itself. All ControlByWeb devices have a small
    /// web server running on them that hosts pages used for monitoring and
    /// controlling them.
    /// </summary>
    public partial class DeviceViewer : Form
    {
        #region Fields
        private IPEndPoint _endpoint = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.DeviceViewer</b>
        /// class. This is the default constructor.
        /// </summary>
        public DeviceViewer() {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.DeviceViewer</b>
        /// class with the endpoint to browse to.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint to browse to. If this is null, an empty window will
        /// appear when shown.
        /// </param>
        public DeviceViewer(IPEndPoint endpoint) {
            InitializeComponent();
            this._endpoint = endpoint;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the endpoint this component connects to.
        /// </summary>
        public IPEndPoint Endpoint {
            get { return this._endpoint; }
        }

        /// <summary>
        /// Gets the URL this component connects to. If <see cref="Endpoint"/>
        /// is null, this will return an empty string.
        /// </summary>
        public String URL {
            get {
                if (this._endpoint == null) {
                    return String.Empty;
                }
                String addr = this._endpoint.Address.ToString();
                String port = this._endpoint.Port.ToString();
                return String.Format("http://{0}:{1}/", addr, port);
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Registers the web browser component events we need.
        /// </summary>
        private void RegisterBrowserEvents() {
            if (!this.webBrowserView.IsDisposed) {
                this.webBrowserView.Navigated += new WebBrowserNavigatedEventHandler(WebBrowserView_Navigated);
                this.webBrowserView.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowserView_DocumentCompleted);
            }
        }

        /// <summary>
        /// Unregisters the web browser component events registered by <see cref="RegisterBrowserEvents"/>.
        /// </summary>
        private void UnRegisterBrowserEvents() {
            if (!this.webBrowserView.IsDisposed) {
                this.webBrowserView.Navigated -= new WebBrowserNavigatedEventHandler(WebBrowserView_Navigated);
                this.webBrowserView.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(WebBrowserView_DocumentCompleted);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the form load event. This sets the title bar caption and
        /// registers web browser events.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void DeviceViewer_Load(Object sender, EventArgs e) {
            this.Text = String.Format("Viewing device at: {0}", this.URL);
            this.RegisterBrowserEvents();
        }

        /// <summary>
        /// Handles the shown event. The shown event fires only once after the
        /// form is loaded, validated, and displayed. This handler automatically
        /// navigates to <see cref="URL"/> asynchronously (if defined).
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void DeviceViewer_Shown(Object sender, EventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                if (String.IsNullOrEmpty(this.URL)) {
                    return;
                }
                this.webBrowserView.Navigate(new Uri(this.URL));
            });
        }

        /// <summary>
        /// Handles the form closing event. This stops document loading and
        /// browser navigation if the web browser component is busy, and then
        /// disposes the web browser.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void DeviceViewer_FormClosing(Object sender, FormClosingEventArgs e) {
            this.UnRegisterBrowserEvents();
            if (!this.webBrowserView.IsDisposed) {
                if (this.webBrowserView.IsBusy) {
                    this.webBrowserView.Stop();
                }
                // Not sure if this is called by the form's dispose method or not,
                // but the webbrowser control is resource intensive, so just to
                // be sure..... (usually calling Dispose() more than once does
                // not hurt anything) to prevent memory leaks.
                this.webBrowserView.Dispose();
            }
        }

        /// <summary>
        /// Handles the web browser navigated event, which fires when the web
        /// browser control has navigated to a new document and has begun
        /// loading it. This displays the absolute URI path in the status bar.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void WebBrowserView_Navigated(Object sender, WebBrowserNavigatedEventArgs e) {
            this.toolStripStatusLabelPage.Text = String.Concat("Page:", e.Url.AbsoluteUri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">
        /// 
        /// </param>
        /// <param name="e">
        /// 
        /// </param>
        private void WebBrowserView_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e) {
            // TODO: Stop an animated status graphic or something?
        }

        /// <summary>
        /// Handles the back button (in the toolbar) click event. This tells
        /// the web browser to navigate to the previous page if it can.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonBack_Click(Object sender, EventArgs e) {
            if (this.webBrowserView.CanGoBack) {
                this.webBrowserView.GoBack();
            }
        }

        /// <summary>
        /// Handles the go/forward button (in the toolbar) click event. This
        /// tells the web browser to navigate to the next page if it can.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonGo_Click(Object sender, EventArgs e) {
            if (this.webBrowserView.CanGoForward) {
                this.webBrowserView.GoForward();
            }
        }

        /// <summary>
        /// Handles the refresh button (in the toolbar) button click event.
        /// This tells the web browser control to refresh the current page.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonRefresh_Click(Object sender, EventArgs e) {
            // TODO: Maybe do something like this:
            // this.BeginInvoke((MethodInvoker) this.webBrowserView.Refresh);
            // - or -
            // this.BeginInvoke((MethodInvoker) delegate {
            //      this.webBrowserView.Refresh();
            // });
            // Not sure if this would benifit performance-wise by making an
            // asynchronous call using an anonymous delegate.
            this.webBrowserView.Refresh();
        }

        /// <summary>
        /// Stop button (in the toolbar) button click event. This tells the web
        /// browser component to stop loading the current document or
        /// navigating to another one.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonStop_Click(Object sender, EventArgs e) {
            if (this.webBrowserView.IsBusy) {
                this.webBrowserView.Stop();
            }
        }

        /// <summary>
        /// Handles the print button (in the toolbar) button click event. This
        /// tells the web browser component to display its print dialog.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonPrint_Click(Object sender, EventArgs e) {
            this.webBrowserView.ShowPrintDialog();
        }

        /// <summary>
        /// Handles the print preview button (in the toolbar) click event. This
        /// tells the web browser control to display its print preview dialog.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonPreview_Click(Object sender, EventArgs e) {
            this.webBrowserView.ShowPrintPreviewDialog();
        }

        /// <summary>
        /// Handles the page setup button (in the toolbar) click event. This
        /// tells the web browser control to display its page setup dialog.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void toolStripButtonLayout_Click(Object sender, EventArgs e) {
            this.webBrowserView.ShowPageSetupDialog();
        }
        #endregion
    }
}
