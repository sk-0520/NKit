using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Workspace
{
    public class WorkspaceManager: ManagerBase
    {
        public WorkspaceManager(ILogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("WS");
        }

        #region property

        ILogFactory LogFactory { get; }
        ILogger Logger { get; }

        #endregion

        #region function
        #endregion
    }
}
