namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class TestExecuteForm
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
            this.commandExecute = new System.Windows.Forms.Button();
            this.commandClose = new System.Windows.Forms.Button();
            this.listApplications = new System.Windows.Forms.ListView();
            this.columnKind = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // commandExecute
            // 
            this.commandExecute.Location = new System.Drawing.Point(12, 212);
            this.commandExecute.Name = "commandExecute";
            this.commandExecute.Size = new System.Drawing.Size(75, 23);
            this.commandExecute.TabIndex = 0;
            this.commandExecute.Text = "execute";
            this.commandExecute.UseVisualStyleBackColor = true;
            this.commandExecute.Click += new System.EventHandler(this.commandExecute_Click);
            // 
            // commandClose
            // 
            this.commandClose.Location = new System.Drawing.Point(132, 212);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(75, 23);
            this.commandClose.TabIndex = 0;
            this.commandClose.Text = "close";
            this.commandClose.UseVisualStyleBackColor = true;
            this.commandClose.Click += new System.EventHandler(this.commandClose_Click);
            // 
            // listApplications
            // 
            this.listApplications.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnKind,
            this.columnName,
            this.columnState});
            this.listApplications.Location = new System.Drawing.Point(12, 12);
            this.listApplications.Name = "listApplications";
            this.listApplications.Size = new System.Drawing.Size(247, 185);
            this.listApplications.TabIndex = 1;
            this.listApplications.UseCompatibleStateImageBehavior = false;
            this.listApplications.View = System.Windows.Forms.View.Details;
            // 
            // columnKind
            // 
            this.columnKind.Text = "kind";
            // 
            // columnName
            // 
            this.columnName.Text = "name";
            // 
            // columnState
            // 
            this.columnState.Text = "state";
            // 
            // TestExecuteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 247);
            this.Controls.Add(this.listApplications);
            this.Controls.Add(this.commandClose);
            this.Controls.Add(this.commandExecute);
            this.Name = "TestExecuteForm";
            this.Text = "TestExecuteForm";
            this.Shown += new System.EventHandler(this.TestExecuteForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button commandExecute;
        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.ListView listApplications;
        private System.Windows.Forms.ColumnHeader columnKind;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnState;
    }
}