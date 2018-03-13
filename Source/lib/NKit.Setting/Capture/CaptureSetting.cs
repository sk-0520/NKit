using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyCaptureSetting: IReadOnlySetting
    {
        #region property



        #endregion
    }

    [Serializable, DataContract]
    public class CaptureSetting: SettingBase, IReadOnlyCaptureSetting
    {
        #region IReadOnlyCaptureSetting



        #endregion
    }
}
