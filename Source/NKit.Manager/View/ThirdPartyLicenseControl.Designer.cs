namespace ContentTypeTextNet.NKit.Manager.View
{
    partial class ThirdPartyLicenseControl
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
            this.viewComponents = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // viewComponents
            // 
            this.viewComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewComponents.Location = new System.Drawing.Point(0, 0);
            this.viewComponents.Multiline = true;
            this.viewComponents.Name = "viewComponents";
            this.viewComponents.ReadOnly = true;
            this.viewComponents.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.viewComponents.Size = new System.Drawing.Size(150, 150);
            this.viewComponents.TabIndex = 0;
            this.viewComponents.WordWrap = false;
            // 
            // ThirdPartyLicenseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewComponents);
            this.Name = "ThirdPartyLicenseControl";
            this.Load += new System.EventHandler(this.ThirdPartyLicenseControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox viewComponents;
    }
}
