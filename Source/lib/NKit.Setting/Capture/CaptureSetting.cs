using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        IReadOnlyCollection<IReadOnlyCaptureGroupSetting> Groups { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class CaptureSetting: SettingBase, IReadOnlyCaptureSetting
    {
        #region IReadOnlyCaptureSetting

        [DataMember]
        public Collection<CaptureGroupSetting> Groups { get; set; } = new Collection<CaptureGroupSetting>();
        IReadOnlyCollection<IReadOnlyCaptureGroupSetting> IReadOnlyCaptureSetting.Groups => Groups;


        #endregion
    }
}
