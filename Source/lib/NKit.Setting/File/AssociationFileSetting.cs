using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.NKit.Setting.File
{
    public interface IReadOnlyAssociationFileSetting
    {
        #region property

        string TextFileApplicationPath { get; }
        string TextFileArgumentFormat { get; }

        string XmlHtmlFileApplicationPath { get; }
        string XmlHtmlFileArgumentFormat { get; }

        #endregion
    }
    public class AssociationFileSetting: SettingBase, IReadOnlyAssociationFileSetting
    {
        #region IReadOnlyAssociationFileSetting

        public string TextFileApplicationPath { get; set; }
        public string TextFileArgumentFormat { get; set; }

        public string XmlHtmlFileApplicationPath { get; set; }
        public string XmlHtmlFileArgumentFormat { get; set; }

        #endregion
    }
}
