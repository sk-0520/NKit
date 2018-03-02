using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.CompatibleForms;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class DialogFilterItem
    {
        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="displayText">表示文字列。</param>
        /// <param name="wildcard">ワイルドカード一覧。</param>
        public DialogFilterItem(string displayText, IEnumerable<string> wildcard)
        {
            DisplayText = displayText;
            Wildcard = new List<string>(wildcard);
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="displayText">表示文字列。</param>
        /// <param name="wildcard">ワイルドカード一覧。</param>
        public DialogFilterItem(string displayText, params string[] wildcard)
            : this(displayText, (IEnumerable<string>)wildcard)
        { }

        #region property

        /// <summary>
        /// フィルタリングに使用するワイルドカード一覧。
        /// </summary>
        public IReadOnlyList<string> Wildcard { get; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayText { get; }

        /// <summary>
        /// ダイアログフィルタとして使用する生値。
        /// </summary>
        /// <returns></returns>
        public virtual string FilterText => string.Format("{0}|{1}", DisplayText, string.Join(";", Wildcard));

        #endregion
    }

    /// <summary>
    /// ダイアログで使用するフィルタのアイテム。
    /// <para>値を保持する。</para>
    /// </summary>
    public class DialogFilterValueItem<TValue> : DialogFilterItem
    {
        public DialogFilterValueItem(TValue value, string displayText, IEnumerable<string> wildcard)
            : base(displayText, wildcard)
        {
            Value = value;
        }

        public DialogFilterValueItem(TValue value, string displayText, params string[] wildcard)
            : base(displayText, wildcard)
        {
            Value = value;
        }

        #region property

        /// <summary>
        /// 保持する値。
        /// </summary>
        public TValue Value { get; }

        #endregion

    }

    public class DialogFilterList : List<DialogFilterItem>
    {
        public string FilterText => string.Join("|", this.Select(i => i.FilterText));
    }

    public class Dialogs
    {
        public FolderBrowserDialog CreateFolderDialog(string selectedPath, Environment.SpecialFolder rootDirectory = Environment.SpecialFolder.Desktop)
        {
            var dialog = new FolderBrowserDialog() {
                ShowNewFolderButton = true,
                RootFolder = rootDirectory,
                SelectedPath = selectedPath,
            };

            return dialog;
        }
    }
}
