using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model.Workspace
{
    public interface IReadOnlyWorkspaceVolatilityItem
    {
        #region property

        /// <summary>
        /// あぷりけーしょんあいでぃー。
        /// </summary>
        string ApplicationId { get; }

        /// <summary>
        /// サービスURI。
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// 終了イベント名。
        /// </summary>
        string ExitEventName { get; }


        #endregion
    }

    public class WorkspaceVolatilityItem : IReadOnlyWorkspaceVolatilityItem
    {
        #region IReadOnlyWorkspaceVolatilityItem

        public string ApplicationId { get; set; }
        /// <summary>
        /// サービスURI。
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// 終了イベント名。
        /// </summary>
        public string ExitEventName { get; set; }

        #endregion
    }
}
