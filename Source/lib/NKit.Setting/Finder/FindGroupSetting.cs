using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.NKit.Setting.Finder
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
        public string RootDirectoryPath { get; set; }

        /// <summary>
        /// ファイル名検索パターン種別。
        /// </summary>
        public SearchPatternKind FileNameSearchPatternKind { get; set; }
        /// <summary>
        /// ファイル名検索パターンにて大文字小文字を区別するか。
        /// </summary>
        public bool FileNameIgnoreCase { get; set; }
        /// <summary>
        /// ファイル名検索内容。
        /// </summary>
        public string FileNameSearchPattern { get; set; }
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
        public SearchPatternKind FileContentSearchPatternKind { get; set; }
        /// <summary>
        /// ファイル内検索パターンにて大文字小文字を区別するか。
        /// </summary>
        public bool FileContentIgnoreCase { get; set; }
        /// <summary>
        /// ファイル内検索内容。
        /// </summary>
        public string FileContentSearchPattern { get; set; }

        public FindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; } = new FindMicrosoftOfficeContentSetting();
        public FindXmlHtmlContentSetting XmlHtmlContent { get; } = new FindXmlHtmlContentSetting();

        #endregion
    }
}
