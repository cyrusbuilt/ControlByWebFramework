namespace CyrusBuilt.ControlByWeb.Commander.Win
{
    partial class RawResponseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RawResponseForm));
            this.textBoxResponse = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxResponse
            // 
            this.textBoxResponse.BackColor = System.Drawing.Color.Black;
            this.textBoxResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxResponse.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxResponse.Location = new System.Drawing.Point(0, 0);
            this.textBoxResponse.Multiline = true;
            this.textBoxResponse.Name = "textBoxResponse";
            this.textBoxResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResponse.Size = new System.Drawing.Size(473, 236);
            this.textBoxResponse.TabIndex = 0;
            // 
            // RawResponseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 236);
            this.Controls.Add(this.textBoxResponse);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RawResponseForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "RawResponseForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RawResponseForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxResponse;
    }
}