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

        public ApplicationManager(IApplicationLogCreator logCreator)
        {
            LogCreator = logCreator;
            Logger = LogCreator.CreateLogger("AP");
        }

        #region property

        NKitApplicationItem MainApplication { get; set; }

        IApplicationLogCreator LogCreator { get; }
        ILogger Logger { get; }

        object _itemsLocker = new object();
        IList<ApplicationItem> Items { get; } = new List<ApplicationItem>();

        #endregion

        #region function

        string AddNKitArguments(IReadOnlyWorkspaceVolatilityItem workspaceVolatilityItem, IReadOnlyWorkspaceItemSetting workspaceItemSetting, string sourceArguments)
        {
            //TODO
            return sourceArguments;
        }

        public void ExecuteMainApplication(IReadOnlyWorkspaceVolatilityItem workspaceVolatilityItem, IReadOnlyWorkspaceItemSetting workspaceItemSetting)
        {
            if(MainApplication != null) {
                MainApplication.Exited -= MainApplication_Exited;
            }

            MainApplication = new NKitApplicationItem(NKitApplicationKind.Main, LogCreator) {
                Arguments = AddNKitArguments(workspaceVolatilityItem, workspaceItemSetting, string.Empty)
            };

            MainApplication.Exited += MainApplication_Exited;

            MainApplication.Execute();
        }

        public void ExecuteNKitApplication(NKitApplicationKind senderApplication, NKitApplicationKind targetApplication, IReadOnlyWorkspaceVolatilityItem workspaceVolatilityItem, IReadOnlyWorkspaceItemSetting workspace, string arguments, string workingDirectoryPath)
        {
            ApplicationItem item = null;

            switch(targetApplication) {
                case NKitApplicationKind.Main:
                    throw new ArgumentException();

                case NKitApplicationKind.Rocket:
                    item = new NKitApplicationItem(targetApplication, LogCreator) {
                        Arguments = AddNKitArguments(workspaceVolatilityItem, workspace, arguments),
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
