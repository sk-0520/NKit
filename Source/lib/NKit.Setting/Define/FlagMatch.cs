using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.NKit.Setting.Define
{
    public enum FlagMatch
    {
        /// <summary>
        /// どれかのフラグが立っている。
        /// </summary>
        Has,
        /// <summary>
        /// 少なくとも指定フラグは立っている。
        /// </summary>
        Approximate,
        /// <summary>
        /// すべてのフラグが立っている。
        /// </summary>
        Full,
    }
}
