namespace ContentTypeTextNet.NKit.Cameraman.View
{
    partial class InformationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InformationForm));
            this.windowStatusControl = new ContentTypeTextNet.NKit.Cameraman.View.WindowStatusControl();
            this.navigationControl = new ContentTypeTextNet.NKit.Cameraman.View.NavigationControl();
            this.SuspendLayout();
            // 
            // windowStatusControl
            // 
            this.windowStatusControl.AutoSize = true;
            this.windowStatusControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.windowStatusControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.windowStatusControl.Location = new System.Drawing.Point(0, 0);
            this.windowStatusControl.Name = "windowStatusControl";
            this.windowStatusControl.Size = new System.Drawing.Size(300, 149);
            this.windowStatusControl.TabIndex = 1;
            this.windowStatusControl.WindowClassBufferLength = 256;
            this.windowStatusControl.WindowTextBufferLength = 256;
            // 
            // navigationControl
            // 
            this.navigationControl.AutoSize = true;
            this.navigationControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.navigationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationControl.Location = new System.Drawing.Point(0, 0);
            this.navigationControl.Name = "navigationControl";
            this.navigationControl.Size = new System.Drawing.Size(300, 149);
            this.navigationControl.TabIndex = 0;
            this.navigationControl.MouseEnter += new System.EventHandler(this.InformationForm_MouseEnter);
            this.navigationControl.MouseLeave += new System.EventHandler(this.InformationForm_MouseLeave);
            // 
            // InformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(300, 149);
            this.ControlBox = false;
            this.Controls.Add(this.windowStatusControl);
            this.Controls.Add(this.navigationControl);
            this.ForeColor = System.Drawing.SystemColors.InfoText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InformationForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "InformationForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.InformationForm_Shown);
            this.MouseEnter += new System.EventHandler(this.InformationForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.InformationForm_MouseLeave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NavigationControl navigationControl;
        private WindowStatusControl windowStatusControl;
    }
}
