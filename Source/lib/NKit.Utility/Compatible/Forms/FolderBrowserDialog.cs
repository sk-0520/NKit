using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.Compatible.Forms
{
    /// <summary>
    /// <see cref="System.Windows.Forms.FolderBrowserDialog"/>互換クラス。
    /// </summary>
    public class FolderBrowserDialog : ModelBase
    {
        public FolderBrowserDialog()
            : base()
        {
            this.Dialog = new System.Windows.Forms.FolderBrowserDialog();
        }

        #region property

        System.Windows.Forms.FolderBrowserDialog Dialog { get; set; }

        /// <summary>
        ///  <see cref="Component"/> を格納している <see cref="IContainer"/> を取得します。
        /// </summary>
        [BrowsableAttribute(false)]
        public IContainer Container { get { return Dialog.Container; } }

        /// <summary>
        /// ダイアログ ボックスのツリー ビュー コントロールの上部に表示する説明テキストを取得または設定します。
        /// </summary>
        [BrowsableAttribute(true)]
        public string Description
        {
            get { return Dialog.Description; }
            set { this.Description = value; }
        }

        /// <summary>
        /// 参照の開始位置とするルート フォルダーを取得または設定します。
        /// </summary>
        [BrowsableAttribute(true)]
        public Environment.SpecialFolder RootFolder
        {
            get { return Dialog.RootFolder; }
            set { Dialog.RootFolder = value; }
        }

        /// <summary>
        /// ユーザーが選択したパスを取得または設定します。
        /// </summary>
        [BrowsableAttribute(true)]
        public string SelectedPath
        {
            get { return Dialog.SelectedPath; }
            set { Dialog.SelectedPath = value; }
        }

        /// <summary>
        /// フォルダー参照ダイアログ ボックスに [新しいフォルダー] ボタンを表示するかどうかを示す値を取得または設定します。
        /// </summary>
        [BrowsableAttribute(true)]
        public bool ShowNewFolderButton
        {
            get { return Dialog.ShowNewFolderButton; }
            set { Dialog.ShowNewFolderButton = value; }
        }

        /// <summary>
        /// <see cref="Component"/> の <see cref="ISite"/> を取得または設定します。
        /// </summary>
        [BrowsableAttribute(false)]
        public virtual ISite Site
        {
            get { return Dialog.Site; }
            set { Dialog.Site = value; }
        }

        /// <summary>
        /// コントロールに関するデータを格納するオブジェクトを取得または設定します。
        /// </summary>
        [BindableAttribute(true)]
        public Object Tag
        {
            get { return Dialog.Tag; }
            set { Dialog.Tag = value; }
        }

        #endregion

        #region function

        public bool? ShowDialog()
        {
            var compatibleresult = Dialog.ShowDialog();
            return compatibleresult== System.Windows.Forms.DialogResult.OK;
        }

        public bool? ShowDialog(System.Windows.Window owner)
        {
            var form = new CompatibleFormWindow(owner);
            var compatibleresult = Dialog.ShowDialog(form);
            return compatibleresult == System.Windows.Forms.DialogResult.OK;
        }

        #endregion

        #region DisposeFinalizeBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(Dialog != null) {
                    Dialog.Dispose();
                    Dialog = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
