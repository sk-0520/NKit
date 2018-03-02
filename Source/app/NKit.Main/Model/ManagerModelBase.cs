using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public abstract class ManagerModelBase : ModelBase
    {
        public ManagerModelBase(Setting setting)
        {
            Setting = setting;
        }

        #region property

        protected Setting Setting { get; }

        #endregion
    }
}
