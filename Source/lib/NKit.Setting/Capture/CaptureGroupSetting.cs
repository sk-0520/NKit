using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyCaptureGroupSetting : IReadOnlyGuidSetting
    {
        #region property

        string GroupName { get; }

        CaptureTarget CaptureTarget { get; }

        bool IsEnabledClipboard { get; }
        /// <summary>
        /// 開始時に即時選択状態に移行するか。
        /// </summary>
        bool IsImmediateSelect { get; }

        /// <summary>
        /// このグループでのスクロール設定を標準設定より優先するか。
        /// </summary>
        bool OverwriteScrollSetting { get; }
        IReadOnlyScrollCaptureSetting Scroll { get;}

        #endregion
    }

    [Serializable, DataContract]
    public class CaptureGroupSetting: GuidSettingBase, IReadOnlyCaptureGroupSetting
    {
        #region IReadOnlyCaptureGroupSetting

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public CaptureTarget CaptureTarget { get; set; } = CaptureTarget.Window;

        [DataMember]
        public bool IsEnabledClipboard { get; set; } = true;

        [DataMember]
        public bool IsImmediateSelect { get; set; } = true;

        [DataMember]
        public bool OverwriteScrollSetting { get; set; } = false;
        [DataMember]
        public ScrollCaptureSetting Scroll { get; set; } = new ScrollCaptureSetting();
        IReadOnlyScrollCaptureSetting IReadOnlyCaptureGroupSetting.Scroll => Scroll;

        #endregion
    }
}
