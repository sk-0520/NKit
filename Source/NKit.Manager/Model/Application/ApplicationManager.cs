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

        #region variable

        /// <summary>
        /// 現在の最大ID。
        /// <para>オーバーフロー？ ないない。</para>
        /// <para>実運用でそこまで起動することもないでしょ。。。</para>
        /// </summary>
        uint _manageIdSequence = 0;
        object _manageLock = new object();

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

        IList<ManageItem> ManageItems { get; } = new List<ManageItem>();

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

            return sourceArguments + " " + headArgs;
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

        void ExecuteManageItem(ApplicationItem item)
        {
            item.Exited += Item_Exited;

            Logger.Debug(item.Path);
            Logger.Debug(item.Arguments);

            ManageItem manageItem = null;
            lock(this._manageLock) {
                var manageId = ++this._manageIdSequence;
                manageItem = new ManageItem(manageId, item);
                ManageItems.Add(manageItem);
            }

            Logger.Debug($"ID: {manageItem.ManageId}, Path: {manageItem.ApplicationItem.Path}, Arguments: {manageItem.ApplicationItem.Arguments}");

            item.Execute();
        }

        void ExitedManageItem(ApplicationItem item)
        {
            item.Exited -= Item_Exited;

            // 削除するわけでもないしロックいるか？
            //lock(this._manageLock) {
            var manageItem = ManageItems.FirstOrDefault(i => i.ApplicationItem == item);
            if(manageItem != null) {
                Logger.Information($"item exited: {item.Path}, {item.Arguments}");
                manageItem.Exited();
            } else {
                Logger.Warning($"unknown item exited: {item.Path}, {item.Arguments}");
            }
            //}

            if(ApplicationExited != null) {
                ApplicationExited(this, EventArgs.Empty);
            }
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

            ExecuteManageItem(item);
        }

        public void ExecuteOtherApplication(NKitApplicationKind senderApplication, string programPath, IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            var item = new ApplicationItem(programPath) {
                Arguments = arguments,
                WorkingDirectoryPath = workingDirectoryPath,
                CreateWindow = false,
                IsOutputReceive = true,
            };

            ExecuteManageItem(item);
        }

        public void ShutdownAllApplications()
        {

        }

        #endregion

        private void MainApplication_Exited(object sender, EventArgs e)
        {
            ShutdownAllApplications();

            if(MainApplicationExited != null) {
                MainApplicationExited(sender, e);
            }
        }

        private void Item_Exited(object sender, EventArgs e)
        {
            ExitedManageItem((ApplicationItem)sender);
        }
    }
}
