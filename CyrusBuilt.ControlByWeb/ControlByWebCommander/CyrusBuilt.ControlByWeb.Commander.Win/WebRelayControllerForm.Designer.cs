namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    partial class WebRelayControllerForm
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
            this.components = new System.ComponentModel.Container();
            this.groupBoxState = new System.Windows.Forms.GroupBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonGo = new System.Windows.Forms.Button();
            this.labelInterval = new System.Windows.Forms.Label();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.checkBoxAuto = new System.Windows.Forms.CheckBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.textBoxTotalRebs = new System.Windows.Forms.TextBox();
            this.labelTotalRebs = new System.Windows.Forms.Label();
            this.textBoxRebState = new System.Windows.Forms.TextBox();
            this.labelRebState = new System.Windows.Forms.Label();
            this.textBoxInpState = new System.Windows.Forms.TextBox();
            this.labelInpState = new System.Windows.Forms.Label();
            this.labelRelState = new System.Windows.Forms.Label();
            this.textBoxRelState = new System.Windows.Forms.TextBox();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.buttonView = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.checkBoxPassword = new System.Windows.Forms.CheckBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.labelAddress = new System.Windows.Forms.Label();
            this.errorProviderMain = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBoxControl = new System.Windows.Forms.GroupBox();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.buttonOn = new System.Windows.Forms.Button();
            this.buttonOff = new System.Windows.Forms.Button();
            this.buttonToggle = new System.Windows.Forms.Button();
            this.buttonPulse = new System.Windows.Forms.Button();
            this.textBoxPulseTime = new System.Windows.Forms.TextBox();
            this.groupBoxState.SuspendLayout();
            this.groupBoxConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderMain)).BeginInit();
            this.groupBoxControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxState
            // 
            this.groupBoxState.Controls.Add(this.buttonClear);
            this.groupBoxState.Controls.Add(this.buttonStop);
            this.groupBoxState.Controls.Add(this.buttonGo);
            this.groupBoxState.Controls.Add(this.labelInterval);
            this.groupBoxState.Controls.Add(this.textBoxInterval);
            this.groupBoxState.Controls.Add(this.checkBoxAuto);
            this.groupBoxState.Controls.Add(this.buttonRefresh);
            this.groupBoxState.Controls.Add(this.textBoxTotalRebs);
            this.groupBoxState.Controls.Add(this.labelTotalRebs);
            this.groupBoxState.Controls.Add(this.textBoxRebState);
            this.groupBoxState.Controls.Add(this.labelRebState);
            this.groupBoxState.Controls.Add(this.textBoxInpState);
            this.groupBoxState.Controls.Add(this.labelInpState);
            this.groupBoxState.Controls.Add(this.labelRelState);
            this.groupBoxState.Controls.Add(this.textBoxRelState);
            this.groupBoxState.Location = new System.Drawing.Point(12, 126);
            this.groupBoxState.Name = "groupBoxState";
            this.groupBoxState.Size = new System.Drawing.Size(443, 156);
            this.groupBoxState.TabIndex = 0;
            this.groupBoxState.TabStop = false;
            this.groupBoxState.Text = "Device Status";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(220, 111);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(20, 23);
            this.buttonClear.TabIndex = 2;
            this.buttonClear.Text = "C";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(305, 125);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(49, 23);
            this.buttonStop.TabIndex = 13;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonGo
            // 
            this.buttonGo.Enabled = false;
            this.buttonGo.Location = new System.Drawing.Point(360, 125);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(52, 23);
            this.buttonGo.TabIndex = 12;
            this.buttonGo.Text = "&Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // labelInterval
            // 
            this.labelInterval.AutoSize = true;
            this.labelInterval.Location = new System.Drawing.Point(260, 103);
            this.labelInterval.Name = "labelInterval";
            this.labelInterval.Size = new System.Drawing.Size(94, 13);
            this.labelInterval.TabIndex = 11;
            this.labelInterval.Text = "Interval (seconds):";
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Enabled = false;
            this.textBoxInterval.Location = new System.Drawing.Point(360, 100);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(52, 20);
            this.textBoxInterval.TabIndex = 10;
            // 
            // checkBoxAuto
            // 
            this.checkBoxAuto.AutoSize = true;
            this.checkBoxAuto.Location = new System.Drawing.Point(263, 74);
            this.checkBoxAuto.Name = "checkBoxAuto";
            this.checkBoxAuto.Size = new System.Drawing.Size(88, 17);
            this.checkBoxAuto.TabIndex = 9;
            this.checkBoxAuto.Text = "Auto-Refresh";
            this.checkBoxAuto.UseVisualStyleBackColor = true;
            this.checkBoxAuto.CheckedChanged += new System.EventHandler(this.checkBoxAuto_CheckedChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(263, 26);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 8;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // textBoxTotalRebs
            // 
            this.textBoxTotalRebs.BackColor = System.Drawing.Color.Black;
            this.textBoxTotalRebs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTotalRebs.ForeColor = System.Drawing.Color.Red;
            this.textBoxTotalRebs.Location = new System.Drawing.Point(110, 113);
            this.textBoxTotalRebs.Name = "textBoxTotalRebs";
            this.textBoxTotalRebs.ReadOnly = true;
            this.textBoxTotalRebs.Size = new System.Drawing.Size(104, 20);
            this.textBoxTotalRebs.TabIndex = 7;
            this.textBoxTotalRebs.Text = "0";
            // 
            // labelTotalRebs
            // 
            this.labelTotalRebs.AutoSize = true;
            this.labelTotalRebs.Location = new System.Drawing.Point(25, 116);
            this.labelTotalRebs.Name = "labelTotalRebs";
            this.labelTotalRebs.Size = new System.Drawing.Size(77, 13);
            this.labelTotalRebs.TabIndex = 6;
            this.labelTotalRebs.Text = "Total Reboots:";
            // 
            // textBoxRebState
            // 
            this.textBoxRebState.BackColor = System.Drawing.Color.Black;
            this.textBoxRebState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRebState.ForeColor = System.Drawing.Color.Red;
            this.textBoxRebState.Location = new System.Drawing.Point(110, 83);
            this.textBoxRebState.Name = "textBoxRebState";
            this.textBoxRebState.ReadOnly = true;
            this.textBoxRebState.Size = new System.Drawing.Size(104, 20);
            this.textBoxRebState.TabIndex = 5;
            this.textBoxRebState.Text = "Unknown";
            // 
            // labelRebState
            // 
            this.labelRebState.AutoSize = true;
            this.labelRebState.Location = new System.Drawing.Point(25, 86);
            this.labelRebState.Name = "labelRebState";
            this.labelRebState.Size = new System.Drawing.Size(73, 13);
            this.labelRebState.TabIndex = 4;
            this.labelRebState.Text = "Reboot State:";
            // 
            // textBoxInpState
            // 
            this.textBoxInpState.BackColor = System.Drawing.Color.Black;
            this.textBoxInpState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInpState.ForeColor = System.Drawing.Color.Red;
            this.textBoxInpState.Location = new System.Drawing.Point(110, 55);
            this.textBoxInpState.Name = "textBoxInpState";
            this.textBoxInpState.ReadOnly = true;
            this.textBoxInpState.Size = new System.Drawing.Size(104, 20);
            this.textBoxInpState.TabIndex = 3;
            this.textBoxInpState.Text = "Unknown";
            // 
            // labelInpState
            // 
            this.labelInpState.AutoSize = true;
            this.labelInpState.Location = new System.Drawing.Point(25, 58);
            this.labelInpState.Name = "labelInpState";
            this.labelInpState.Size = new System.Drawing.Size(62, 13);
            this.labelInpState.TabIndex = 2;
            this.labelInpState.Text = "Input State:";
            // 
            // labelRelState
            // 
            this.labelRelState.AutoSize = true;
            this.labelRelState.Location = new System.Drawing.Point(25, 31);
            this.labelRelState.Name = "labelRelState";
            this.labelRelState.Size = new System.Drawing.Size(63, 13);
            this.labelRelState.TabIndex = 1;
            this.labelRelState.Text = "Relay state:";
            // 
            // textBoxRelState
            // 
            this.textBoxRelState.BackColor = System.Drawing.Color.Black;
            this.textBoxRelState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRelState.ForeColor = System.Drawing.Color.Red;
            this.textBoxRelState.Location = new System.Drawing.Point(110, 28);
            this.textBoxRelState.Name = "textBoxRelState";
            this.textBoxRelState.ReadOnly = true;
            this.textBoxRelState.Size = new System.Drawing.Size(104, 20);
            this.textBoxRelState.TabIndex = 0;
            this.textBoxRelState.Text = "Unknown";
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.buttonView);
            this.groupBoxConnection.Controls.Add(this.textBoxPassword);
            this.groupBoxConnection.Controls.Add(this.checkBoxPassword);
            this.groupBoxConnection.Controls.Add(this.textBoxPort);
            this.groupBoxConnection.Controls.Add(this.labelPort);
            this.groupBoxConnection.Controls.Add(this.textBoxAddress);
            this.groupBoxConnection.Controls.Add(this.labelAddress);
            this.groupBoxConnection.Location = new System.Drawing.Point(12, 20);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(443, 100);
            this.groupBoxConnection.TabIndex = 1;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection";
            // 
            // buttonView
            // 
            this.buttonView.Image = global::CyrusBuilt.ControlByWeb.Commander.Win.Properties.Resources.world_link;
            this.buttonView.Location = new System.Drawing.Point(335, 18);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(34, 30);
            this.buttonView.TabIndex = 7;
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            this.buttonView.MouseHover += new System.EventHandler(this.buttonView_MouseHover);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Enabled = false;
            this.textBoxPassword.Location = new System.Drawing.Point(147, 58);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(171, 20);
            this.textBoxPassword.TabIndex = 6;
            // 
            // checkBoxPassword
            // 
            this.checkBoxPassword.AutoSize = true;
            this.checkBoxPassword.Location = new System.Drawing.Point(22, 60);
            this.checkBoxPassword.Name = "checkBoxPassword";
            this.checkBoxPassword.Size = new System.Drawing.Size(119, 17);
            this.checkBoxPassword.TabIndex = 5;
            this.checkBoxPassword.Text = "Requires password:";
            this.checkBoxPassword.UseVisualStyleBackColor = true;
            this.checkBoxPassword.CheckedChanged += new System.EventHandler(this.checkBoxPassword_CheckedChanged);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(277, 24);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(41, 20);
            this.textBoxPort.TabIndex = 4;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(242, 27);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(29, 13);
            this.labelPort.TabIndex = 3;
            this.labelPort.Text = "Port:";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(73, 24);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(141, 20);
            this.textBoxAddress.TabIndex = 2;
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(19, 27);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(48, 13);
            this.labelAddress.TabIndex = 0;
            this.labelAddress.Text = "Address:";
            // 
            // errorProviderMain
            // 
            this.errorProviderMain.ContainerControl = this;
            // 
            // groupBoxControl
            // 
            this.groupBoxControl.Controls.Add(this.textBoxPulseTime);
            this.groupBoxControl.Controls.Add(this.buttonPulse);
            this.groupBoxControl.Controls.Add(this.buttonToggle);
            this.groupBoxControl.Controls.Add(this.buttonOff);
            this.groupBoxControl.Controls.Add(this.buttonOn);
            this.groupBoxControl.Location = new System.Drawing.Point(12, 288);
            this.groupBoxControl.Name = "groupBoxControl";
            this.groupBoxControl.Size = new System.Drawing.Size(443, 100);
            this.groupBoxControl.TabIndex = 2;
            this.groupBoxControl.TabStop = false;
            this.groupBoxControl.Text = "Control";
            // 
            // buttonOn
            // 
            this.buttonOn.Location = new System.Drawing.Point(12, 28);
            this.buttonOn.Name = "buttonOn";
            this.buttonOn.Size = new System.Drawing.Size(75, 23);
            this.buttonOn.TabIndex = 0;
            this.buttonOn.Text = "On";
            this.buttonOn.UseVisualStyleBackColor = true;
            this.buttonOn.Click += new System.EventHandler(this.buttonOn_Click);
            // 
            // buttonOff
            // 
            this.buttonOff.Location = new System.Drawing.Point(12, 58);
            this.buttonOff.Name = "buttonOff";
            this.buttonOff.Size = new System.Drawing.Size(75, 23);
            this.buttonOff.TabIndex = 1;
            this.buttonOff.Text = "Off";
            this.buttonOff.UseVisualStyleBackColor = true;
            this.buttonOff.Click += new System.EventHandler(this.buttonOff_Click);
            // 
            // buttonToggle
            // 
            this.buttonToggle.Location = new System.Drawing.Point(93, 28);
            this.buttonToggle.Name = "buttonToggle";
            this.buttonToggle.Size = new System.Drawing.Size(75, 23);
            this.buttonToggle.TabIndex = 2;
            this.buttonToggle.Text = "Toggle";
            this.buttonToggle.UseVisualStyleBackColor = true;
            this.buttonToggle.Click += new System.EventHandler(this.buttonToggle_Click);
            // 
            // buttonPulse
            // 
            this.buttonPulse.Location = new System.Drawing.Point(93, 58);
            this.buttonPulse.Name = "buttonPulse";
            this.buttonPulse.Size = new System.Drawing.Size(75, 23);
            this.buttonPulse.TabIndex = 3;
            this.buttonPulse.Text = "Pulse";
            this.buttonPulse.UseVisualStyleBackColor = true;
            this.buttonPulse.Click += new System.EventHandler(this.buttonPulse_Click);
            // 
            // textBoxPulseTime
            // 
            this.textBoxPulseTime.Location = new System.Drawing.Point(174, 60);
            this.textBoxPulseTime.Name = "textBoxPulseTime";
            this.textBoxPulseTime.Size = new System.Drawing.Size(66, 20);
            this.textBoxPulseTime.TabIndex = 4;
            // 
            // WebRelayControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 393);
            this.Controls.Add(this.groupBoxControl);
            this.Controls.Add(this.groupBoxConnection);
            this.Controls.Add(this.groupBoxState);
            this.Name = "WebRelayControllerForm";
            this.Text = "WebRelayControllerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebRelayControllerForm_FormClosing);
            this.Load += new System.EventHandler(this.WebRelayControllerForm_Load);
            this.groupBoxState.ResumeLayout(false);
            this.groupBoxState.PerformLayout();
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderMain)).EndInit();
            this.groupBoxControl.ResumeLayout(false);
            this.groupBoxControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxState;
        private System.Windows.Forms.Label labelRelState;
        private System.Windows.Forms.TextBox textBoxRelState;
        private System.Windows.Forms.TextBox textBoxInpState;
        private System.Windows.Forms.Label labelInpState;
        private System.Windows.Forms.TextBox textBoxRebState;
        private System.Windows.Forms.Label labelRebState;
        private System.Windows.Forms.TextBox textBoxTotalRebs;
        private System.Windows.Forms.Label labelTotalRebs;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.CheckBox checkBoxAuto;
        private System.Windows.Forms.Label labelInterval;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.ErrorProvider errorProviderMain;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label labelAddress;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.CheckBox checkBoxPassword;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupBoxControl;
        private System.Windows.Forms.Button buttonView;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.TextBox textBoxPulseTime;
        private System.Windows.Forms.Button buttonPulse;
        private System.Windows.Forms.Button buttonToggle;
        private System.Windows.Forms.Button buttonOff;
        private System.Windows.Forms.Button buttonOn;
    }
}