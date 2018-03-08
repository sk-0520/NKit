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
            this.selectWorkspace = new System.Windows.Forms.ComboBox();
            this.commandLoad = new System.Windows.Forms.Button();
            this.commandClose = new System.Windows.Forms.Button();
            this.commandEdit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectWorkspace
            // 
            this.selectWorkspace.FormattingEnabled = true;
            this.selectWorkspace.Location = new System.Drawing.Point(103, 12);
            this.selectWorkspace.Name = "selectWorkspace";
            this.selectWorkspace.Size = new System.Drawing.Size(205, 20);
            this.selectWorkspace.TabIndex = 0;
            // 
            // commandLoad
            // 
            this.commandLoad.Location = new System.Drawing.Point(314, 12);
            this.commandLoad.Name = "commandLoad";
            this.commandLoad.Size = new System.Drawing.Size(75, 23);
            this.commandLoad.TabIndex = 1;
            this.commandLoad.Text = "load";
            this.commandLoad.UseVisualStyleBackColor = true;
            // 
            // commandClose
            // 
            this.commandClose.Location = new System.Drawing.Point(395, 12);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(75, 23);
            this.commandClose.TabIndex = 1;
            this.commandClose.Text = "close";
            this.commandClose.UseVisualStyleBackColor = true;
            // 
            // commandEdit
            // 
            this.commandEdit.Location = new System.Drawing.Point(12, 12);
            this.commandEdit.Name = "commandEdit";
            this.commandEdit.Size = new System.Drawing.Size(75, 23);
            this.commandEdit.TabIndex = 1;
            this.commandEdit.Text = "edit";
            this.commandEdit.UseVisualStyleBackColor = true;
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 228);
            this.Controls.Add(this.commandClose);
            this.Controls.Add(this.commandEdit);
            this.Controls.Add(this.commandLoad);
            this.Controls.Add(this.selectWorkspace);
            this.Name = "ManagerForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox selectWorkspace;
        private System.Windows.Forms.Button commandLoad;
        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.Button commandEdit;
    }
}

