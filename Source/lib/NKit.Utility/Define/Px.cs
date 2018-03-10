using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Define
{
    /// <summary>
    /// ピクセル情報。
    /// </summary>
    [DataContract]
    public enum Px
    {
        /// <summary>
        /// 知らん。
        /// </summary>
        [EnumMember]
        Unknown,
        /// <summary>
        /// 論理座標系。
        /// </summary>
        [EnumMember]
        Logical,
        /// <summary>
        /// デバイス座標系。
        /// </summary>
        [EnumMember]
        Device,
    }
}
