using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyCaptureGroupSetting: IReadOnlyGuidSetting
    {
        #region property

        string GroupName { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class CaptureGroupSetting: SettingBase, IReadOnlyCaptureGroupSetting
    {
        #region IReadOnlyCaptureGroupSetting

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        #endregion
    }
}
