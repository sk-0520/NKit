namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class ReleaseNoteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReleaseNoteForm));
            this.commandClose = new System.Windows.Forms.Button();
            this.commandUpdate = new System.Windows.Forms.Button();
            this.releaseNoteControl = new ContentTypeTextNet.NKit.Manager.View.ReleaseNoteControl();
            this.SuspendLayout();
            // 
            // commandClose
            // 
            this.commandClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commandClose.AutoSize = true;
            this.commandClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.commandClose.Location = new System.Drawing.Point(392, 276);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(80, 23);
            this.commandClose.TabIndex = 0;
            this.commandClose.Text = "閉じる(&C)";
            this.commandClose.UseVisualStyleBackColor = true;
            this.commandClose.Click += new System.EventHandler(this.commandClose_Click);
            // 
            // commandUpdate
            // 
            this.commandUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.commandUpdate.AutoSize = true;
            this.commandUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.commandUpdate.Location = new System.Drawing.Point(12, 276);
            this.commandUpdate.Name = "commandUpdate";
            this.commandUpdate.Size = new System.Drawing.Size(117, 23);
            this.commandUpdate.TabIndex = 2;
            this.commandUpdate.Text = "アップデートを開始(&U)";
            this.commandUpdate.UseVisualStyleBackColor = true;
            this.commandUpdate.Click += new System.EventHandler(this.commandUpdate_Click);
            // 
            // releaseNoteControl
            // 
            this.releaseNoteControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.releaseNoteControl.IssueBaseUri = null;
            this.releaseNoteControl.Location = new System.Drawing.Point(12, 12);
            this.releaseNoteControl.MinimumSize = new System.Drawing.Size(20, 20);
            this.releaseNoteControl.Name = "releaseNoteControl";
            this.releaseNoteControl.Size = new System.Drawing.Size(460, 244);
            this.releaseNoteControl.TabIndex = 1;
            // 
            // ReleaseNoteForm
            // 
            this.AcceptButton = this.commandClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 311);
            this.Controls.Add(this.releaseNoteControl);
            this.Controls.Add(this.commandUpdate);
            this.Controls.Add(this.commandClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Name = "ReleaseNoteForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<CODE>";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.Button commandUpdate;
        private ReleaseNoteControl releaseNoteControl;
    }
}
