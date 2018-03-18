using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyCaptureGroupSetting : IReadOnlyGuidSetting
    {
        #region property

        string GroupName { get; }

        /// <summary>
        /// 開始時に即時選択状態に移行するか。
        /// </summary>
        bool ImmediateSelect { get; }

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
        public bool ImmediateSelect { get; set; }

        [DataMember]
        public bool OverwriteScrollSetting { get; set; }
        [DataMember]
        public ScrollCaptureSetting Scroll { get; set; } = new ScrollCaptureSetting();
        IReadOnlyScrollCaptureSetting IReadOnlyCaptureGroupSetting.Scroll => Scroll;

        #endregion
    }
}
