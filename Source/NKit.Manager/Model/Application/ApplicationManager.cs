using System;
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

        public event EventHandler<EventArgs> MainProcessExited;

        #endregion

        #region property

        Process MainProcess { get; set; }

        #endregion

        #region function

        public void ExecuteMainApplication(IReadOnlyWorkspaceItemSetting workspace)
        {
            // TODO: 名称変更のあれ
            var mainExecPath = Path.Combine(CommonUtility.GetApplicationDirectory().FullName, "NKit.Main.exe");
            var arguments = $"--workspace \"{workspace.DirectoryPath}\" --その他なんか いっぱい";
            MainProcess = new Process();
            MainProcess.StartInfo.FileName = mainExecPath;
            MainProcess.StartInfo.Arguments = arguments;
            MainProcess.EnableRaisingEvents = true;
            MainProcess.Exited += MainProcess_Exited;

            MainProcess.Start();
        }

        #endregion
        private void MainProcess_Exited(object sender, EventArgs e)
        {
            if(MainProcessExited != null) {
                MainProcessExited(sender, e);
            }
        }
    }
}
