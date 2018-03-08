using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting
{
    public interface IReadOnlyWorkspaceItemSetting: IReadOnlyGuidIdSetting
    {
        #region property

        /// <summary>
        /// ワークスペース名。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ワークスペースのディレクトリパス。
        /// </summary>
        string DirectoryPath { get; }

        /// <summary>
        /// 作成日時。
        /// </summary>
        DateTime CreatedTimestamp { get; }
        /// <summary>
        /// 最終使用日時。
        /// <para>使用開始日時となる。</para>
        /// </summary>
        DateTime UpdatedTimestamp { get; }

        #endregion
    }

    public class WorkspaceItemSetting : GuidIdSettingBase, IReadOnlyGuidIdSetting
    {
        #region IReadOnlyWorkspaceItemSetting

        public string Name { get; set; }

        public string DirectoryPath { get; set; }

        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }

        #endregion
    }
}
