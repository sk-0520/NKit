using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Define
{
    /// <summary>
    /// 実行状態。
    /// </summary>
    [DataContract]
    public enum RunState
    {
        /// <summary>
        /// 未実行。
        /// </summary>
        [EnumMember]
        None,
        /// <summary>
        /// 準備中。
        /// </summary>
        [EnumMember]
        Preparate,
        /// <summary>
        /// 実行中。
        /// </summary>
        [EnumMember]
        Running,
        /// <summary>
        /// 正常完了。
        /// </summary>
        [EnumMember]
        Finished,
        /// <summary>
        /// 異常終了
        /// </summary>
        [EnumMember]
        Error,
        /// <summary>
        /// キャンセル
        /// </summary>
        [EnumMember]
        Cancel,
    }
}
