using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting
{
    public interface IReadOnlyWorkspaceSetting
    {
        #region property
        IReadOnlyWorkspaceItemSetting[] Items { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class WorkspaceSetting: SettingBase, IReadOnlyWorkspaceSetting
    {
        #region IReadOnlyWorkspaceSetting
        [DataMember]
        public WorkspaceItemSetting[] Items { get; set; } = new WorkspaceItemSetting[0];

        IReadOnlyWorkspaceItemSetting[] IReadOnlyWorkspaceSetting.Items => Items;

        #endregion
    }
}
