using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyInternetExplorerHideScrollCaptureSetting : IReadOnlySetting
    {
        #region property

        bool IsEnabled { get; }
        string HideElements { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class InternetExplorerScrollHideCaptureSetting : SettingBase, IReadOnlyInternetExplorerHideScrollCaptureSetting
    {
        #region IReadOnlyCaptureScrollSetting

        [DataMember]
        public bool IsEnabled { get; set; } = false;
        [DataMember]
        public string HideElements { get; set; } = "*";

        #endregion
    }

    public interface IReadOnlyInternetExplorerScrollCaptureSetting: IReadOnlySetting
    {
        #region property

        TimeSpan InitializeTime { get; }

        IReadOnlyInternetExplorerHideScrollCaptureSetting Header { get; }
        IReadOnlyInternetExplorerHideScrollCaptureSetting Footer { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class InternetExplorerScrollCaptureSetting: SettingBase, IReadOnlyInternetExplorerScrollCaptureSetting
    {
        #region IReadOnlyInternetExplorerScrollCaptureSetting

        [DataMember]
        public TimeSpan InitializeTime { get; set; } = TimeSpan.FromMilliseconds(750);

        [DataMember]
        public InternetExplorerScrollHideCaptureSetting Header { get; set; } = new InternetExplorerScrollHideCaptureSetting();
        IReadOnlyInternetExplorerHideScrollCaptureSetting IReadOnlyInternetExplorerScrollCaptureSetting.Header => Header;

        [DataMember]
        public InternetExplorerScrollHideCaptureSetting Footer { get; set; } = new InternetExplorerScrollHideCaptureSetting();
        IReadOnlyInternetExplorerHideScrollCaptureSetting IReadOnlyInternetExplorerScrollCaptureSetting.Footer => Footer;

        #endregion
    }

    public interface IReadOnlyScrollCaptureSetting : IReadOnlySetting
    {
        #region property

        TimeSpan ScrollDelayTime { get; }

        IReadOnlyInternetExplorerScrollCaptureSetting InternetExplorer { get; }

        #endregion
    }

    public class ScrollCaptureSetting: SettingBase, IReadOnlyScrollCaptureSetting
    {
        #region IReadOnlyScrollCaptureSetting

        [DataMember]
        public TimeSpan ScrollDelayTime { get; set; } = TimeSpan.FromSeconds(1);

        [DataMember]
        public InternetExplorerScrollCaptureSetting InternetExplorer { get; set; } = new InternetExplorerScrollCaptureSetting();
        IReadOnlyInternetExplorerScrollCaptureSetting IReadOnlyScrollCaptureSetting.InternetExplorer => InternetExplorer;

        #endregion
    }


}
