using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.NKit.Setting.Finder
{
    public interface IReadOnlyFindGroupSetting: IReadOnlyGuidIdSetting
    {
        #region property

        /// <summary>
        /// 検索名。
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// 検索対象ディレクトリパス。
        /// </summary>
        string RootDirectoryPath { get; }
        /// <summary>
        /// 検索するディレクトリ階層上限。
        /// <para>0で無制限</para>
        /// </summary>
        int DirectoryLimitLevel { get; }
        /// <summary>
        /// 隠しディレクトリを検索するか。
        /// </summary>
        bool FindHiddenDirectory { get; }
        /// <summary>
        /// ディレクトリ名の先頭が . で始まるディレクトリを検索するか。
        /// </summary>
        bool FindDotDirectory { get; }

        /// <summary>
        /// ファイル名検索パターン種別。
        /// </summary>
        SearchPatternKind FileNameSearchPatternKind { get; }
        /// <summary>
        /// ファイル名検索パターンにて大文字小文字を区別するか。
        /// </summary>
        bool FileNameCase { get; }
        /// <summary>
        /// ファイル名検索パターン。
        /// </summary>
        string FileNameSearchPattern { get; }
        /// <summary>
        /// ファイル内まで検索するか。
        /// </summary>
        bool FindFileContent { get;  }
        /// <summary>
        /// ファイル内検索方法。
        /// </summary>
        SearchPatternKind FileContentSearchPatternKind { get;  }
        /// <summary>
        /// ファイル内検索パターンにて大文字小文字を区別するか。
        /// </summary>
        bool FileContentCase { get;  }
        /// <summary>
        /// ファイル内検索内容。
        /// </summary>
        string FileContentSearchPattern { get;  }

        IReadOnlyFindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; }
        IReadOnlyFindXmlHtmlContentSetting XmlHtmlContent { get; } 

        #endregion
    }

    public class FindGroupSetting : GuidIdSettingBase, IReadOnlyFindGroupSetting
    {
        #region IReadOnlyFindGroupSetting

        public string GroupName { get; set; }

        public string RootDirectoryPath { get; set; }
        public int DirectoryLimitLevel { get; set; }
        public bool FindHiddenDirectory { get; set; }
        public bool FindDotDirectory { get; set; }

        public SearchPatternKind FileNameSearchPatternKind { get; set; }
        public bool FileNameCase { get; set; }
        public string FileNameSearchPattern { get; set; }
        public bool FindFileContent { get; set; } = true;
        public SearchPatternKind FileContentSearchPatternKind { get; set; }
        public bool FileContentCase { get; set; }
        public string FileContentSearchPattern { get; set; }

        public FindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; } = new FindMicrosoftOfficeContentSetting();
        IReadOnlyFindMicrosoftOfficeContentSetting IReadOnlyFindGroupSetting.MicrosoftOfficeContent => MicrosoftOfficeContent;

        public FindXmlHtmlContentSetting XmlHtmlContent { get; } = new FindXmlHtmlContentSetting();
        IReadOnlyFindXmlHtmlContentSetting IReadOnlyFindGroupSetting.XmlHtmlContent => XmlHtmlContent;

        #endregion
    }
}
