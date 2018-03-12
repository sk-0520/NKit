using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        /// ログ出力を行うか。
        /// </summary>
        bool Logging { get; }

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

    [Serializable, DataContract]
    public class WorkspaceItemSetting : GuidIdSettingBase, IReadOnlyWorkspaceItemSetting
    {
        #region IReadOnlyWorkspaceItemSetting

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DirectoryPath { get; set; }

        [DataMember]
        public bool Logging { get; set; }


        [DataMember]
        public DateTime CreatedTimestamp { get; set; }
        [DataMember]
        public DateTime UpdatedTimestamp { get; set; }

        #endregion
    }
}
