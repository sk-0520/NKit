namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class ManagerForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.selectWorkspace = new System.Windows.Forms.ComboBox();
            this.commandWorkspaceLoad = new System.Windows.Forms.Button();
            this.commandWorkspaceClose = new System.Windows.Forms.Button();
            this.commandWorkspaceCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.commandWorkspaceSave = new System.Windows.Forms.Button();
            this.commandWorkspaceDirectorySelect = new System.Windows.Forms.Button();
            this.inputWorkspaceName = new System.Windows.Forms.TextBox();
            this.inputWorkspaceDirectoryPath = new System.Windows.Forms.TextBox();
            this.commandWorkspaceCopy = new System.Windows.Forms.Button();
            this.commandWorkspaceDelete = new System.Windows.Forms.Button();
            this.panelWorkspace = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.viewLog = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.selectLogging = new System.Windows.Forms.CheckBox();
            this.panelWorkspace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // selectWorkspace
            // 
            this.selectWorkspace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectWorkspace.FormattingEnabled = true;
            this.selectWorkspace.Location = new System.Drawing.Point(93, 12);
            this.selectWorkspace.Name = "selectWorkspace";
            this.selectWorkspace.Size = new System.Drawing.Size(215, 20);
            this.selectWorkspace.TabIndex = 0;
            this.selectWorkspace.SelectedIndexChanged += new System.EventHandler(this.selectWorkspace_SelectedIndexChanged);
            // 
            // commandWorkspaceLoad
            // 
            this.commandWorkspaceLoad.Location = new System.Drawing.Point(249, 38);
            this.commandWorkspaceLoad.Name = "commandWorkspaceLoad";
            this.commandWorkspaceLoad.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceLoad.TabIndex = 1;
            this.commandWorkspaceLoad.Text = "load";
            this.commandWorkspaceLoad.UseVisualStyleBackColor = true;
            this.commandWorkspaceLoad.Click += new System.EventHandler(this.commandWorkspaceLoad_Click);
            // 
            // commandWorkspaceClose
            // 
            this.commandWorkspaceClose.Location = new System.Drawing.Point(233, 79);
            this.commandWorkspaceClose.Name = "commandWorkspaceClose";
            this.commandWorkspaceClose.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceClose.TabIndex = 1;
            this.commandWorkspaceClose.Text = "close";
            this.commandWorkspaceClose.UseVisualStyleBackColor = true;
            this.commandWorkspaceClose.Click += new System.EventHandler(this.commandWorkspaceClose_Click);
            // 
            // commandWorkspaceCreate
            // 
            this.commandWorkspaceCreate.Location = new System.Drawing.Point(49, 38);
            this.commandWorkspaceCreate.Name = "commandWorkspaceCreate";
            this.commandWorkspaceCreate.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceCreate.TabIndex = 1;
            this.commandWorkspaceCreate.Text = "create";
            this.commandWorkspaceCreate.UseVisualStyleBackColor = true;
            this.commandWorkspaceCreate.Click += new System.EventHandler(this.commandWorkspaceCreate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "dir";
            // 
            // commandWorkspaceSave
            // 
            this.commandWorkspaceSave.Location = new System.Drawing.Point(153, 35);
            this.commandWorkspaceSave.Name = "commandWorkspaceSave";
            this.commandWorkspaceSave.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceSave.TabIndex = 1;
            this.commandWorkspaceSave.Text = "save";
            this.commandWorkspaceSave.UseVisualStyleBackColor = true;
            this.commandWorkspaceSave.Click += new System.EventHandler(this.commandWorkspaceSave_Click);
            // 
            // commandWorkspaceDirectorySelect
            // 
            this.commandWorkspaceDirectorySelect.Location = new System.Drawing.Point(101, 60);
            this.commandWorkspaceDirectorySelect.Name = "commandWorkspaceDirectorySelect";
            this.commandWorkspaceDirectorySelect.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceDirectorySelect.TabIndex = 3;
            this.commandWorkspaceDirectorySelect.Text = "dir";
            this.commandWorkspaceDirectorySelect.UseVisualStyleBackColor = true;
            // 
            // inputWorkspaceName
            // 
            this.inputWorkspaceName.Location = new System.Drawing.Point(47, 7);
            this.inputWorkspaceName.Name = "inputWorkspaceName";
            this.inputWorkspaceName.Size = new System.Drawing.Size(100, 19);
            this.inputWorkspaceName.TabIndex = 4;
            // 
            // inputWorkspaceDirectoryPath
            // 
            this.inputWorkspaceDirectoryPath.Location = new System.Drawing.Point(47, 35);
            this.inputWorkspaceDirectoryPath.Name = "inputWorkspaceDirectoryPath";
            this.inputWorkspaceDirectoryPath.Size = new System.Drawing.Size(100, 19);
            this.inputWorkspaceDirectoryPath.TabIndex = 4;
            // 
            // commandWorkspaceCopy
            // 
            this.commandWorkspaceCopy.Location = new System.Drawing.Point(12, 9);
            this.commandWorkspaceCopy.Name = "commandWorkspaceCopy";
            this.commandWorkspaceCopy.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceCopy.TabIndex = 1;
            this.commandWorkspaceCopy.Text = "copy";
            this.commandWorkspaceCopy.UseVisualStyleBackColor = true;
            this.commandWorkspaceCopy.Click += new System.EventHandler(this.commandWorkspaceCopy_Click);
            // 
            // commandWorkspaceDelete
            // 
            this.commandWorkspaceDelete.Location = new System.Drawing.Point(167, 3);
            this.commandWorkspaceDelete.Name = "commandWorkspaceDelete";
            this.commandWorkspaceDelete.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceDelete.TabIndex = 1;
            this.commandWorkspaceDelete.Text = "delete";
            this.commandWorkspaceDelete.UseVisualStyleBackColor = true;
            this.commandWorkspaceDelete.Click += new System.EventHandler(this.commandWorkspaceDelete_Click);
            // 
            // panelWorkspace
            // 
            this.panelWorkspace.Controls.Add(this.selectLogging);
            this.panelWorkspace.Controls.Add(this.inputWorkspaceName);
            this.panelWorkspace.Controls.Add(this.inputWorkspaceDirectoryPath);
            this.panelWorkspace.Controls.Add(this.commandWorkspaceDelete);
            this.panelWorkspace.Controls.Add(this.commandWorkspaceSave);
            this.panelWorkspace.Controls.Add(this.label1);
            this.panelWorkspace.Controls.Add(this.label2);
            this.panelWorkspace.Controls.Add(this.commandWorkspaceDirectorySelect);
            this.panelWorkspace.Location = new System.Drawing.Point(354, 0);
            this.panelWorkspace.Name = "panelWorkspace";
            this.panelWorkspace.Size = new System.Drawing.Size(246, 92);
            this.panelWorkspace.TabIndex = 5;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // viewLog
            // 
            this.viewLog.Location = new System.Drawing.Point(57, 108);
            this.viewLog.Name = "viewLog";
            this.viewLog.Size = new System.Drawing.Size(525, 134);
            this.viewLog.TabIndex = 6;
            this.viewLog.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "こーじちゅう";
            // 
            // selectLogging
            // 
            this.selectLogging.AutoSize = true;
            this.selectLogging.Location = new System.Drawing.Point(11, 60);
            this.selectLogging.Name = "selectLogging";
            this.selectLogging.Size = new System.Drawing.Size(39, 16);
            this.selectLogging.TabIndex = 5;
            this.selectLogging.Text = "log";
            this.selectLogging.UseVisualStyleBackColor = true;
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 270);
            this.Controls.Add(this.viewLog);
            this.Controls.Add(this.panelWorkspace);
            this.Controls.Add(this.commandWorkspaceClose);
            this.Controls.Add(this.commandWorkspaceCopy);
            this.Controls.Add(this.commandWorkspaceCreate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.commandWorkspaceLoad);
            this.Controls.Add(this.selectWorkspace);
            this.Name = "ManagerForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.panelWorkspace.ResumeLayout(false);
            this.panelWorkspace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox selectWorkspace;
        private System.Windows.Forms.Button commandWorkspaceLoad;
        private System.Windows.Forms.Button commandWorkspaceClose;
        private System.Windows.Forms.Button commandWorkspaceCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button commandWorkspaceSave;
        private System.Windows.Forms.Button commandWorkspaceDirectorySelect;
        private System.Windows.Forms.TextBox inputWorkspaceName;
        private System.Windows.Forms.TextBox inputWorkspaceDirectoryPath;
        private System.Windows.Forms.Button commandWorkspaceCopy;
        private System.Windows.Forms.Button commandWorkspaceDelete;
        private System.Windows.Forms.Panel panelWorkspace;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.RichTextBox viewLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox selectLogging;
    }
}

