using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Log;
using ContentTypeTextNet.NKit.Manager.Model.Workspace;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ApplicationManager : ManagerBase
    {
        #region event

        public event EventHandler<EventArgs> MainApplicationExited;
        public event EventHandler<EventArgs> ApplicationExited;

        #endregion

        public ApplicationManager(IApplicationLogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("AP");
        }

        #region property

        NKitApplicationItem MainApplication { get; set; }

        IApplicationLogFactory LogFactory { get; }
        ILogger Logger { get; }

        object _itemsLocker = new object();
        IList<ApplicationItem> Items { get; } = new List<ApplicationItem>();

        #endregion

        #region function

        string AddNKitArguments(IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspaceItemSetting, string sourceArguments)
        {
            var list = new[] {
                sourceArguments != null && sourceArguments.IndexOf(CommonUtility.ManagedStartup.ExecuteFlag) != -1
                    ? string.Empty
                    : CommonUtility.ManagedStartup.ExecuteFlag
                ,

                CommonUtility.ManagedStartup.ServiceUri,
                ProgramRelationUtility.EscapesequenceToArgument(activeWorkspace.ServiceUri.ToString()),

                CommonUtility.ManagedStartup.WorkspacePath,
                ProgramRelationUtility.EscapesequenceToArgument(workspaceItemSetting.DirectoryPath),

                CommonUtility.ManagedStartup.ApplicationId,
                ProgramRelationUtility.EscapesequenceToArgument(activeWorkspace.ApplicationId),

                CommonUtility.ManagedStartup.ExitEventName,
                ProgramRelationUtility.EscapesequenceToArgument(activeWorkspace.ExitEventName),
            };
            var headArgs = string.Join(" ", list);

            return  sourceArguments + " " + headArgs;
        }

        public void ExecuteMainApplication(IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspaceItemSetting)
        {
            if(MainApplication != null) {
                MainApplication.Exited -= MainApplication_Exited;
            }

            MainApplication = new NKitApplicationItem(NKitApplicationKind.Main, LogFactory) {
                Arguments = AddNKitArguments(activeWorkspace, workspaceItemSetting, string.Empty)
            };

            MainApplication.Exited += MainApplication_Exited;

            MainApplication.Execute();
        }

        public void ExecuteNKitApplication(NKitApplicationKind senderApplication, NKitApplicationKind targetApplication, IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            ApplicationItem item = null;

            switch(targetApplication) {
                case NKitApplicationKind.Main: // 通常処理として起動する可能性あり(想定する用途としては初回一括起動)
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(activeWorkspace, workspace, arguments),
                    };
                    break;

                case NKitApplicationKind.Rocket:
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(activeWorkspace, workspace, arguments),
                    };
                    break;

                case NKitApplicationKind.Cameraman:
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(activeWorkspace, workspace, arguments),
                    };
                    break;

                default:
                    throw new NotImplementedException();
            }

            item.Exited += Item_Exited;
            lock(this._itemsLocker) {
                Items.Add(item);
            }
            Logger.Debug(item.Path);
            Logger.Debug(item.Arguments);
            item.Execute();
        }

        public void ExecuteOtherApplication(NKitApplicationKind senderApplication, string programPath, IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            var item = new ApplicationItem(programPath) {
                Arguments = arguments,
                WorkingDirectoryPath = workingDirectoryPath,
                CreateWindow = false,
                IsOutputReceive = true,
            };
            item.Exited += Item_Exited;
            lock(this._itemsLocker) {
                Items.Add(item);
            }
            item.Execute();
        }

        public void ShutdownOthersApplications()
        {

        }

        #endregion

        private void MainApplication_Exited(object sender, EventArgs e)
        {
            ShutdownOthersApplications();

            if(MainApplicationExited != null) {
                MainApplicationExited(sender, e);
            }
        }

        private void Item_Exited(object sender, EventArgs e)
        {
            var item = (ApplicationItem)sender;
            item.Exited -= Item_Exited;

            lock(this._itemsLocker) {
                Items.Remove(item);
            }

            if(ApplicationExited != null) {
                ApplicationExited(this, e);
            }
        }
    }
}
