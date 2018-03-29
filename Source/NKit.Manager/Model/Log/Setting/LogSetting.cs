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

        bool IsReceiveTrace { get; }
        bool IsReceiveDebug { get; }
        bool IsReceiveInformation { get; }
        bool IsReceiveWarning { get; }
        bool IsReceiveError { get; }
        bool IsReceiveFatal { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class LogSetting: SettingBase, IReadOnlyLogSetting
    {
        #region IReadOnlyLogSetting

        [DataMember]
        public bool IsReceiveTrace { get; set; } =
#if DEBUG || BETA
            true
#else
            false
#endif
        ;

        [DataMember]
        public bool IsReceiveDebug { get; set; } =
#if DEBUG || BETA
            true
#else
            false
#endif
        ;

        [DataMember]
        public bool IsReceiveInformation { get; set; } = true;
        [DataMember]
        public bool IsReceiveWarning { get; set; } = true;
        [DataMember]
        public bool IsReceiveError { get; set; } = true;
        [DataMember]
        public bool IsReceiveFatal { get; set; } = true;

#endregion
    }
}
