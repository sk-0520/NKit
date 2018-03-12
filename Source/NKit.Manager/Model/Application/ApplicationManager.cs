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
            //TODO: 重複は未考慮
            //      エスケープシーケンスも知らない問題にしておく
            var list = new[] {
                $"--nkit_service_uri",
                ProgramRelationUtility.EscapesequenceToArgument(activeWorkspace.ServiceUri.ToString()),

                $"--nkit_workspace",
                ProgramRelationUtility.EscapesequenceToArgument(workspaceItemSetting.DirectoryPath),

                $"--nkit_application_id",
                ProgramRelationUtility.EscapesequenceToArgument(activeWorkspace.ApplicationId),

                $"--nkit_exit_event_name",
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
                case NKitApplicationKind.Main:
                    throw new ArgumentException();

                case NKitApplicationKind.Rocket:
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
            var item = (NKitApplicationItem)sender;
            item.Exited -= Item_Exited;

            lock(this._itemsLocker) {
                Items.Remove(item);
            }
        }
    }
}
