using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Main.Model.Wapper
{
    public class BusyBoxExecutor : ActionCliApplicationExecutor
    {
        public BusyBoxExecutor(bool usePlatformBusyBox, string command, string arguments)
            : base(CommonUtility.GetBusyBox(usePlatformBusyBox, CommonUtility.GetBinaryDirectoryForApplication()).FullName, command + " " + arguments)
        {
            Command = command;
        }

        #region property

        public string Command { get; }

        #endregion

        #region function
        #endregion

        #region ActionCliApplicationExecutor
        #endregion
    }
}
