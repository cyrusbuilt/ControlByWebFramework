namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    partial class DeviceViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceViewer));
            this.toolStripNav = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLayout = new System.Windows.Forms.ToolStripButton();
            this.webBrowserView = new System.Windows.Forms.WebBrowser();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripNav.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripNav
            // 
            this.toolStripNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBack,
            this.toolStripButtonGo,
            this.toolStripButtonRefresh,
            this.toolStripButtonStop,
            this.toolStripButtonPrint,
            this.toolStripButtonPreview,
            this.toolStripButtonLayout});
            this.toolStripNav.Location = new System.Drawing.Point(0, 0);
            this.toolStripNav.Name = "toolStripNav";
            this.toolStripNav.Size = new System.Drawing.Size(668, 25);
            this.toolStripNav.TabIndex = 0;
            this.toolStripNav.Text = "toolStrip1";
            // 
            // toolStripButtonBack
            // 
            this.toolStripButtonBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBack.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.arrow_left;
            this.toolStripButtonBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBack.Name = "toolStripButtonBack";
            this.toolStripButtonBack.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBack.Text = "toolStripButton1";
            this.toolStripButtonBack.ToolTipText = "Back";
            this.toolStripButtonBack.Click += new System.EventHandler(this.toolStripButtonBack_Click);
            // 
            // toolStripButtonGo
            // 
            this.toolStripButtonGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGo.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.arrow_right;
            this.toolStripButtonGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGo.Name = "toolStripButtonGo";
            this.toolStripButtonGo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGo.Text = "toolStripButton1";
            this.toolStripButtonGo.ToolTipText = "Go (Forward)";
            this.toolStripButtonGo.Click += new System.EventHandler(this.toolStripButtonGo_Click);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.arrow_refresh_small;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "toolStripButton1";
            this.toolStripButtonRefresh.ToolTipText = "Refresh Page";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.cancel;
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStop.Text = "toolStripButton1";
            this.toolStripButtonStop.ToolTipText = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // toolStripButtonPrint
            // 
            this.toolStripButtonPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrint.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.printer;
            this.toolStripButtonPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrint.Name = "toolStripButtonPrint";
            this.toolStripButtonPrint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrint.Text = "toolStripButton1";
            this.toolStripButtonPrint.ToolTipText = "Print Page";
            this.toolStripButtonPrint.Click += new System.EventHandler(this.toolStripButtonPrint_Click);
            // 
            // toolStripButtonPreview
            // 
            this.toolStripButtonPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPreview.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.page_white_magnify;
            this.toolStripButtonPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPreview.Name = "toolStripButtonPreview";
            this.toolStripButtonPreview.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPreview.Text = "toolStripButton1";
            this.toolStripButtonPreview.ToolTipText = "Print Preview";
            this.toolStripButtonPreview.Click += new System.EventHandler(this.toolStripButtonPreview_Click);
            // 
            // toolStripButtonLayout
            // 
            this.toolStripButtonLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLayout.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.layout_edit;
            this.toolStripButtonLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLayout.Name = "toolStripButtonLayout";
            this.toolStripButtonLayout.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLayout.Text = "toolStripButton1";
            this.toolStripButtonLayout.ToolTipText = "Page Setup";
            this.toolStripButtonLayout.Click += new System.EventHandler(this.toolStripButtonLayout_Click);
            // 
            // webBrowserView
            // 
            this.webBrowserView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserView.Location = new System.Drawing.Point(0, 25);
            this.webBrowserView.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserView.Name = "webBrowserView";
            this.webBrowserView.ScriptErrorsSuppressed = true;
            this.webBrowserView.Size = new System.Drawing.Size(668, 461);
            this.webBrowserView.TabIndex = 1;
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelPage});
            this.statusStripMain.Location = new System.Drawing.Point(0, 464);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(668, 22);
            this.statusStripMain.TabIndex = 2;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripStatusLabelPage
            // 
            this.toolStripStatusLabelPage.Name = "toolStripStatusLabelPage";
            this.toolStripStatusLabelPage.Size = new System.Drawing.Size(36, 17);
            this.toolStripStatusLabelPage.Text = "Page:";
            // 
            // DeviceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 486);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.webBrowserView);
            this.Controls.Add(this.toolStripNav);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeviceViewer";
            this.Text = "DeviceViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceViewer_FormClosing);
            this.Load += new System.EventHandler(this.DeviceViewer_Load);
            this.Shown += new System.EventHandler(this.DeviceViewer_Shown);
            this.toolStripNav.ResumeLayout(false);
            this.toolStripNav.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripNav;
        private System.Windows.Forms.WebBrowser webBrowserView;
        private System.Windows.Forms.ToolStripButton toolStripButtonBack;
        private System.Windows.Forms.ToolStripButton toolStripButtonGo;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrint;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreview;
        private System.Windows.Forms.ToolStripButton toolStripButtonLayout;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelPage;

    }
}