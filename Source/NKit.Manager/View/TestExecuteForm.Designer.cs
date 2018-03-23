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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestExecuteForm));
            this.commandExecute = new System.Windows.Forms.Button();
            this.commandClose = new System.Windows.Forms.Button();
            this.listApplications = new System.Windows.Forms.ListView();
            this.columnKind = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandExecute
            // 
            this.commandExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.commandExecute.AutoSize = true;
            this.commandExecute.Location = new System.Drawing.Point(3, 230);
            this.commandExecute.Name = "commandExecute";
            this.commandExecute.Size = new System.Drawing.Size(80, 22);
            this.commandExecute.TabIndex = 0;
            this.commandExecute.Text = "起動テスト(&E)";
            this.commandExecute.UseVisualStyleBackColor = true;
            this.commandExecute.Click += new System.EventHandler(this.commandExecute_Click);
            // 
            // commandClose
            // 
            this.commandClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commandClose.AutoSize = true;
            this.commandClose.Location = new System.Drawing.Point(185, 230);
            this.commandClose.Name = "commandClose";
            this.commandClose.Size = new System.Drawing.Size(80, 22);
            this.commandClose.TabIndex = 0;
            this.commandClose.Text = "閉じる(&C)";
            this.commandClose.UseVisualStyleBackColor = true;
            this.commandClose.Click += new System.EventHandler(this.commandClose_Click);
            // 
            // listApplications
            // 
            this.listApplications.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnKind,
            this.columnName,
            this.columnState});
            this.tableLayoutPanel1.SetColumnSpan(this.listApplications, 2);
            this.listApplications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listApplications.FullRowSelect = true;
            this.listApplications.GridLines = true;
            this.listApplications.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listApplications.HideSelection = false;
            this.listApplications.Location = new System.Drawing.Point(3, 44);
            this.listApplications.Margin = new System.Windows.Forms.Padding(3, 8, 3, 16);
            this.listApplications.MultiSelect = false;
            this.listApplications.Name = "listApplications";
            this.listApplications.Size = new System.Drawing.Size(262, 167);
            this.listApplications.TabIndex = 1;
            this.listApplications.UseCompatibleStateImageBehavior = false;
            this.listApplications.View = System.Windows.Forms.View.Details;
            // 
            // columnKind
            // 
            this.columnKind.Text = "種類";
            // 
            // columnName
            // 
            this.columnName.Text = "プログラム";
            // 
            // columnState
            // 
            this.columnState.Text = "状態";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.listApplications, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.commandClose, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.commandExecute, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(268, 255);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 36);
            this.label1.TabIndex = 2;
            this.label1.Text = "本処理はアンチウィルスソフトウェアや実行監視ソフトウェアに対して前もって関連プログラムを実行を試験するための機能です。";
            // 
            // TestExecuteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 271);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestExecuteForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<CODE>";
            this.Shown += new System.EventHandler(this.TestExecuteForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button commandExecute;
        private System.Windows.Forms.Button commandClose;
        private System.Windows.Forms.ListView listApplications;
        private System.Windows.Forms.ColumnHeader columnKind;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnState;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}