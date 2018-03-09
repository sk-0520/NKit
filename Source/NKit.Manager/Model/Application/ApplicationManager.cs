using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ApplicationManager : ManagerBase
    {
        #region event

        public event EventHandler<EventArgs> MainApplicationExited;

        #endregion

        public ApplicationManager(ILogCreator logCreator)
        {
            LogCreator = logCreator;
        }

        #region property

        NKitApplicationItem MainApplication { get; set; }

        ILogCreator LogCreator { get; }

        object _itemsLocker = new object();
        IList<ApplicationItem> Items { get; } = new List<ApplicationItem>();

        #endregion

        #region function

        public void ExecuteMainApplication(IReadOnlyWorkspaceItemSetting workspace)
        {
            if(MainApplication != null) {
                MainApplication.Exited -= MainApplication_Exited;
            }

            MainApplication = new NKitApplicationItem(NKitApplicationKind.Main, LogCreator) {
                Arguments = $"--workspace {workspace.DirectoryPath}"
            };
            
            MainApplication.Exited += MainApplication_Exited;

            MainApplication.Execute();
        }

        public void ExecuteNKitApplication(NKitApplicationKind senderApplication, NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            ApplicationItem item = null;

            switch(targetApplication) {
                case NKitApplicationKind.Main:
                    throw new ArgumentException();

                case NKitApplicationKind.Rocket:
                    item = new NKitApplicationItem(targetApplication, LogCreator) {
                        Arguments = arguments,
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
