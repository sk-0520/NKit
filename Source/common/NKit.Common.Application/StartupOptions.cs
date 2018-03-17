using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace ContentTypeTextNet.NKit.Common
{
    public static class StartupOptions
    {
        #region property
        public static bool LetsDie { get; private set; }

        public static Uri ServiceUri { get; private set; }
        public static string WorkspacePath { get; private set; }
        public static string ApplicationId { get; private set; }
        public static string ExitEventName { get; private set; }

        public static bool IsManaged { get; private set; }

#if DEBUG
        static bool IsInitialized { get; set; } = false;
#endif
        #endregion

        #region function

        public static void Initialize(string[] args)
        {
#if DEBUG
            if(IsInitialized) {
                throw new InvalidOperationException();
            }
#endif
            var cl = new CommandLineApplication(false);

            var letsDieOption = cl.Option(CommonUtility.ManagedStartup.LetsDie, "Let's die", CommandOptionType.NoValue);
            var serviceUriOption = cl.Option(CommonUtility.ManagedStartup.ServiceUri, "service uri", CommandOptionType.SingleValue);
            var applicationIdOption = cl.Option(CommonUtility.ManagedStartup.ApplicationId, "application id", CommandOptionType.SingleValue);
            var workspacePathOption = cl.Option(CommonUtility.ManagedStartup.WorkspacePath, "workspace path", CommandOptionType.SingleValue);
            var exitEventNameOption = cl.Option(CommonUtility.ManagedStartup.GroupSuicideEventName, "exit event name", CommandOptionType.SingleValue);

            var index = Array.IndexOf(args, CommonUtility.ManagedStartup.ExecuteFlag);
            IsManaged = index != -1;

            if(IsManaged && index < args.Length - 1) {
                var nkitArgs = new string[args.Length - index - 1];
                Array.Copy(args, index + 1, nkitArgs, 0, nkitArgs.Length);

                cl.Execute(nkitArgs);
            } else {
                cl.Execute(args);
            }

            LetsDie = letsDieOption.HasValue();

            if(serviceUriOption.HasValue()) {
                ServiceUri = new Uri(serviceUriOption.Value());
            }
            WorkspacePath = workspacePathOption.Value();
            ApplicationId = applicationIdOption.Value();
            ExitEventName = exitEventNameOption.Value();

            if(!string.IsNullOrWhiteSpace(ApplicationId)) {
                TaskbarManager.Instance.ApplicationId = ApplicationId;
            }


#if DEBUG
            IsInitialized = true;
#endif
        }

        #endregion
    }
}
