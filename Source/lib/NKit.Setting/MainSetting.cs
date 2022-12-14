using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting.Capture;
using ContentTypeTextNet.NKit.Setting.File;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Setting.NKit;

namespace ContentTypeTextNet.NKit.Setting
{
    [Serializable, DataContract]
    public class MainSetting: SettingBase
    {
        #region property

        [DataMember]
        public WindowSetting MainWindow { get; set; } = new WindowSetting();

        [DataMember]
        public FinderSetting Finder { get; set; } = new FinderSetting();

        [DataMember]
        public FileSetting File { get; set; } = new FileSetting();

        [DataMember]
        public CaptureSetting Capture { get; set; } = new CaptureSetting();

        [DataMember]
        public NKitSetting NKit { get; set; } = new NKitSetting();

        #endregion
    }
}
