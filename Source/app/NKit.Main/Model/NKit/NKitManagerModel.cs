using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting;
using ContentTypeTextNet.NKit.Setting.Finder;

namespace ContentTypeTextNet.NKit.Main.Model.NKit
{
    public class NKitManagerModel : ManagerModelBase
    {
        public NKitManagerModel(MainSetting setting)
            : base(setting)
        { }

        #region property

        public FinderSetting Finder => Setting.Finder;

        #endregion
    }
}
