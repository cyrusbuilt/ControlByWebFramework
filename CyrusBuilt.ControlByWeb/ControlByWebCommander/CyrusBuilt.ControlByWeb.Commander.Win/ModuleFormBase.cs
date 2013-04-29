using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    /// <summary>
    /// A base module form class.
    /// </summary>
    public partial class ModuleFormBase : Form
    {
        #region Fields
        private Boolean _isPolling = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.ModuleFormBase</b>
        /// class. This is the default constructor.
        /// </summary>
        public ModuleFormBase() {
            // We don't need (or want) this default ctor for our implementation, but it is required
            // for the VS form designer.
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Commander.Win.ModuleFormBase</b>
        /// class with the parent form.
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
        public ModuleFormBase(Form parent) {
            if (parent == null) {
                throw new ArgumentNullException("parent");
            }

            if (!parent.IsMdiContainer) {
                throw new ArgumentException("The specified parent form is not an MDI container.", "parent");
            }
            InitializeComponent();
            this.MdiParent = parent;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Determines whether or not this module is currently polling status.
        /// </summary>
        public Boolean IsPolling {
            get { return this._isPolling; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Aborts a status poll cycle.
        /// </summary>
        protected virtual void AbortPoll() {
            this._isPolling = false;
        }

        /// <summary>
        /// Starts a status poll cycle.
        /// </summary>
        protected virtual void StartPoll() {
            this._isPolling = true;
        }
        #endregion
    }
}
