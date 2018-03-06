using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.NKit.Setting.Finder
{
    public interface IReadOnlyFinderSetting
    {
        FindGroupSetting[] Groups { get; }

        string TextNamePattern { get; }
        string MicrosoftOfficeNamePattern { get; }
        string XmlHtmlNamePattern { get; }
    }

    public class FinderSetting : SettingBase, IReadOnlyFinderSetting
    {
        #region IReadOnlyFinderSetting

        public FindGroupSetting[] Groups { get; set; } = new FindGroupSetting[0];

        public string TextNamePattern { get; set; } = "*.txt|*.csv";
        public string MicrosoftOfficeNamePattern { get; set; } = "*.xls|*.xlsx|*.xlsm|*.docx";
        public string XmlHtmlNamePattern { get; set; } = "*.xml|*.html|*.htm|*.xaml";

        #endregion
    }
}
