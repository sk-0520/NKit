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
    public interface IReadOnlyFinderSetting: IReadOnlySetting
    {
        /// <summary>
        /// 保存済み検索条件。
        /// </summary>
        IReadOnlyCollection<IReadOnlyFindGroupSetting> Groups { get; }
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
        public FindGroupSetting DefaultGroupSetting { get; set; } = new FindGroupSetting();
        IReadOnlyFindGroupSetting IReadOnlyFinderSetting.DefaultGroupSetting => DefaultGroupSetting;

        [DataMember]
        public string TextFileNamePattern { get; set; } = "*.txt|*.csv";
        [DataMember]
        public string MicrosoftOfficeFileNamePattern { get; set; } = "*.xls|*.xlsx|*.xlsm|*.docx";
        [DataMember]
        public string XmlHtmlFileNamePattern { get; set; } = "*.xml|*.html|*.htm|*.xaml";

        #endregion
    }
}
