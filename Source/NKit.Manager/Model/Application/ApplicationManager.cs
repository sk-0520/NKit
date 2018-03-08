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

        NKitMainApplicationItem MainApplication { get; set; }

        object _itemsLocker = new object();
        IList<NKitApplicationItem> Items { get; } = new List<NKitApplicationItem>();

        #endregion

        #region function

        public void ExecuteMainApplication(IReadOnlyWorkspaceItemSetting workspace)
        {
            if(MainApplication != null) {
                MainApplication.Exited -= MainApplication_Exited;
            }

            MainApplication = new NKitMainApplicationItem(workspace.DirectoryPath);
            MainApplication.Exited += MainApplication_Exited;
            MainApplication.OutputDataReceived += Item_Application_OutputDataReceived;
            MainApplication.ErrorDataReceived += Item_Application_ErrorDataReceived;

            MainApplication.Execute();
        }

        public void ExecuteNKitApplication(NKitApplicationKind senderApplication, NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            NKitApplicationItem item = null;

            switch(targetApplication) {
                case NKitApplicationKind.Main:
                    throw new ArgumentException();

                case NKitApplicationKind.Rocket:
                    item = new NKitApplicationItem(targetApplication) {
                        Arguments = arguments,
                    };
                    break;

                default:
                    throw new NotImplementedException();
            }

            item.Exited += Item_Exited;
            item.OutputDataReceived += Item_Application_OutputDataReceived;
            item.ErrorDataReceived += Item_Application_ErrorDataReceived;
            lock(this._itemsLocker) {
                Items.Add(item);
            }
            item.Execute();
        }

        public void ShutdownOthersApplications()
        {

        }

        private void ReceivedData(NKitApplicationItem item, bool isError, string s)
        {
            if(s != null){
                var type = isError ? "E" : "I";
                Debug.WriteLine($"[{type}] {item.Kind}, {s}");
            }
        }

        #endregion

        private void MainApplication_Exited(object sender, EventArgs e)
        {
            MainApplication.OutputDataReceived -= Item_Application_OutputDataReceived;
            MainApplication.ErrorDataReceived -= Item_Application_ErrorDataReceived;

            ShutdownOthersApplications();

            if(MainApplicationExited != null) {
                MainApplicationExited(sender, e);
            }
        }

        private void Item_Exited(object sender, EventArgs e)
        {
            var item = (NKitApplicationItem)sender;
            item.Exited -= Item_Exited;

            item.OutputDataReceived -= Item_Application_OutputDataReceived;
            item.ErrorDataReceived -= Item_Application_OutputDataReceived;

            lock(this._itemsLocker) {
                Items.Remove(item);
            }
        }

        private void Item_Application_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var item = (NKitApplicationItem)sender;
            ReceivedData(item, true, e.Data);
        }

        private void Item_Application_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var item = (NKitApplicationItem)sender;
            ReceivedData(item, false, e.Data);
        }

    }
}
