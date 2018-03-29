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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
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
            this.selectLogging = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.commandCheckUpdate = new System.Windows.Forms.Button();
            this.commandExecuteUpdate = new System.Windows.Forms.Button();
            this.commandShowReleaseNote = new System.Windows.Forms.Button();
            this.commandTestExecute = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.selectWorkspaceLoadToMinimize = new System.Windows.Forms.CheckBox();
            this.selectWorkspaceRunningMinimizeToNotifyArea = new System.Windows.Forms.CheckBox();
            this.commandShowAbout = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.viewLog = new ContentTypeTextNet.NKit.Manager.View.LogListView();
            this.viewLogColumnNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewLogColumnTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewLogColumnKind = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewLogColumnSender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewLogColumnSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewLogColumnMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.selectLogTrace = new System.Windows.Forms.CheckBox();
            this.selectLogDebug = new System.Windows.Forms.CheckBox();
            this.selectLogInformation = new System.Windows.Forms.CheckBox();
            this.selectLogWarning = new System.Windows.Forms.CheckBox();
            this.selectLogError = new System.Windows.Forms.CheckBox();
            this.selectLogFatal = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusbar = new System.Windows.Forms.StatusStrip();
            this.statusbarLabelBuildType = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusbarLabelVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusbarLabelHash = new System.Windows.Forms.ToolStripStatusLabel();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.commandLogClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusbar.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectWorkspace
            // 
            this.selectWorkspace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.selectWorkspace, 3);
            this.selectWorkspace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectWorkspace.FormattingEnabled = true;
            this.selectWorkspace.Location = new System.Drawing.Point(98, 3);
            this.selectWorkspace.Name = "selectWorkspace";
            this.selectWorkspace.Size = new System.Drawing.Size(468, 20);
            this.selectWorkspace.TabIndex = 1;
            this.selectWorkspace.SelectedIndexChanged += new System.EventHandler(this.selectWorkspace_SelectedIndexChanged);
            // 
            // commandWorkspaceLoad
            // 
            this.commandWorkspaceLoad.Location = new System.Drawing.Point(98, 29);
            this.commandWorkspaceLoad.Name = "commandWorkspaceLoad";
            this.commandWorkspaceLoad.Size = new System.Drawing.Size(85, 23);
            this.commandWorkspaceLoad.TabIndex = 2;
            this.commandWorkspaceLoad.Text = "読み込み(&L)";
            this.commandWorkspaceLoad.UseVisualStyleBackColor = true;
            this.commandWorkspaceLoad.Click += new System.EventHandler(this.commandWorkspaceLoad_Click);
            // 
            // commandWorkspaceClose
            // 
            this.commandWorkspaceClose.Location = new System.Drawing.Point(98, 58);
            this.commandWorkspaceClose.Name = "commandWorkspaceClose";
            this.commandWorkspaceClose.Size = new System.Drawing.Size(85, 23);
            this.commandWorkspaceClose.TabIndex = 6;
            this.commandWorkspaceClose.Text = "閉じる(&X)";
            this.commandWorkspaceClose.UseVisualStyleBackColor = true;
            this.commandWorkspaceClose.Click += new System.EventHandler(this.commandWorkspaceClose_Click);
            // 
            // commandWorkspaceCreate
            // 
            this.commandWorkspaceCreate.Location = new System.Drawing.Point(481, 29);
            this.commandWorkspaceCreate.Name = "commandWorkspaceCreate";
            this.commandWorkspaceCreate.Size = new System.Drawing.Size(85, 23);
            this.commandWorkspaceCreate.TabIndex = 4;
            this.commandWorkspaceCreate.Text = "作成(&N)";
            this.commandWorkspaceCreate.UseVisualStyleBackColor = true;
            this.commandWorkspaceCreate.Click += new System.EventHandler(this.commandWorkspaceCreate_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名前(&N):";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "フォルダ(&F):";
            // 
            // commandWorkspaceSave
            // 
            this.commandWorkspaceSave.Location = new System.Drawing.Point(3, 3);
            this.commandWorkspaceSave.Name = "commandWorkspaceSave";
            this.commandWorkspaceSave.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceSave.TabIndex = 0;
            this.commandWorkspaceSave.Text = "保存(&S)";
            this.commandWorkspaceSave.UseVisualStyleBackColor = true;
            this.commandWorkspaceSave.Click += new System.EventHandler(this.commandWorkspaceSave_Click);
            // 
            // commandWorkspaceDirectorySelect
            // 
            this.commandWorkspaceDirectorySelect.Image = global::ContentTypeTextNet.NKit.Manager.Properties.Resources.Image_FolderOpen;
            this.commandWorkspaceDirectorySelect.Location = new System.Drawing.Point(218, 28);
            this.commandWorkspaceDirectorySelect.Name = "commandWorkspaceDirectorySelect";
            this.commandWorkspaceDirectorySelect.Size = new System.Drawing.Size(32, 23);
            this.commandWorkspaceDirectorySelect.TabIndex = 4;
            this.commandWorkspaceDirectorySelect.UseVisualStyleBackColor = true;
            this.commandWorkspaceDirectorySelect.Click += new System.EventHandler(this.commandWorkspaceDirectorySelect_Click);
            // 
            // inputWorkspaceName
            // 
            this.inputWorkspaceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.inputWorkspaceName.Location = new System.Drawing.Point(66, 3);
            this.inputWorkspaceName.Name = "inputWorkspaceName";
            this.inputWorkspaceName.Size = new System.Drawing.Size(146, 19);
            this.inputWorkspaceName.TabIndex = 1;
            // 
            // inputWorkspaceDirectoryPath
            // 
            this.inputWorkspaceDirectoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.inputWorkspaceDirectoryPath.Location = new System.Drawing.Point(66, 30);
            this.inputWorkspaceDirectoryPath.Name = "inputWorkspaceDirectoryPath";
            this.inputWorkspaceDirectoryPath.Size = new System.Drawing.Size(146, 19);
            this.inputWorkspaceDirectoryPath.TabIndex = 3;
            // 
            // commandWorkspaceCopy
            // 
            this.commandWorkspaceCopy.Location = new System.Drawing.Point(481, 58);
            this.commandWorkspaceCopy.Name = "commandWorkspaceCopy";
            this.commandWorkspaceCopy.Size = new System.Drawing.Size(85, 23);
            this.commandWorkspaceCopy.TabIndex = 5;
            this.commandWorkspaceCopy.Text = "コピー(&C)";
            this.commandWorkspaceCopy.UseVisualStyleBackColor = true;
            this.commandWorkspaceCopy.Click += new System.EventHandler(this.commandWorkspaceCopy_Click);
            // 
            // commandWorkspaceDelete
            // 
            this.commandWorkspaceDelete.Location = new System.Drawing.Point(84, 3);
            this.commandWorkspaceDelete.Name = "commandWorkspaceDelete";
            this.commandWorkspaceDelete.Size = new System.Drawing.Size(75, 23);
            this.commandWorkspaceDelete.TabIndex = 1;
            this.commandWorkspaceDelete.Text = "削除(&D)";
            this.commandWorkspaceDelete.UseVisualStyleBackColor = true;
            this.commandWorkspaceDelete.Click += new System.EventHandler(this.commandWorkspaceDelete_Click);
            // 
            // selectLogging
            // 
            this.selectLogging.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.selectLogging, 3);
            this.selectLogging.Location = new System.Drawing.Point(3, 57);
            this.selectLogging.Name = "selectLogging";
            this.selectLogging.Size = new System.Drawing.Size(108, 16);
            this.selectLogging.TabIndex = 5;
            this.selectLogging.Text = "ログを出力する(&L)";
            this.selectLogging.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // commandCheckUpdate
            // 
            this.commandCheckUpdate.Location = new System.Drawing.Point(219, 3);
            this.commandCheckUpdate.Name = "commandCheckUpdate";
            this.commandCheckUpdate.Size = new System.Drawing.Size(102, 23);
            this.commandCheckUpdate.TabIndex = 2;
            this.commandCheckUpdate.Text = "アップデート確認(&H)";
            this.commandCheckUpdate.UseVisualStyleBackColor = true;
            this.commandCheckUpdate.Click += new System.EventHandler(this.commandCheckUpdate_Click);
            // 
            // commandExecuteUpdate
            // 
            this.commandExecuteUpdate.Location = new System.Drawing.Point(435, 3);
            this.commandExecuteUpdate.Name = "commandExecuteUpdate";
            this.commandExecuteUpdate.Size = new System.Drawing.Size(102, 23);
            this.commandExecuteUpdate.TabIndex = 4;
            this.commandExecuteUpdate.Text = "アップデート実行(&U)";
            this.commandExecuteUpdate.UseVisualStyleBackColor = true;
            this.commandExecuteUpdate.Click += new System.EventHandler(this.commandExecuteUpdate_Click);
            // 
            // commandShowReleaseNote
            // 
            this.commandShowReleaseNote.Location = new System.Drawing.Point(327, 3);
            this.commandShowReleaseNote.Name = "commandShowReleaseNote";
            this.commandShowReleaseNote.Size = new System.Drawing.Size(102, 23);
            this.commandShowReleaseNote.TabIndex = 3;
            this.commandShowReleaseNote.Text = "リリースノート(&R)";
            this.commandShowReleaseNote.UseVisualStyleBackColor = true;
            this.commandShowReleaseNote.Click += new System.EventHandler(this.commandShowReleaseNote_Click);
            // 
            // commandTestExecute
            // 
            this.commandTestExecute.Location = new System.Drawing.Point(3, 3);
            this.commandTestExecute.Name = "commandTestExecute";
            this.commandTestExecute.Size = new System.Drawing.Size(102, 23);
            this.commandTestExecute.TabIndex = 0;
            this.commandTestExecute.Text = "試験起動(&E)";
            this.commandTestExecute.UseVisualStyleBackColor = true;
            this.commandTestExecute.Click += new System.EventHandler(this.commandTestExecute_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // selectWorkspaceLoadToMinimize
            // 
            this.selectWorkspaceLoadToMinimize.AutoSize = true;
            this.selectWorkspaceLoadToMinimize.Location = new System.Drawing.Point(3, 3);
            this.selectWorkspaceLoadToMinimize.Name = "selectWorkspaceLoadToMinimize";
            this.selectWorkspaceLoadToMinimize.Size = new System.Drawing.Size(211, 16);
            this.selectWorkspaceLoadToMinimize.TabIndex = 0;
            this.selectWorkspaceLoadToMinimize.Text = "ワークスペース読み込み時に最小化する";
            this.selectWorkspaceLoadToMinimize.UseVisualStyleBackColor = true;
            this.selectWorkspaceLoadToMinimize.CheckedChanged += new System.EventHandler(this.selectWorkspaceLoadToMinimize_CheckedChanged);
            // 
            // selectWorkspaceRunningMinimizeToNotifyArea
            // 
            this.selectWorkspaceRunningMinimizeToNotifyArea.AutoSize = true;
            this.selectWorkspaceRunningMinimizeToNotifyArea.Location = new System.Drawing.Point(3, 25);
            this.selectWorkspaceRunningMinimizeToNotifyArea.Name = "selectWorkspaceRunningMinimizeToNotifyArea";
            this.selectWorkspaceRunningMinimizeToNotifyArea.Size = new System.Drawing.Size(280, 16);
            this.selectWorkspaceRunningMinimizeToNotifyArea.TabIndex = 1;
            this.selectWorkspaceRunningMinimizeToNotifyArea.Text = "ワークスペース実行中に最小化で通知領域に移動する";
            this.selectWorkspaceRunningMinimizeToNotifyArea.UseVisualStyleBackColor = true;
            this.selectWorkspaceRunningMinimizeToNotifyArea.CheckedChanged += new System.EventHandler(this.selectWorkspaceRunningMinimizeToNotifyArea_CheckedChanged);
            // 
            // commandShowAbout
            // 
            this.commandShowAbout.Location = new System.Drawing.Point(111, 3);
            this.commandShowAbout.Name = "commandShowAbout";
            this.commandShowAbout.Size = new System.Drawing.Size(102, 23);
            this.commandShowAbout.TabIndex = 1;
            this.commandShowAbout.Text = "<CODE:ABOUT>";
            this.commandShowAbout.UseVisualStyleBackColor = true;
            this.commandShowAbout.Click += new System.EventHandler(this.commandShowAbout_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(578, 3);
            this.groupBox1.Name = "groupBox1";
            this.tableLayoutPanel4.SetRowSpan(this.groupBox1, 2);
            this.groupBox1.Size = new System.Drawing.Size(259, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ワークスペース設定";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.Controls.Add(this.selectLogging, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.inputWorkspaceName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.commandWorkspaceDirectorySelect, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.inputWorkspaceDirectoryPath, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(253, 111);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this.commandWorkspaceSave);
            this.flowLayoutPanel1.Controls.Add(this.commandWorkspaceDelete);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 79);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(247, 29);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.selectWorkspace, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.commandWorkspaceLoad, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.commandWorkspaceClose, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.commandWorkspaceCopy, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.commandWorkspaceCreate, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel2, 2, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(569, 84);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "ワークスペース(&W):";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.selectWorkspaceLoadToMinimize);
            this.flowLayoutPanel2.Controls.Add(this.selectWorkspaceRunningMinimizeToNotifyArea);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(189, 29);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.tableLayoutPanel2.SetRowSpan(this.flowLayoutPanel2, 2);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(286, 44);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.commandTestExecute);
            this.flowLayoutPanel3.Controls.Add(this.commandShowAbout);
            this.flowLayoutPanel3.Controls.Add(this.commandCheckUpdate);
            this.flowLayoutPanel3.Controls.Add(this.commandShowReleaseNote);
            this.flowLayoutPanel3.Controls.Add(this.commandExecuteUpdate);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(17, 98);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(540, 29);
            this.flowLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel4.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.viewLog, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 138);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(838, 98);
            this.tableLayoutPanel3.TabIndex = 9;
            // 
            // viewLog
            // 
            this.viewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.viewLogColumnNumber,
            this.viewLogColumnTimestamp,
            this.viewLogColumnKind,
            this.viewLogColumnSender,
            this.viewLogColumnSubject,
            this.viewLogColumnMessage});
            this.viewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewLog.GridLines = true;
            this.viewLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.viewLog.Location = new System.Drawing.Point(96, 3);
            this.viewLog.MultiSelect = false;
            this.viewLog.Name = "viewLog";
            this.viewLog.ShowGroups = false;
            this.viewLog.ShowItemToolTips = true;
            this.viewLog.Size = new System.Drawing.Size(739, 92);
            this.viewLog.TabIndex = 0;
            this.viewLog.UseCompatibleStateImageBehavior = false;
            this.viewLog.View = System.Windows.Forms.View.Details;
            this.viewLog.VirtualMode = true;
            this.viewLog.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.viewLog_ItemSelectionChanged);
            this.viewLog.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.viewLog_RetrieveVirtualItem);
            // 
            // viewLogColumnNumber
            // 
            this.viewLogColumnNumber.Text = "#";
            // 
            // viewLogColumnTimestamp
            // 
            this.viewLogColumnTimestamp.Text = "タイムスタンプ";
            this.viewLogColumnTimestamp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // viewLogColumnKind
            // 
            this.viewLogColumnKind.Text = "種別";
            this.viewLogColumnKind.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // viewLogColumnSender
            // 
            this.viewLogColumnSender.Text = "送信元";
            // 
            // viewLogColumnSubject
            // 
            this.viewLogColumnSubject.Text = "グループ";
            // 
            // viewLogColumnMessage
            // 
            this.viewLogColumnMessage.Text = "メッセージ";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel5, 0, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(87, 92);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.flowLayoutPanel4);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(72, 51);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "表示ログ";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoScroll = true;
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.selectLogTrace);
            this.flowLayoutPanel4.Controls.Add(this.selectLogDebug);
            this.flowLayoutPanel4.Controls.Add(this.selectLogInformation);
            this.flowLayoutPanel4.Controls.Add(this.selectLogWarning);
            this.flowLayoutPanel4.Controls.Add(this.selectLogError);
            this.flowLayoutPanel4.Controls.Add(this.selectLogFatal);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 15);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(66, 33);
            this.flowLayoutPanel4.TabIndex = 0;
            this.flowLayoutPanel4.WrapContents = false;
            // 
            // selectLogTrace
            // 
            this.selectLogTrace.AutoSize = true;
            this.selectLogTrace.Location = new System.Drawing.Point(3, 3);
            this.selectLogTrace.Name = "selectLogTrace";
            this.selectLogTrace.Size = new System.Drawing.Size(60, 16);
            this.selectLogTrace.TabIndex = 0;
            this.selectLogTrace.Text = "トレース";
            this.selectLogTrace.UseVisualStyleBackColor = true;
            this.selectLogTrace.CheckedChanged += new System.EventHandler(this.selectLogTrace_CheckedChanged);
            // 
            // selectLogDebug
            // 
            this.selectLogDebug.AutoSize = true;
            this.selectLogDebug.Location = new System.Drawing.Point(3, 25);
            this.selectLogDebug.Name = "selectLogDebug";
            this.selectLogDebug.Size = new System.Drawing.Size(60, 16);
            this.selectLogDebug.TabIndex = 0;
            this.selectLogDebug.Text = "デバッグ";
            this.selectLogDebug.UseVisualStyleBackColor = true;
            this.selectLogDebug.CheckedChanged += new System.EventHandler(this.selectLogDebug_CheckedChanged);
            // 
            // selectLogInformation
            // 
            this.selectLogInformation.AutoSize = true;
            this.selectLogInformation.Location = new System.Drawing.Point(3, 47);
            this.selectLogInformation.Name = "selectLogInformation";
            this.selectLogInformation.Size = new System.Drawing.Size(48, 16);
            this.selectLogInformation.TabIndex = 0;
            this.selectLogInformation.Text = "情報";
            this.selectLogInformation.UseVisualStyleBackColor = true;
            this.selectLogInformation.CheckedChanged += new System.EventHandler(this.selectLogInformation_CheckedChanged);
            // 
            // selectLogWarning
            // 
            this.selectLogWarning.AutoSize = true;
            this.selectLogWarning.Location = new System.Drawing.Point(3, 69);
            this.selectLogWarning.Name = "selectLogWarning";
            this.selectLogWarning.Size = new System.Drawing.Size(48, 16);
            this.selectLogWarning.TabIndex = 0;
            this.selectLogWarning.Text = "警告";
            this.selectLogWarning.UseVisualStyleBackColor = true;
            this.selectLogWarning.CheckedChanged += new System.EventHandler(this.selectLogWarning_CheckedChanged);
            // 
            // selectLogError
            // 
            this.selectLogError.AutoSize = true;
            this.selectLogError.Location = new System.Drawing.Point(3, 91);
            this.selectLogError.Name = "selectLogError";
            this.selectLogError.Size = new System.Drawing.Size(51, 16);
            this.selectLogError.TabIndex = 0;
            this.selectLogError.Text = "エラー";
            this.selectLogError.UseVisualStyleBackColor = true;
            this.selectLogError.CheckedChanged += new System.EventHandler(this.selectLogError_CheckedChanged);
            // 
            // selectLogFatal
            // 
            this.selectLogFatal.AutoSize = true;
            this.selectLogFatal.Location = new System.Drawing.Point(3, 113);
            this.selectLogFatal.Name = "selectLogFatal";
            this.selectLogFatal.Size = new System.Drawing.Size(48, 16);
            this.selectLogFatal.TabIndex = 0;
            this.selectLogFatal.Text = "致命";
            this.selectLogFatal.UseVisualStyleBackColor = true;
            this.selectLogFatal.CheckedChanged += new System.EventHandler(this.selectLogFatal_CheckedChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel3, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(844, 239);
            this.tableLayoutPanel4.TabIndex = 10;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusbar);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel4);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(844, 239);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(844, 261);
            this.toolStripContainer1.TabIndex = 11;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // statusbar
            // 
            this.statusbar.Dock = System.Windows.Forms.DockStyle.None;
            this.statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusbarLabelBuildType,
            this.statusbarLabelVersion,
            this.statusbarLabelHash});
            this.statusbar.Location = new System.Drawing.Point(0, 0);
            this.statusbar.Name = "statusbar";
            this.statusbar.Size = new System.Drawing.Size(844, 22);
            this.statusbar.TabIndex = 0;
            // 
            // statusbarLabelBuildType
            // 
            this.statusbarLabelBuildType.Name = "statusbarLabelBuildType";
            this.statusbarLabelBuildType.Size = new System.Drawing.Size(88, 17);
            this.statusbarLabelBuildType.Text = "<CODE:BUILD>";
            // 
            // statusbarLabelVersion
            // 
            this.statusbarLabelVersion.Name = "statusbarLabelVersion";
            this.statusbarLabelVersion.Size = new System.Drawing.Size(103, 17);
            this.statusbarLabelVersion.Text = "<CODE:VERSION>";
            // 
            // statusbarLabelHash
            // 
            this.statusbarLabelHash.Name = "statusbarLabelHash";
            this.statusbarLabelHash.Size = new System.Drawing.Size(88, 17);
            this.statusbarLabelHash.Text = "<CODE:HASH>";
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel5.Controls.Add(this.commandLogClear);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 60);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(81, 29);
            this.flowLayoutPanel5.TabIndex = 1;
            // 
            // commandLogClear
            // 
            this.commandLogClear.AutoSize = true;
            this.commandLogClear.Location = new System.Drawing.Point(3, 3);
            this.commandLogClear.Name = "commandLogClear";
            this.commandLogClear.Size = new System.Drawing.Size(75, 23);
            this.commandLogClear.TabIndex = 0;
            this.commandLogClear.Text = "ログクリア";
            this.commandLogClear.UseVisualStyleBackColor = true;
            this.commandLogClear.Click += new System.EventHandler(this.commandLogClear_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 261);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "<CODE>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.Shown += new System.EventHandler(this.ManagerForm_Shown);
            this.SizeChanged += new System.EventHandler(this.ManagerForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusbar.ResumeLayout(false);
            this.statusbar.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox selectLogging;
        private System.Windows.Forms.Button commandExecuteUpdate;
        private System.Windows.Forms.Button commandCheckUpdate;
        private System.Windows.Forms.Button commandShowReleaseNote;
        private System.Windows.Forms.Button commandTestExecute;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.CheckBox selectWorkspaceLoadToMinimize;
        private System.Windows.Forms.CheckBox selectWorkspaceRunningMinimizeToNotifyArea;
        private System.Windows.Forms.Button commandShowAbout;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusbar;
        private System.Windows.Forms.ToolStripStatusLabel statusbarLabelBuildType;
        private System.Windows.Forms.ToolStripStatusLabel statusbarLabelVersion;
        private System.Windows.Forms.ToolStripStatusLabel statusbarLabelHash;
        private LogListView viewLog;
        private System.Windows.Forms.ColumnHeader viewLogColumnTimestamp;
        private System.Windows.Forms.ColumnHeader viewLogColumnKind;
        private System.Windows.Forms.ColumnHeader viewLogColumnSender;
        private System.Windows.Forms.ColumnHeader viewLogColumnSubject;
        private System.Windows.Forms.ColumnHeader viewLogColumnMessage;
        private System.Windows.Forms.ColumnHeader viewLogColumnNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.CheckBox selectLogTrace;
        private System.Windows.Forms.CheckBox selectLogDebug;
        private System.Windows.Forms.CheckBox selectLogInformation;
        private System.Windows.Forms.CheckBox selectLogWarning;
        private System.Windows.Forms.CheckBox selectLogError;
        private System.Windows.Forms.CheckBox selectLogFatal;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Button commandLogClear;
    }
}

