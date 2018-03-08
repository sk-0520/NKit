using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Log.Setting
{
    public interface IReadOnlyLogSetting
    {
        #region property
        #endregion
    }

    [Serializable, DataContract]
    public class LogSetting: SettingBase, IReadOnlyLogSetting
    {
        #region IReadOnlyLogSetting
        #endregion
    }
}
