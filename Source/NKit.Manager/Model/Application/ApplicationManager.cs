using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Log;
using ContentTypeTextNet.NKit.Manager.Model.Workspace;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ApplicationManager : ManagerBase
    {
        #region define

        static readonly TimeSpan CloseWaitTime = TimeSpan.FromMilliseconds(500);

        #endregion

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

        /// <summary>
        /// うおー、こっち int しか無理なんかよ。
        /// </summary>
        int _exitedSequence = 0;

        #endregion

        public ApplicationManager(IApplicationLogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("AP");
        }

        #region property

        ManageItem MainApplication { get; set; }

        IApplicationLogFactory LogFactory { get; }
        ILogger Logger { get; }

        IList<ManageItem> ManageItems { get; } = new List<ManageItem>();

        EventWaitHandle GroupSuicideEvent { get; set; }

        #endregion

        #region function

        string CreateExitedEventName(IReadOnlyActiveWorkspace activeWorkspace)
        {
            var number = Interlocked.Increment(ref this._exitedSequence);
            return $"cttn-nkit-exited-{activeWorkspace.BaseId}-{number}";
        }

        IReadOnlyDictionary<string, string> CreateNKitArguments(IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspaceItemSetting)
        {
            var result = new Dictionary<string, string>() {
                //[CommonUtility.ManagedStartup.ExecuteFlag] = string.Empty,
                [CommonUtility.ManagedStartup.ServiceUri] = activeWorkspace.ServiceUri.ToString(),
                [CommonUtility.ManagedStartup.WorkspacePath] = workspaceItemSetting.DirectoryPath,
                [CommonUtility.ManagedStartup.ApplicationId] = activeWorkspace.ApplicationId,
                [CommonUtility.ManagedStartup.GroupSuicideEventName] = activeWorkspace.GroupSuicideEventName,
                [CommonUtility.ManagedStartup.AloneSuicideEventName] = $"cttn-nkit-alone-suicide-{activeWorkspace.BaseId}-{Guid.NewGuid()}",
            };

            return result;
        }

        string AddNKitArguments(string sourceArguments, IReadOnlyDictionary<string, string> nkitArguments)
        {
            var list = new[] {
                sourceArguments != null && sourceArguments.IndexOf(CommonUtility.ManagedStartup.ExecuteFlag) != -1
                    ? string.Empty
                    : CommonUtility.ManagedStartup.ExecuteFlag
                ,

                CommonUtility.ManagedStartup.ServiceUri,
                ProgramRelationUtility.EscapesequenceToArgument(nkitArguments[CommonUtility.ManagedStartup.ServiceUri]) ,

                CommonUtility.ManagedStartup.WorkspacePath,
                ProgramRelationUtility.EscapesequenceToArgument(nkitArguments[CommonUtility.ManagedStartup.WorkspacePath]),

                CommonUtility.ManagedStartup.ApplicationId,
                ProgramRelationUtility.EscapesequenceToArgument(nkitArguments[CommonUtility.ManagedStartup.ApplicationId]),

                CommonUtility.ManagedStartup.GroupSuicideEventName,
                ProgramRelationUtility.EscapesequenceToArgument(nkitArguments[CommonUtility.ManagedStartup.GroupSuicideEventName]),
            };
            var headArgs = string.Join(" ", list);

            return sourceArguments + " " + headArgs;
        }

        public void ExecuteMainApplication(IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspaceItemSetting)
        {
            if(MainApplication != null) {
                MainApplication.ApplicationItem.Exited -= MainApplication_Exited;
            }

            var exitedEventName = CreateExitedEventName(activeWorkspace);
            var nkitArgs = CreateNKitArguments(activeWorkspace, workspaceItemSetting);
            var mainApplication = new NKitApplicationItem(NKitApplicationKind.Main, LogFactory) {
                Arguments = AddNKitArguments(string.Empty, nkitArgs)
            };
            MainApplication = new ManageItem(0, mainApplication, exitedEventName, nkitArgs);
            Logger.Debug($"unmanage main, Path: {MainApplication.ApplicationItem.Path}, Arguments: {MainApplication.ApplicationItem.Arguments}");

            GroupSuicideEvent = new EventWaitHandle(false, EventResetMode.ManualReset, activeWorkspace.GroupSuicideEventName);

            MainApplication.ApplicationItem.Exited += MainApplication_Exited;

            MainApplication.ApplicationItem.Execute();
        }

        uint PreparateManageItem(ApplicationItem item, string exitedEventName, IReadOnlyDictionary<string, string> nkitArgs)
        {
            item.Exited += Item_Exited;

            Logger.Debug(item.Path);
            Logger.Debug(item.Arguments);

            ManageItem manageItem = null;
            lock(this._manageLock) {
                var manageId = ++this._manageIdSequence;
                manageItem = new ManageItem(manageId, item, exitedEventName, nkitArgs);
                ManageItems.Add(manageItem);
            }

            Logger.Debug($"ID: {manageItem.ManageId}, Path: {manageItem.ApplicationItem.Path}, Arguments: {manageItem.ApplicationItem.Arguments}");

            //item.Execute();

            return manageItem.ManageId;
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

        public uint PreparateNKitApplication(NKitApplicationKind senderApplication, NKitApplicationKind targetApplication, IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            ApplicationItem item = null;

            var exitedEventName = CreateExitedEventName(activeWorkspace);
            var nkitArgs = CreateNKitArguments(activeWorkspace, workspace);

            switch(targetApplication) {
                case NKitApplicationKind.Main: // 通常処理として起動する可能性あり(想定する用途としては初回一括起動)
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(arguments, nkitArgs),
                    };
                    break;

                case NKitApplicationKind.Rocket:
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(arguments, nkitArgs),
                    };
                    break;

                case NKitApplicationKind.Cameraman:
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(arguments, nkitArgs),
                    };
                    break;

                case NKitApplicationKind.JustLooking:
                    item = new NKitApplicationItem(targetApplication, LogFactory) {
                        Arguments = AddNKitArguments(arguments, nkitArgs),
                    };
                    break;

                default:
                    throw new NotImplementedException();
            }

            return PreparateManageItem(item, exitedEventName, nkitArgs);
        }

        public uint PreparateOtherApplication(NKitApplicationKind senderApplication, string programPath, IReadOnlyActiveWorkspace activeWorkspace, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            var exitedEventName = CreateExitedEventName(activeWorkspace);
            var item = new ApplicationItem(programPath) {
                Arguments = arguments,
                WorkingDirectoryPath = workingDirectoryPath,
                CreateWindow = false,
                IsOutputReceive = true,
            };

            return PreparateManageItem(item, exitedEventName, new Dictionary<string, string>());
        }

        bool TryGetManageItem(uint manageId, out ManageItem result)
        {
            ManageItem manageItem = null;
            lock(this._manageLock) {
                manageItem = ManageItems.FirstOrDefault(i => i.ManageId == manageId);
            }
            result = manageItem;

            return result != null;
        }

        public bool WakeupNKitApplication(NKitApplicationKind senderApplication, uint manageId)
        {
            if(TryGetManageItem(manageId, out var manageItem)) {
                //TODO: 起動状態とかもう死んでるとか調べた方がいい

                manageItem.ApplicationItem.Execute();
                return true;
            }

            return false;
        }

        public NKitApplicationStatus GetStatus(NKitApplicationKind senderApplication, uint manageId)
        {
            if(TryGetManageItem(manageId, out var manageItem)) {
                //TODO: 起動状態とかもう死んでるとか調べた方がいい

                var result = new NKitApplicationStatus() {
                    IsEnabled = true,
                    Running = manageItem.ApplicationItem.IsRunning,
                    Exited = manageItem.ApplicationItem.IsExited,
                    ExitedEventName = manageItem.ExitedEventName,
                };

                return result;
            }

            return new NKitApplicationStatus() {
                IsEnabled = false,
            };
        }

        public bool Shutdown(NKitApplicationKind senderApplication, uint manageId, bool force)
        {
            if(TryGetManageItem(manageId, out var manageItem)) {
                //TODO: 起動状態とかもう死んでるとか調べた方がいい
                var logger = LogFactory.CreateLogger(manageItem.GetType().Name);
                var closeResult = manageItem.Close(logger);
                if(closeResult) {
                    return true;
                }
                if(force) {
                    return manageItem.Kill(logger);
                }
            }

            return false;
        }

        void ShutdownManageApplications()
        {
            var logger = LogFactory.CreateLogger("shutdown");
            logger.Information($"event set: {nameof(GroupSuicideEvent)}, {CommonUtility.ManagedStartup.GroupSuicideEventName}");
            GroupSuicideEvent.Set();

            Thread.Sleep(CloseWaitTime);

            var runningItems = ManageItems
                .Where(i => i.ApplicationItem.IsRunning)
                .ToList()
            ;
            logger.Information($"running: {runningItems.Count}");
            foreach(var item in runningItems) {
                if(!item.Close(logger)) {
                    item.Kill(logger);
                }
            }
        }

        public void ShutdownMainApplication()
        {
            // こわいし
            ShutdownManageApplications();

            var logger = LogFactory.CreateLogger("main");
            Thread.Sleep(CloseWaitTime);
            if(!MainApplication.Close(logger)) {
                MainApplication.Kill(logger);
            }
        }

        #endregion

        private void MainApplication_Exited(object sender, EventArgs e)
        {
            ShutdownManageApplications();

            if(MainApplicationExited != null) {
                MainApplicationExited(sender, e);
            }

            GroupSuicideEvent.Dispose();
        }

        private void Item_Exited(object sender, EventArgs e)
        {
            ExitedManageItem((ApplicationItem)sender);
        }

    }
}
