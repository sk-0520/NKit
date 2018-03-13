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

            var letsDieOption = cl.Option("--nkit_lets_die", "Let's die", CommandOptionType.NoValue);
            var serviceUriOption = cl.Option("--nkit_service_uri", "service uri", CommandOptionType.SingleValue);
            var applicationIdOption = cl.Option("--nkit_application_id", "application id", CommandOptionType.SingleValue);
            var workspacePathOption = cl.Option("--nkit_workspace", "workspace path", CommandOptionType.SingleValue);
            var exitEventNameOption = cl.Option("--nkit_exit_event_name", "exit event name", CommandOptionType.SingleValue);

            cl.Execute(args);

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
