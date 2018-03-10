using System;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Common
{
    public abstract class CliApplicationModelBase : RunnableSyncModel<int>
    {
        public CliApplicationModelBase(string[] arguments)
        {
            Arguments = arguments;
        }

        #region property

        public string[] Arguments { get; }

        protected CommandLineApplication CommandLineApplication { get; } = new CommandLineApplication(false);

        #endregion

        #region function

        protected bool BuildCommandLine()
        {
            CommandLineApplication.Execute(Arguments);

            return true;
        }

        #endregion
    }
}
