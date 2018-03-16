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
    public interface IReadOnlyCaptureSetting : IReadOnlySetting
    {
        #region property

        IReadOnlyCollection<IReadOnlyCaptureGroupSetting> Groups { get; }

        IReadOnlyKeySetting SelectKey { get; }
        IReadOnlyKeySetting TakeShotKey { get; }

        bool ScrollInternetExplorerIsEnabledHideFixedHeader { get; }
        string ScrollInternetExplorerHideFixedHeaderElements { get; }
        bool ScrollInternetExplorerIsEnabledHideFixedFooter { get; }
        string ScrollInternetExplorerHideFixedFooterElements { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class CaptureSetting : SettingBase, IReadOnlyCaptureSetting
    {
        #region IReadOnlyCaptureSetting

        [DataMember]
        public Collection<CaptureGroupSetting> Groups { get; set; } = new Collection<CaptureGroupSetting>();
        IReadOnlyCollection<IReadOnlyCaptureGroupSetting> IReadOnlyCaptureSetting.Groups => Groups;

        [DataMember]
        public KeySetting SelectKey { get; set; } = new KeySetting();
        IReadOnlyKeySetting IReadOnlyCaptureSetting.SelectKey => SelectKey;

        [DataMember]
        public KeySetting TakeShotKey { get; set; } = new KeySetting();
        IReadOnlyKeySetting IReadOnlyCaptureSetting.TakeShotKey => TakeShotKey;

        [DataMember]
        public bool ScrollInternetExplorerIsEnabledHideFixedHeader { get; set; }
        [DataMember]
        public string ScrollInternetExplorerHideFixedHeaderElements { get; set; }
        [DataMember]
        public bool ScrollInternetExplorerIsEnabledHideFixedFooter { get; set; }
        [DataMember]
        public string ScrollInternetExplorerHideFixedFooterElements { get; set; }

        #endregion
    }
}
