namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class AcceptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcceptForm));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabMainPageContent = new System.Windows.Forms.TabPage();
            this.viewMessage = new System.Windows.Forms.RichTextBox();
            this.tabMainPageLicense = new System.Windows.Forms.TabPage();
            this.viewLicense = new System.Windows.Forms.RichTextBox();
            this.tabMainPageThirdParty = new System.Windows.Forms.TabPage();
            this.thirdPartyLicenseControl1 = new ContentTypeTextNet.NKit.Manager.View.ThirdPartyLicenseControl();
            this.commandCancel = new System.Windows.Forms.Button();
            this.selectedRead = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.commanAccept = new System.Windows.Forms.Button();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabMain.SuspendLayout();
            this.tabMainPageContent.SuspendLayout();
            this.tabMainPageLicense.SuspendLayout();
            this.tabMainPageThirdParty.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabMainPageContent);
            this.tabMain.Controls.Add(this.tabMainPageLicense);
            this.tabMain.Controls.Add(this.tabMainPageThirdParty);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.HotTrack = true;
            this.tabMain.Location = new System.Drawing.Point(3, 3);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(478, 277);
            this.tabMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabMain.TabIndex = 0;
            // 
            // tabMainPageContent
            // 
            this.tabMainPageContent.Controls.Add(this.viewMessage);
            this.tabMainPageContent.Location = new System.Drawing.Point(4, 22);
            this.tabMainPageContent.Name = "tabMainPageContent";
            this.tabMainPageContent.Padding = new System.Windows.Forms.Padding(3);
            this.tabMainPageContent.Size = new System.Drawing.Size(470, 251);
            this.tabMainPageContent.TabIndex = 0;
            this.tabMainPageContent.Text = "内容";
            this.tabMainPageContent.UseVisualStyleBackColor = true;
            // 
            // viewMessage
            // 
            this.viewMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewMessage.Location = new System.Drawing.Point(3, 3);
            this.viewMessage.Name = "viewMessage";
            this.viewMessage.Size = new System.Drawing.Size(464, 245);
            this.viewMessage.TabIndex = 2;
            this.viewMessage.Text = "";
            // 
            // tabMainPageLicense
            // 
            this.tabMainPageLicense.Controls.Add(this.viewLicense);
            this.tabMainPageLicense.Location = new System.Drawing.Point(4, 22);
            this.tabMainPageLicense.Name = "tabMainPageLicense";
            this.tabMainPageLicense.Padding = new System.Windows.Forms.Padding(3);
            this.tabMainPageLicense.Size = new System.Drawing.Size(370, 171);
            this.tabMainPageLicense.TabIndex = 2;
            this.tabMainPageLicense.Text = "ライセンス情報";
            this.tabMainPageLicense.UseVisualStyleBackColor = true;
            // 
            // viewLicense
            // 
            this.viewLicense.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewLicense.Location = new System.Drawing.Point(3, 3);
            this.viewLicense.Name = "viewLicense";
            this.viewLicense.Size = new System.Drawing.Size(364, 165);
            this.viewLicense.TabIndex = 1;
            this.viewLicense.Text = "";
            // 
            // tabMainPageThirdParty
            // 
            this.tabMainPageThirdParty.Controls.Add(this.thirdPartyLicenseControl1);
            this.tabMainPageThirdParty.Location = new System.Drawing.Point(4, 22);
            this.tabMainPageThirdParty.Name = "tabMainPageThirdParty";
            this.tabMainPageThirdParty.Padding = new System.Windows.Forms.Padding(3);
            this.tabMainPageThirdParty.Size = new System.Drawing.Size(370, 171);
            this.tabMainPageThirdParty.TabIndex = 3;
            this.tabMainPageThirdParty.Text = "サードパーティ";
            this.tabMainPageThirdParty.UseVisualStyleBackColor = true;
            // 
            // thirdPartyLicenseControl1
            // 
            this.thirdPartyLicenseControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thirdPartyLicenseControl1.Location = new System.Drawing.Point(3, 3);
            this.thirdPartyLicenseControl1.Name = "thirdPartyLicenseControl1";
            this.thirdPartyLicenseControl1.Size = new System.Drawing.Size(364, 165);
            this.thirdPartyLicenseControl1.TabIndex = 0;
            // 
            // commandCancel
            // 
            this.commandCancel.AutoSize = true;
            this.commandCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.commandCancel.Location = new System.Drawing.Point(385, 3);
            this.commandCancel.Name = "commandCancel";
            this.commandCancel.Size = new System.Drawing.Size(90, 23);
            this.commandCancel.TabIndex = 1;
            this.commandCancel.Text = "使用しない(&N)";
            this.commandCancel.UseVisualStyleBackColor = true;
            this.commandCancel.Click += new System.EventHandler(this.commandCancel_Click);
            // 
            // selectedRead
            // 
            this.selectedRead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.selectedRead.AutoSize = true;
            this.selectedRead.Location = new System.Drawing.Point(3, 7);
            this.selectedRead.Name = "selectedRead";
            this.selectedRead.Size = new System.Drawing.Size(116, 16);
            this.selectedRead.TabIndex = 2;
            this.selectedRead.Text = "内容を許諾する(&A)";
            this.selectedRead.UseVisualStyleBackColor = true;
            this.selectedRead.CheckedChanged += new System.EventHandler(this.selectedRead_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabMain, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(484, 319);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.commandCancel, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.commanAccept, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.selectedRead, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 286);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(478, 30);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // commanAccept
            // 
            this.commanAccept.AutoSize = true;
            this.commanAccept.Location = new System.Drawing.Point(289, 3);
            this.commanAccept.Name = "commanAccept";
            this.commanAccept.Size = new System.Drawing.Size(90, 23);
            this.commanAccept.TabIndex = 1;
            this.commanAccept.Text = "使用する(&A)";
            this.commanAccept.UseVisualStyleBackColor = true;
            this.commanAccept.Click += new System.EventHandler(this.commanAccept_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(484, 319);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(484, 341);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(484, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // AcceptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.commandCancel;
            this.ClientSize = new System.Drawing.Size(484, 341);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AcceptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "<CODE>";
            this.tabMain.ResumeLayout(false);
            this.tabMainPageContent.ResumeLayout(false);
            this.tabMainPageLicense.ResumeLayout(false);
            this.tabMainPageThirdParty.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabMainPageContent;
        private System.Windows.Forms.Button commandCancel;
        private System.Windows.Forms.CheckBox selectedRead;
        private System.Windows.Forms.TabPage tabMainPageLicense;
        private System.Windows.Forms.RichTextBox viewLicense;
        private System.Windows.Forms.RichTextBox viewMessage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button commanAccept;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabPage tabMainPageThirdParty;
        private ThirdPartyLicenseControl thirdPartyLicenseControl1;
    }
}
