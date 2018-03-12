using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Log;

namespace ContentTypeTextNet.NKit.Manager.Model.Update
{
    public class UpdateManager : ManagerBase
    {
        protected UpdateManager(ILogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("UP");
        }

        #region property

        ILogFactory LogFactory { get; }
        ILogger Logger { get; }

        #endregion

        public Task<bool> CheckUpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
