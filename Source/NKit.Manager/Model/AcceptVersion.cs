using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class AcceptVersionItem: SettingBase
    {
        #region property

        /// <summary>
        /// この変更に対するバージョン。
        /// </summary>
        [XmlAttribute("version")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string _Version { get; set; }
        [XmlIgnore]
        public Version Version
        {
            get { return string.IsNullOrWhiteSpace(_Version) ? new Version() : Version.Parse(_Version); }
            set { _Version = value.ToString(4); }
        }

        /// <summary>
        /// 変更内容。
        /// </summary>
        [XmlElement("value")]
        public string[] Values { get; set; }

        #endregion
    }

    [Serializable, XmlRoot("version")]
    public class AcceptVersion: SettingBase
    {
        #region property

        /// <summary>
        /// このバージョン**以下**であれば使用許諾が必要。
        /// </summary>
        [XmlAttribute("min")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string _MinimumVersion { get; set; }
        [XmlIgnore]
        public Version MinimumVersion
        {
            get { return string.IsNullOrWhiteSpace(_MinimumVersion) ? new Version(): Version.Parse(_MinimumVersion); }
            set { _MinimumVersion = value.ToString(4); }
        }

        /// <summary>
        /// 各バージョンの許諾に書か変わる変更内容。
        /// </summary>
        [XmlElement("item")]
        public AcceptVersionItem[] Items { get; set; }

        #endregion
    }
}
