using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.NKit.Setting.NKit
{
    public interface IReadOnlyNKitSetting
    {
        #region property

        bool UsePlatformBusyBox { get; }

        #endregion
    }

    public class NKitSetting : SettingBase, IReadOnlyNKitSetting
    {
        #region IReadOnlyApplicationSetting

        public bool UsePlatformBusyBox { get; set; } = false;

        #endregion
    }
}
