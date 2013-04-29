using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    /// <summary>
    /// Used to display the raw response from a device.
    /// </summary>
    public partial class RawResponseForm : Form
    {
        #region Fields
        private String _response = String.Empty;
        private IPEndPoint _endpoint = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.RawResponseForm</b>
        /// class. This is the default constructor.
        /// </summary>
        public RawResponseForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.RawResponseForm</b>
        /// class with the response from the device.
        /// </summary>
        /// <param name="response">
        /// The response from the device.
        /// </param>
        public RawResponseForm(String response) {
            this.InitializeComponent();
            this._response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.RawResponseForm</b>
        /// class with the response from the device and the IP endpoint of the
        /// device the response came from.
        /// </summary>
        /// <param name="response">
        /// The response from the device.
        /// </param>
        /// <param name="endpoint">
        /// The IP endpoint of the device the response came from.
        /// </param>
        public RawResponseForm(String response, IPEndPoint endpoint) {
            this.InitializeComponent();
            this._response = response;
            this._endpoint = endpoint;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the form load event. This sets the title bar caption and
        /// populates the contents of the textbox with the response from the
        /// device.
        /// </summary>
        /// <param name="sender">
        /// The object that made the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void RawResponseForm_Load(Object sender, EventArgs e) {
            this.Text = "Raw Device Response";
            if (this._endpoint != null) {
                String addr = this._endpoint.Address.ToString();
                String port = this._endpoint.Port.ToString();
                this.Text = String.Format("Response from: {0}:{1}", addr, port);
            }
            this.SuspendLayout();
            this.textBoxResponse.Text = this._response;
            this.textBoxResponse.SelectionStart = this.textBoxResponse.Text.Length;
            this.textBoxResponse.ScrollToCaret();
            this.ResumeLayout();
        }
        #endregion
    }
}
