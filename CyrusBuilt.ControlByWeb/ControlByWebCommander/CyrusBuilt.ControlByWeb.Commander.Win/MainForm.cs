using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    /// <summary>
    /// The main application form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.MainForm</b>
        /// class. This is the default constructor.
        /// </summary>
        public MainForm() {
            InitializeComponent();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Determines whether or not the main form has any MDI children.
        /// </summary>
        /// <returns>
        /// true if this form has childrem; Otherwise, false.
        /// </returns>
        private Boolean HasChildModules() {
            return ((this.MdiChildren != null) && (this.MdiChildren.Length > 0));
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the main form load. Sets the application title.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void MainForm_Load(Object sender, EventArgs e) {
            String name = Application.ProductName;
            String ver = Application.ProductVersion.ToString();
            this.Text = String.Format("{0} v{1}", name, ver);
        }

        /// <summary>
        /// Cascades all open child forms.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void cascadeToolStripMenuItem_Click(Object sender, EventArgs e) {
            if (this.HasChildModules()) {
                this.LayoutMdi(MdiLayout.Cascade);
            }
        }

        /// <summary>
        /// Tiles all open child forms horizontally.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void tileHorizontalToolStripMenuItem_Click(Object sender, EventArgs e) {
            if (this.HasChildModules()) {
                this.LayoutMdi(MdiLayout.TileHorizontal);
            }
        }

        /// <summary>
        /// Tiles all open child forms vertically.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void tileVerticalToolStripMenuItem_Click(Object sender, EventArgs e) {
            if (this.HasChildModules()) {
                this.LayoutMdi(MdiLayout.TileVertical);
            }
        }

        /// <summary>
        /// Iconizes all open child forms.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void arrangeIconsToolStripMenuItem_Click(Object sender, EventArgs e) {
            if (this.HasChildModules()) {
                this.LayoutMdi(MdiLayout.ArrangeIcons);
            }
        }

        /// <summary>
        /// Destroys all child forms.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void closeAllToolStripMenuItem_Click(Object sender, EventArgs e) {
            // Kill off all child forms.
            if (this.HasChildModules()) {
                foreach (Form child in this.MdiChildren) {
                    child.Close();
                    child.Dispose();
                }
            }
        }

        /// <summary>
        /// Handles the form closing event. Destroys all child forms.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e) {
            this.closeAllToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// Closes the main form (this) and thus, exits the application.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void exitToolStripMenuItem_Click(Object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// Launches and instance of the WebRelay module.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void webRelayModuleToolStripMenuItem_Click(Object sender, EventArgs e) {
            WebRelayControllerForm webRelay = new WebRelayControllerForm(this);
            webRelay.Show();
        }

        /// <summary>
        /// Handles the "Windows" menu click event. This will enable/disable
        /// all the items in the drop-down menu depending on whether or not
        /// there are any MDI child windows loaded.
        /// </summary>
        /// <param name="sender">
        /// The object sending the event call.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void windowToolStripMenuItem_Click(Object sender, EventArgs e) {
            this.SuspendLayout();
            ToolStripItemCollection items = this.windowToolStripMenuItem.DropDownItems;
            if (this.MdiChildren.Length == 0) {
                foreach (ToolStripItem menuItem in items) {
                    menuItem.Enabled = false;
                }
            }
            else {
                foreach (ToolStripItem menuItem in items) {
                    menuItem.Enabled = true;
                }
            }
            this.ResumeLayout();
        }
        #endregion
    }
}
