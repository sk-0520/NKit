namespace ContentTypeTextNet.NKit.Cameraman.View
{
    partial class CameramanForm
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
            this.hallArea = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // hallArea
            // 
            this.hallArea.BackColor = System.Drawing.Color.Black;
            this.hallArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hallArea.Location = new System.Drawing.Point(5, 5);
            this.hallArea.Margin = new System.Windows.Forms.Padding(0);
            this.hallArea.Name = "hallArea";
            this.hallArea.Size = new System.Drawing.Size(167, 52);
            this.hallArea.TabIndex = 0;
            // 
            // CameramanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(177, 62);
            this.Controls.Add(this.hallArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CameramanForm";
            this.Opacity = 0D;
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel hallArea;
    }
}

