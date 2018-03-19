using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    [Serializable, XmlRoot("third-party-license")]
    public class ThirdPartyLicense
    {
        #region property

        [XmlElement("component")]
        public ThirdPartyComponent[] Components { get; set; } = new ThirdPartyComponent[0];

        #endregion
    }

    [Serializable]
    public class ThirdPartyComponent
    {
        #region property

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("project-uri")]
        public string ProjectUri { get; set; }

        [XmlAttribute("license-name")]
        public string LicenseName { get; set; }

        [XmlAttribute("license-uri")]
        public string LicenseUri { get; set; }

        public string Document { get; set; }

        #endregion
    }
}
