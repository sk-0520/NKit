using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Define
{
    public enum WorkspaceState
    {
        /// <summary>
        /// ワークスペースがない。
        /// </summary>
        None,
        /// <summary>
        /// ワークスペース作成中。
        /// </summary>
        Creating,
        /// <summary>
        /// ワークスペースコピー作成中。
        /// </summary>
        Copy,
        /// <summary>
        /// ワークスペース選択中。
        /// </summary>
        Selecting,
        /// <summary>
        /// ワークスペース実行中。
        /// </summary>
        Running,
        /// <summary>
        /// 更新中。
        /// </summary>
        Updating,
    }
}
