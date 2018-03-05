using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.NKit.Setting;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public abstract class ManagerModelBase : ModelBase
    {
        public ManagerModelBase(MainSetting setting)
        {
            Setting = setting;
        }

        #region property

        protected MainSetting Setting { get; }

        #endregion
    }
}
