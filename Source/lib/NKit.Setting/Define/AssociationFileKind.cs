using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Setting.Define
{
    /// <summary>
    /// 関連付けファイル種別。
    /// </summary>
    [DataContract]
    public enum AssociationFileKind
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Text,
        [EnumMember]
        MicrosoftOfficeExcel,
        [EnumMember]
        MicrosoftOfficeWord,
        [EnumMember]
        Pdf,
        [EnumMember]
        XmlHtml,
    }
}
