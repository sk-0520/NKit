﻿namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.commandClose = new System.Windows.Forms.Button();
            this.tabInformation = new System.Windows.Forms.TabControl();
            this.tabInformationPageThirdParty = new System.Windows.Forms.TabPage();
            this.tabInformationPageReleaseNote = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.commandData = new System.Windows.Forms.Button();
            this.commandApp = new System.Windows.Forms.Button();
            this.linkForum = new System.Windows.Forms.LinkLabel();
            this.linkDevelopment = new System.Windows.Forms.LinkLabel();
            this.linkWebsite = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.viewVersion = new System.Windows.Forms.TextBox();
            this.thirdPartyLicenseControl1 = new ContentTypeTextNet.NKit.Manager.View.ThirdPartyLicenseControl();
            this.releaseNoteControl1 = new ContentTypeTextNet.NKit.Manager.View.ReleaseNoteControl();
            this.commandVersionCopy = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabInformation.SuspendLayout();
            this.tabInformationPageThirdParty.SuspendLayout();
            this.tabInformationPageReleaseNote.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabInformation, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 312);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.commandClose);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 280);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(628, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // commandClose
            // 
            this.commandClose.Location = new System.Drawing.Point(550, 3);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(75, 23);
            this.commandClose.TabIndex = 0;
            this.commandClose.Text = "close";
            this.commandClose.UseVisualStyleBackColor = true;
            this.commandClose.Click += new System.EventHandler(this.commandClose_Click);
            // 
            // tabInformation
            // 
            this.tabInformation.Controls.Add(this.tabInformationPageThirdParty);
            this.tabInformation.Controls.Add(this.tabInformationPageReleaseNote);
            this.tabInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabInformation.Location = new System.Drawing.Point(320, 3);
            this.tabInformation.Name = "tabInformation";
            this.tabInformation.SelectedIndex = 0;
            this.tabInformation.Size = new System.Drawing.Size(311, 271);
            this.tabInformation.TabIndex = 1;
            // 
            // tabInformationPageThirdParty
            // 
            this.tabInformationPageThirdParty.Controls.Add(this.thirdPartyLicenseControl1);
            this.tabInformationPageThirdParty.Location = new System.Drawing.Point(4, 22);
            this.tabInformationPageThirdParty.Name = "tabInformationPageThirdParty";
            this.tabInformationPageThirdParty.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformationPageThirdParty.Size = new System.Drawing.Size(303, 245);
            this.tabInformationPageThirdParty.TabIndex = 0;
            this.tabInformationPageThirdParty.Text = "tabPage1";
            this.tabInformationPageThirdParty.UseVisualStyleBackColor = true;
            // 
            // tabInformationPageReleaseNote
            // 
            this.tabInformationPageReleaseNote.Controls.Add(this.releaseNoteControl1);
            this.tabInformationPageReleaseNote.Location = new System.Drawing.Point(4, 22);
            this.tabInformationPageReleaseNote.Name = "tabInformationPageReleaseNote";
            this.tabInformationPageReleaseNote.Padding = new System.Windows.Forms.Padding(3);
            this.tabInformationPageReleaseNote.Size = new System.Drawing.Size(303, 245);
            this.tabInformationPageReleaseNote.TabIndex = 1;
            this.tabInformationPageReleaseNote.Text = "tabPage2";
            this.tabInformationPageReleaseNote.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.commandVersionCopy);
            this.panel1.Controls.Add(this.viewVersion);
            this.panel1.Controls.Add(this.commandData);
            this.panel1.Controls.Add(this.commandApp);
            this.panel1.Controls.Add(this.linkForum);
            this.panel1.Controls.Add(this.linkDevelopment);
            this.panel1.Controls.Add(this.linkWebsite);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(311, 271);
            this.panel1.TabIndex = 2;
            // 
            // commandData
            // 
            this.commandData.Location = new System.Drawing.Point(221, 154);
            this.commandData.Name = "commandData";
            this.commandData.Size = new System.Drawing.Size(75, 23);
            this.commandData.TabIndex = 2;
            this.commandData.Text = "data";
            this.commandData.UseVisualStyleBackColor = true;
            this.commandData.Click += new System.EventHandler(this.commandData_Click);
            // 
            // commandApp
            // 
            this.commandApp.Location = new System.Drawing.Point(221, 125);
            this.commandApp.Name = "commandApp";
            this.commandApp.Size = new System.Drawing.Size(75, 23);
            this.commandApp.TabIndex = 2;
            this.commandApp.Text = "app";
            this.commandApp.UseVisualStyleBackColor = true;
            this.commandApp.Click += new System.EventHandler(this.commandApp_Click);
            // 
            // linkForum
            // 
            this.linkForum.AutoSize = true;
            this.linkForum.Location = new System.Drawing.Point(219, 98);
            this.linkForum.Name = "linkForum";
            this.linkForum.Size = new System.Drawing.Size(34, 12);
            this.linkForum.TabIndex = 1;
            this.linkForum.TabStop = true;
            this.linkForum.Text = "forum";
            this.linkForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkForum_LinkClicked);
            // 
            // linkDevelopment
            // 
            this.linkDevelopment.AutoSize = true;
            this.linkDevelopment.Location = new System.Drawing.Point(219, 63);
            this.linkDevelopment.Name = "linkDevelopment";
            this.linkDevelopment.Size = new System.Drawing.Size(69, 12);
            this.linkDevelopment.TabIndex = 1;
            this.linkDevelopment.TabStop = true;
            this.linkDevelopment.Text = "development";
            this.linkDevelopment.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDevelopment_LinkClicked);
            // 
            // linkWebsite
            // 
            this.linkWebsite.AutoSize = true;
            this.linkWebsite.Location = new System.Drawing.Point(219, 35);
            this.linkWebsite.Name = "linkWebsite";
            this.linkWebsite.Size = new System.Drawing.Size(48, 12);
            this.linkWebsite.TabIndex = 1;
            this.linkWebsite.TabStop = true;
            this.linkWebsite.Text = "web site";
            this.linkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWebsite_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ContentTypeTextNet.NKit.Manager.Properties.Resources.NKit_Manager;
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(192, 192);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(634, 312);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(634, 334);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(634, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(170, 228);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(170, 228);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // viewVersion
            // 
            this.viewVersion.Location = new System.Drawing.Point(22, 214);
            this.viewVersion.Name = "viewVersion";
            this.viewVersion.ReadOnly = true;
            this.viewVersion.Size = new System.Drawing.Size(155, 19);
            this.viewVersion.TabIndex = 3;
            // 
            // thirdPartyLicenseControl1
            // 
            this.thirdPartyLicenseControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thirdPartyLicenseControl1.Location = new System.Drawing.Point(3, 3);
            this.thirdPartyLicenseControl1.Name = "thirdPartyLicenseControl1";
            this.thirdPartyLicenseControl1.Size = new System.Drawing.Size(297, 239);
            this.thirdPartyLicenseControl1.TabIndex = 0;
            // 
            // releaseNoteControl1
            // 
            this.releaseNoteControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.releaseNoteControl1.IssueBaseUri = null;
            this.releaseNoteControl1.Location = new System.Drawing.Point(3, 3);
            this.releaseNoteControl1.MinimumSize = new System.Drawing.Size(20, 20);
            this.releaseNoteControl1.Name = "releaseNoteControl1";
            this.releaseNoteControl1.Size = new System.Drawing.Size(297, 239);
            this.releaseNoteControl1.TabIndex = 0;
            // 
            // commandVersionCopy
            // 
            this.commandVersionCopy.Location = new System.Drawing.Point(200, 226);
            this.commandVersionCopy.Name = "commandVersionCopy";
            this.commandVersionCopy.Size = new System.Drawing.Size(75, 23);
            this.commandVersionCopy.TabIndex = 4;
            this.commandVersionCopy.Text = "copy";
            this.commandVersionCopy.UseVisualStyleBackColor = true;
            this.commandVersionCopy.Click += new System.EventHandler(this.commandVersionCopy_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 334);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Text = "AboutForm";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabInformation.ResumeLayout(false);
            this.tabInformationPageThirdParty.ResumeLayout(false);
            this.tabInformationPageReleaseNote.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.TabControl tabInformation;
        private System.Windows.Forms.TabPage tabInformationPageThirdParty;
        private System.Windows.Forms.TabPage tabInformationPageReleaseNote;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ThirdPartyLicenseControl thirdPartyLicenseControl1;
        private ReleaseNoteControl releaseNoteControl1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkForum;
        private System.Windows.Forms.LinkLabel linkDevelopment;
        private System.Windows.Forms.LinkLabel linkWebsite;
        private System.Windows.Forms.Button commandData;
        private System.Windows.Forms.Button commandApp;
        private System.Windows.Forms.TextBox viewVersion;
        private System.Windows.Forms.Button commandVersionCopy;
    }
}