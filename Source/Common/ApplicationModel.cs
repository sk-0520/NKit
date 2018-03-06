using System;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Common
{
    public abstract class ApplicationModelBase : RunnableModelBase<int>
    {
        public ApplicationModelBase(string[] arguments)
        {
            Arguments = arguments;

            OptionHelp = CommandLineApplication.Option("--help", "help", CommandOptionType.NoValue);
        }

        #region property

        public string[] Arguments { get; }

        protected CommandOption OptionHelp { get; }

        protected CommandLineApplication CommandLineApplication { get; } = new CommandLineApplication(true);

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
