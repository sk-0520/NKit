using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.NKit.Setting.File;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.NKit.Setting.NKit;

namespace ContentTypeTextNet.NKit.NKit.Setting
{
    public class MainSetting: SettingBase
    {
        #region property

        public WindowSetting MainWindow { get; set; } = new WindowSetting();

        public FinderSetting Finder { get; set; } = new FinderSetting();

        public FileSetting File { get; set; } = new FileSetting();

        public NKitSetting NKit { get; set; } = new NKitSetting();

        #endregion
    }
}
