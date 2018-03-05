using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindGroupSetting : SettingBase
    {
        #region property

        /// <summary>
        /// 検索名。
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 検索対象ディレクトリパス。
        /// </summary>
        public string RootDirectoryPath { get; set; } = "Z:\\abc";

        /// <summary>
        /// ファイル名検索パターン種別。
        /// </summary>
        public FindPatternKind FileNameFindPatternKind { get; set; }
        /// <summary>
        /// ファイル名検索パターンにて大文字小文字を区別するか。
        /// </summary>
        public bool FileNameIgnoreCase { get; set; }
        /// <summary>
        /// ファイル名検索内容。
        /// </summary>
        public string FileNameFindPattern { get; set; }
        /// <summary>
        /// 検索するディレクトリ階層上限。
        /// <para>0で無制限</para>
        /// </summary>
        public int DirectoryLimitLevel { get; set; }
#if DEBUG
            = 0;
#endif
        /// <summary>
        /// ファイル内まで検索するか。
        /// </summary>
        public bool FindFileContent { get; set; } = true;
        /// <summary>
        /// ファイル内検索方法。
        /// </summary>
        public FindPatternKind FileContentFindPatternKind { get; set; }
        /// <summary>
        /// ファイル内検索パターンにて大文字小文字を区別するか。
        /// </summary>
        public bool FileContentIgnoreCase { get; set; }
        /// <summary>
        /// ファイル内検索内容。
        /// </summary>
        public string FileContentFindPattern { get; set; } = "を"; //"領"

        public FindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; } = new FindMicrosoftOfficeContentSetting();
        public FindXmlHtmlContentSetting XmlHtmlContent { get; } = new FindXmlHtmlContentSetting();

        #endregion
    }
}
