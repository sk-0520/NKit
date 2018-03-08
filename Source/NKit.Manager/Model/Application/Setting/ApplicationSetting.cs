using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Application.Setting
{
    public interface IReadOnlyApplicationSetting
    {
        #region property
        #endregion
    }

    [Serializable, DataContract]
    public class ApplicationSetting: SettingBase, IReadOnlyApplicationSetting
    {
        #region IReadOnlyApplicationSetting 
        #endregion
    }
}
