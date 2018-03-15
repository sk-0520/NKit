using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Application.Setting;
using ContentTypeTextNet.NKit.Manager.Model.Log.Setting;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public interface IReadOnlyManagerSetting
    {
        #region property

        IReadOnlyWorkspaceSetting Workspace { get; }
        IReadOnlyLogSetting Log { get; }
        IReadOnlyApplicationSetting Application { get; }

        /// <summary>
        /// 使用許諾がなされているか。
        /// </summary>
        bool Accepted { get; set; }
        /// <summary>
        /// 初回使用バージョン。
        /// </summary>
        Version FirstExecuteVersion { get; }
        /// <summary>
        /// 初回使用日時。
        /// </summary>
        DateTime FirstExecuteTimestamp { get; }
        /// <summary>
        /// 最終使用バージョン。
        /// </summary>
        Version LastExecuteVersion { get; }
        /// <summary>
        /// 最終使用日時。
        /// </summary>
        DateTime LastExecuteTimestamp { get; }
        /// <summary>
        /// 実行回数。
        /// </summary>
        int ExecuteCount { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class ManagerSetting: SettingBase, IReadOnlyManagerSetting
    {
        #region IReadOnlyManagerSetting

        [DataMember]
        public WorkspaceSetting Workspace { get; private set; } = new WorkspaceSetting();
        IReadOnlyWorkspaceSetting IReadOnlyManagerSetting.Workspace => Workspace;

        [DataMember]
        public LogSetting Log { get; private set; } = new LogSetting();
        IReadOnlyLogSetting IReadOnlyManagerSetting.Log => Log;

        [DataMember]
        public ApplicationSetting Application { get; private set; } = new ApplicationSetting();
        IReadOnlyApplicationSetting IReadOnlyManagerSetting.Application => Application;

        [DataMember]
        public bool Accepted { get; set; }
        [DataMember]
        public Version FirstExecuteVersion { get; set; }
        public DateTime FirstExecuteTimestamp { get; set; }
        [DataMember]
        public Version LastExecuteVersion { get; set; }
        public DateTime LastExecuteTimestamp { get; set; }
        [DataMember]
        public int ExecuteCount { get; set; }



        #endregion
    }
}
