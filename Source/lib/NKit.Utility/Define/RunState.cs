using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Define
{
    /// <summary>
    /// 実行状態。
    /// </summary>
    public enum RunState
    {
        /// <summary>
        /// 未実行。
        /// </summary>
        None,
        /// <summary>
        /// 準備中。
        /// </summary>
        Prepare,
        /// <summary>
        /// 実行中。
        /// </summary>
        Running,
        /// <summary>
        /// 正常完了。
        /// </summary>
        Finished,
        /// <summary>
        /// 異常終了
        /// </summary>
        Error,
    }
}
