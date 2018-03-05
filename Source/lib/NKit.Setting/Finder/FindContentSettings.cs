using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.NKit.Setting.Finder
{
    /// <summary>
    /// ファイル内容検索の専用検索処理。
    /// </summary>
    public abstract class FindContentSettingBase : SettingBase
    {
        #region property

        /// <summary>
        /// 専用検索を行うか。
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        #endregion
    }

    public interface IReadOnlyFindMicrosoftOfficeContentSetting
    {
        #region proeprty

        /// <summary>
        /// 図形内の文字列を検索するか。
        /// </summary>
        bool TextInShape { get; }

        #endregion
    }

    public interface IReadOnlyFindMicrosoftOfficeExcelContentSetting : IReadOnlyFindMicrosoftOfficeContentSetting
    {
        /// <summary>
        /// シート名を対象とするか。
        /// </summary>
        bool SheetName { get; }
        /// <summary>
        /// 数式を優先するか。
        /// <para>Excelの場合にキャッシュデータではなく数式を検索対象とするか(A1とかを検索したい場合に使用)。</para>
        /// </summary>
        bool PriorityFormula { get; }
        /// <summary>
        /// セル内のコメントも対象とするか。
        /// </summary>
        bool CommentInCell { get; }
    }

    public interface IReadOnlyFindMicrosoftOfficeWordContentSetting : IReadOnlyFindMicrosoftOfficeContentSetting
    {
        #region property



        #endregion
    }

    public class FindMicrosoftOfficeContentSetting : FindContentSettingBase, IReadOnlyFindMicrosoftOfficeExcelContentSetting, IReadOnlyFindMicrosoftOfficeContentSetting, IReadOnlyFindMicrosoftOfficeWordContentSetting
    {
        #region IReadOnlyFindMicrosoftOfficeContentSetting

        public bool TextInShape { get; set; }

        #endregion

        #region IReadOnlyFindMicrosoftOfficeExcelContentSetting

        public bool SheetName { get; set; }
        public bool PriorityFormula { get; set; }
        public bool CommentInCell { get; set; }

        #endregion

        #region IReadOnlyFindMicrosoftOfficeWordContentSetting
        #endregion
    }

    public interface IReadOnlyFindXmlHtmlContentSetting
    {
        #region property

        /// <summary>
        /// 要素名を検索対象とするか。
        /// </summary>
        bool Element { get; }
        /// <summary>
        /// テキストノードを検索対象とするか。
        /// </summary>
        bool Text { get; }
        /// <summary>
        /// 属性のキーを検索対象とするか。
        /// </summary>
        bool AttributeKey { get; }
        /// <summary>
        /// 属性の値を検索対象とするか。
        /// </summary>
        bool AttributeValue { get; }
        /// <summary>
        /// コメントを検索対象とするか。
        /// </summary>
        bool Comment { get; }
        /// <summary>
        /// a[href], img[src] の値を検索対象外とするか。
        /// <para><see cref="AttributeKey"/>, <see cref="AttributeValue"/> に思いっきり影響される。</para>
        /// </summary>
        bool IgnoreHtmlLinkValue { get; }

        #endregion
    }

    /// <summary>
    /// Xml とか Html の検索処理。
    /// <para>xml ≠ html とかいいから別に。</para>
    /// </summary>
    public class FindXmlHtmlContentSetting : FindContentSettingBase, IReadOnlyFindXmlHtmlContentSetting
    {
        #region IReadOnlyFindXmlHtmlContentSetting

        public bool Element { get; set; } = true;
        public bool Text { get; set; } = true;
        public bool AttributeKey { get; set; } = true;
        public bool AttributeValue { get; set; } = true;
        public bool Comment { get; set; } = true;
        public bool IgnoreHtmlLinkValue { get; set; }

        #endregion
    }
}
