using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting;

namespace ContentTypeTextNet.NKit.Main.Model.App
{
    public interface IReadOnlyAppSetting
    {
        #region property

        bool UsePlatformBusyBox { get; }

        #endregion
    }

    public class AppSetting : SettingBase, IReadOnlyAppSetting
    {
        #region IReadOnlyApplicationSetting

        public bool UsePlatformBusyBox { get; set; } = false;

        #endregion
    }
}
