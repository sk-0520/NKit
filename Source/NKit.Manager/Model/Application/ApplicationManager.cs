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

        #region property

        MainApplicationItem MainApplication { get; set; }

        object _itemsLocker = new object();
        IList<ApplicationItem> Items { get; } = new List<ApplicationItem>();

        #endregion

        #region function

        public void ExecuteMainApplication(IReadOnlyWorkspaceItemSetting workspace)
        {
            if(MainApplication != null) {
                MainApplication.ApplicationItem_Exited -= MainApplication_ApplicationItem_Exited;
            }

            MainApplication = new MainApplicationItem(workspace.DirectoryPath);
            MainApplication.ApplicationItem_Exited += MainApplication_ApplicationItem_Exited;

            MainApplication.Execute();
        }

        public void ExecuteNKitApplication(INKitApplicationTalkWakeupMessage message)
        {
            ApplicationItem item = null;

            switch(message.TargetApplication) {
                case NKitApplicationKind.Main:
                    throw new ArgumentException();

                case NKitApplicationKind.Rocket:
                    item = new ApplicationItem(CommonUtility.GetRocketApplication(CommonUtility.GetApplicationDirectory()).FullName) {
                        Arguments = message.Arguments,
                    };
                    break;

                default:
                    throw new NotImplementedException();
            }

            item.ApplicationItem_Exited += Item_ApplicationItem_Exited;
            lock(this._itemsLocker) {
                Items.Add(item);
            }
            item.Execute();
        }

        public void ShutdownOthersApplications()
        {

        }

        #endregion

        private void MainApplication_ApplicationItem_Exited(object sender, EventArgs e)
        {
            ShutdownOthersApplications();

            if(MainApplicationExited != null) {
                MainApplicationExited(sender, e);
            }
        }

        private void Item_ApplicationItem_Exited(object sender, EventArgs e)
        {
            var item = (ApplicationItem)sender;
            item.ApplicationItem_Exited -= Item_ApplicationItem_Exited;

            lock(this._itemsLocker) {
                Items.Remove(item);
            }
        }

    }
}
