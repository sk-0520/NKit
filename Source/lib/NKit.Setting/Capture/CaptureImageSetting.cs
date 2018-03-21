using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting.Capture
{
    public interface IReadOnlyCaptureImageSetting
    {
        #region proeprty

        uint Width { get; }
        uint Height { get; }

        string DirectoryName { get; }
        string FileName { get; }

        string Comment { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class CaptureImageSetting: SettingBase, IReadOnlyCaptureImageSetting
    {
        #region IReadOnlyCaptureImageSetting

        [DataMember]
        public string DirectoryName { get; set; }
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public uint Width { get; set; }
        [DataMember]
        public uint Height { get; set; }

        [DataMember]
        public string Comment { get; set; }

        #endregion
    }
}
