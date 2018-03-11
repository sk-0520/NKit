using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ContentTypeTextNet.NKit.Setting.Finder
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
        /// ファイル検索に用いるファイルサイズの上下限を指定する。
        /// <para>0は無制限。</para>
        /// <para>一応のUI運用としては上限のみの指定でいいと思ってる。下限 0 の 上限 n みたいな。</para>
        /// </summary>
        IReadOnlyRange<long> FileSizeLimit { get; }

        /// <summary>
        /// ファイルの情報を検索対象とするか
        /// </summary>
        bool FindFileProperty { get; }

        /// <summary>
        /// 検索ファイル属性
        /// </summary>
        FileAttributes FilePropertyFileAttributes { get; }
        /// <summary>
        /// <see cref="FilePropertyFileAttributes"/> の選択内容全てを一致対象とするか。
        /// </summary>
        FlagMatchKind FilePropertyFileAttributeFlagMatchKind { get; }

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
        /// <summary>
        /// ファイル内容検索においてファイルサイズ制限以内のものに限定する。
        /// </summary>
        bool IsEnabledFileContentSizeLimit { get; }

        IReadOnlyFindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; }
        IReadOnlyFindXmlHtmlContentSetting XmlHtmlContent { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class FindGroupSetting : GuidIdSettingBase, IReadOnlyFindGroupSetting
    {
        #region IReadOnlyFindGroupSetting

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string RootDirectoryPath { get; set; }
        [DataMember]
        public int DirectoryLimitLevel { get; set; }
        [DataMember]
        public bool FindHiddenDirectory { get; set; }
        [DataMember]
        public bool FindDotDirectory { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchPatternKind FileNameSearchPatternKind { get; set; }
        [DataMember]
        public bool FileNameCase { get; set; }
        [DataMember]
        public string FileNameSearchPattern { get; set; }

        [DataMember]
        public Range<long> FileSizeLimit { get; set; }
        IReadOnlyRange<long> IReadOnlyFindGroupSetting.FileSizeLimit => FileSizeLimit;

        [DataMember]
        public bool FindFileProperty { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public FileAttributes FilePropertyFileAttributes { get; set; }
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public FlagMatchKind FilePropertyFileAttributeFlagMatchKind { get; set; }

        [DataMember]
        public bool FindFileContent { get; set; }
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchPatternKind FileContentSearchPatternKind { get; set; }
        [DataMember]
        public bool FileContentCase { get; set; }
        [DataMember]
        public string FileContentSearchPattern { get; set; }
        [DataMember]
        public bool IsEnabledFileContentSizeLimit { get; set; } = true;

        [DataMember]
        public FindMicrosoftOfficeContentSetting MicrosoftOfficeContent { get; set; } = new FindMicrosoftOfficeContentSetting();
        IReadOnlyFindMicrosoftOfficeContentSetting IReadOnlyFindGroupSetting.MicrosoftOfficeContent => MicrosoftOfficeContent;

        [DataMember]
        public FindXmlHtmlContentSetting XmlHtmlContent { get; set; } = new FindXmlHtmlContentSetting();
        IReadOnlyFindXmlHtmlContentSetting IReadOnlyFindGroupSetting.XmlHtmlContent => XmlHtmlContent;

        #endregion
    }
}
