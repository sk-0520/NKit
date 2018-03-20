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
            this.commandClose = new System.Windows.Forms.Button();
            this.viewReleaseNote = new System.Windows.Forms.WebBrowser();
            this.commandUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // commandClose
            // 
            this.commandClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.commandClose.Location = new System.Drawing.Point(146, 269);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(75, 23);
            this.commandClose.TabIndex = 0;
            this.commandClose.Text = "close";
            this.commandClose.UseVisualStyleBackColor = true;
            this.commandClose.Click += new System.EventHandler(this.commandClose_Click);
            // 
            // viewReleaseNote
            // 
            this.viewReleaseNote.Location = new System.Drawing.Point(12, 12);
            this.viewReleaseNote.MinimumSize = new System.Drawing.Size(20, 20);
            this.viewReleaseNote.Name = "viewReleaseNote";
            this.viewReleaseNote.Size = new System.Drawing.Size(421, 259);
            this.viewReleaseNote.TabIndex = 1;
            // 
            // commandUpdate
            // 
            this.commandUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.commandUpdate.Location = new System.Drawing.Point(28, 277);
            this.commandUpdate.Name = "commandUpdate";
            this.commandUpdate.Size = new System.Drawing.Size(75, 23);
            this.commandUpdate.TabIndex = 0;
            this.commandUpdate.Text = "update";
            this.commandUpdate.UseVisualStyleBackColor = true;
            this.commandUpdate.Click += new System.EventHandler(this.commandUpdate_Click);
            // 
            // ReleaseNoteForm
            // 
            this.AcceptButton = this.commandClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.commandClose;
            this.ClientSize = new System.Drawing.Size(482, 312);
            this.Controls.Add(this.commandUpdate);
            this.Controls.Add(this.commandClose);
            this.Controls.Add(this.viewReleaseNote);
            this.Name = "ReleaseNoteForm";
            this.Text = "ReleaseNoteForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.WebBrowser viewReleaseNote;
        private System.Windows.Forms.Button commandUpdate;
    }
}
