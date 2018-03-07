using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.NKit.Setting.Finder
{
    public interface IReadOnlyFindGroupSetting : IReadOnlyGuidIdSetting
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
        /// ファイルの情報を検索対象とするか
        /// </summary>
        bool FindFileProperty { get; }

        /// <summary>
        /// ファイル検索に用いるファイルサイズの上下限を指定する。
        /// <para>0は無制限。</para>
        /// <para>一応のUI運用としては上限のみの指定でいいと思ってる。下限 0 の 上限 n みたいな。</para>
        /// </summary>
        IReadOnlyRange<long> FilePropertySizeLimit { get; }
        /// <summary>
        /// 検索ファイル属性
        /// </summary>
        FileAttributes FilePropertyFileAttributes { get; }
        /// <summary>
        /// <see cref="FilePropertyFileAttributes"/> の選択内容全てを一致対象とするか。
        /// </summary>
        bool FilePropertyFileAttributeAllEnable { get; }

        /// <summary>
        /// ファイル内まで検索するか。
        /// </summary>
        bool FindFileContent { get; }
        /// <summary>
        /// ファイル内検索方法。
        /// </summary>
        SearchPatternKind FileContentSearchPatternKind { get; }
        /// <summary>
        /// ファイル内検索パターンにて大文字小文字を区別するか。
        /// </summary>
        bool FileContentCase { get; }
        /// <summary>
        /// ファイル内検索内容。
        /// </summary>
        string FileContentSearchPattern { get; }


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

        public bool FindFileProperty { get; set; } = true;

        public Range<long> FilePropertySizeLimit { get; set; }
        IReadOnlyRange<long> IReadOnlyFindGroupSetting.FilePropertySizeLimit => FilePropertySizeLimit;
        public FileAttributes FilePropertyFileAttributes { get; set; }
        public bool FilePropertyFileAttributeAllEnable { get; set; }

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
