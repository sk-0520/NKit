using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.NKit.Setting.File
{
    public interface IReadOnlyFileSetting
    {
        #region property

        IReadOnlyAssociationFileSetting AssociationFile { get; }

        #endregion
    }

    public class FileSetting: SettingBase, IReadOnlyFileSetting
    {
        #region property

        public AssociationFileSetting AssociationFile { get; set; } = new AssociationFileSetting();
        IReadOnlyAssociationFileSetting IReadOnlyFileSetting.AssociationFile => AssociationFile;

        #endregion
    }
}
