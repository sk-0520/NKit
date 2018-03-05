using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.NKit.Setting;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public sealed class Setting : SettingBase
    {
        #region property

        public WindowSetting MainWindow { get; set; } = new WindowSetting();

        public FinderSetting Finder { get; set; } = new FinderSetting();

        public NKitSetting NKit { get; set; } = new NKitSetting();

        #endregion

    }
}
