namespace ContentTypeTextNet.NKit.Cameraman.View
{
    partial class NavigationControl
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTakeShotKey = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.linkSelectKey = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.labelContinuation = new System.Windows.Forms.Label();
            this.linkExitKey = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "対象選択キー";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelTakeShotKey, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelContinuation, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.linkExitKey, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 150);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "キャプチャキー";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "終了キー";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "連続キャプチャ";
            // 
            // labelTakeShotKey
            // 
            this.labelTakeShotKey.AutoSize = true;
            this.labelTakeShotKey.Location = new System.Drawing.Point(82, 24);
            this.labelTakeShotKey.Name = "labelTakeShotKey";
            this.labelTakeShotKey.Size = new System.Drawing.Size(80, 12);
            this.labelTakeShotKey.TabIndex = 1;
            this.labelTakeShotKey.Text = "<CODE:SHOT>";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.linkSelectKey);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(79, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(113, 24);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // linkSelectKey
            // 
            this.linkSelectKey.AutoSize = true;
            this.linkSelectKey.Location = new System.Drawing.Point(3, 0);
            this.linkSelectKey.Name = "linkSelectKey";
            this.linkSelectKey.Size = new System.Drawing.Size(92, 12);
            this.linkSelectKey.TabIndex = 3;
            this.linkSelectKey.TabStop = true;
            this.linkSelectKey.Text = "<CODE:SELECT>";
            this.linkSelectKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSelectKey_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "二回押下でキャプチャ";
            // 
            // labelContinuation
            // 
            this.labelContinuation.AutoSize = true;
            this.labelContinuation.Location = new System.Drawing.Point(82, 48);
            this.labelContinuation.Name = "labelContinuation";
            this.labelContinuation.Size = new System.Drawing.Size(134, 12);
            this.labelContinuation.TabIndex = 1;
            this.labelContinuation.Text = "<CODE:CONTINUATION>";
            // 
            // linkExitKey
            // 
            this.linkExitKey.AutoSize = true;
            this.linkExitKey.Location = new System.Drawing.Point(82, 36);
            this.linkExitKey.Name = "linkExitKey";
            this.linkExitKey.Size = new System.Drawing.Size(74, 12);
            this.linkExitKey.TabIndex = 3;
            this.linkExitKey.TabStop = true;
            this.linkExitKey.Text = "<CODE:EXIT>";
            this.linkExitKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExitKey_LinkClicked);
            // 
            // NavigationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NavigationControl";
            this.Size = new System.Drawing.Size(231, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelTakeShotKey;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelContinuation;
        private System.Windows.Forms.LinkLabel linkExitKey;
        private System.Windows.Forms.LinkLabel linkSelectKey;
    }
}
