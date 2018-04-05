using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Finder
{
    public interface IReadOnlyFinderSetting : IReadOnlySetting
    {
        /// <summary>
        /// 保存済み検索条件。
        /// </summary>
        IReadOnlyCollection<IReadOnlyFindGroupSetting> Groups { get; }
        /// <summary>
        /// 検索履歴。
        /// </summary>
        IReadOnlyCollection<IReadOnlyFindGroupSetting> Histories { get; }
        /// <summary>
        /// デフォルト検索条件。
        /// </summary>
        IReadOnlyFindGroupSetting DefaultGroupSetting { get; }

        /// <summary>
        /// テキストファイルとしてのファイル名パターン。
        /// </summary>
        string TextFileNamePattern { get; }
        /// <summary>
        /// MS Officeファイルとしてのファイル名パターン。
        /// </summary>
        string MicrosoftOfficeFileNamePattern { get; }
        string PdfFileNamePattern { get; }
        /// <summary>
        /// Xml/HTMLファイルとしてのファイル名パターン。
        /// </summary>
        string XmlHtmlFileNamePattern { get; }
    }

    [Serializable, DataContract]
    public class FinderSetting : SettingBase, IReadOnlyFinderSetting
    {
        #region IReadOnlyFinderSetting

        [DataMember]
        public Collection<FindGroupSetting> Groups { get; set; } = new Collection<FindGroupSetting>();
        IReadOnlyCollection<IReadOnlyFindGroupSetting> IReadOnlyFinderSetting.Groups => Groups;

        [DataMember]
        public Collection<FindGroupSetting> Histories { get; set; } = new Collection<FindGroupSetting>();
        IReadOnlyCollection<IReadOnlyFindGroupSetting> IReadOnlyFinderSetting.Histories => Histories;

        [DataMember]
        public FindGroupSetting DefaultGroupSetting { get; set; } = new FindGroupSetting();
        IReadOnlyFindGroupSetting IReadOnlyFinderSetting.DefaultGroupSetting => DefaultGroupSetting;

        [DataMember]
        public string TextFileNamePattern { get; set; } = CreateExtensions(
            "txt", "log",
            "bat", "cmd", "bin",
            "ini", "conf", "inf",
            "csv", "tsv",
            "css", // お前結局 XSLT 以外に仲間できたんか、スタイルシートがカスケード一択ってどうなん？
            "xml", "setting", "xsl",
            "html", "htm", "asp", "cshtml",
            "md", "yaml",
            "js", "json", "ts", "vbs",
            "c", "cpp", "h",
            "d", "ddoc",
            "sln", "",
            "cs", "csproj", "xaml", "config", "csproj.user", "manifest", "resx", "xsd", "tt", // VB? しらんなぁ
            "sh",
            "pl", "pm", "cgi",
            "php",
            "py",
            "sql",
            // Java は最後な！
            "java", "jsp", "properties"
        );
        [DataMember]
        public string MicrosoftOfficeFileNamePattern { get; set; } = CreateExtensions(
            "xls",
            "xlsx",
            "xlsm",
            "docx"
        );

        public string PdfFileNamePattern { get; set; } = CreateExtensions(
            "pdf"
        );

        [DataMember]
        public string XmlHtmlFileNamePattern { get; set; } = CreateExtensions(
            "xml", "setting", "xaml", "config",
            "manifest", "resx",
            "html", "htm",
            "csproj", "csproj.user",
            "svg"
        );

        #endregion

        #region function

        static string CreateExtensions(params string[] args) => string.Join("|", args.Select(s => "*." + s));

        #endregion
    }
}
