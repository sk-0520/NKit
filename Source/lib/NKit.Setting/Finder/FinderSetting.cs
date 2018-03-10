using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Finder
{
    public interface IReadOnlyFinderSetting
    {
        IReadOnlyFindGroupSetting[] Groups { get; }

        string TextNamePattern { get; }
        string MicrosoftOfficeNamePattern { get; }
        string XmlHtmlNamePattern { get; }
    }

    public class FinderSetting : SettingBase, IReadOnlyFinderSetting
    {
        #region IReadOnlyFinderSetting

        public FindGroupSetting[] Groups { get; set; } = new FindGroupSetting[0];
        IReadOnlyFindGroupSetting[] IReadOnlyFinderSetting.Groups => Groups;

        public string TextNamePattern { get; set; } = "*.txt|*.csv";
        public string MicrosoftOfficeNamePattern { get; set; } = "*.xls|*.xlsx|*.xlsm|*.docx";
        public string XmlHtmlNamePattern { get; set; } = "*.xml|*.html|*.htm|*.xaml";

        #endregion
    }
}
