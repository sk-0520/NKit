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
        public WorkspaceManager(ILogCreator logCreator)
        {
            LogCreator = logCreator;
            Logger = LogCreator.CreateLogger("WS");
        }

        #region property

        ILogCreator LogCreator { get; }
        ILogger Logger { get; }

        #endregion

        #region function
        #endregion
    }
}
