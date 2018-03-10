using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Setting.Define
{
    [DataContract]
    public enum FlagMatchKind
    {
        /// <summary>
        /// どれかのフラグが立っている。
        /// </summary>
        [EnumMember]
        Has,
        /// <summary>
        /// 少なくとも指定フラグは立っている。
        /// </summary>
        [EnumMember]
        Approximate,
        /// <summary>
        /// すべてのフラグが立っている。
        /// </summary>
        [EnumMember]
        Full,
    }
}
