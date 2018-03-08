using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ApplicationItem: DisposerBase
    {
        #region event

        public event EventHandler<EventArgs> ApplicationItem_Exited;

        #endregion

        public ApplicationItem(string path)
        {
            Process = new Process();
            Process.StartInfo.FileName = path;
            Process.EnableRaisingEvents = true;
            Process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if(ApplicationItem_Exited != null) {
                ApplicationItem_Exited(this, e);
            }
        }

        #region proeprty

        protected Process Process { get; }

        public string Arguments
        {
            get { return Process.StartInfo.Arguments; }
            set { Process.StartInfo.Arguments = value; }
        }

        #endregion

        #region function

        public void Execute()
        {
            Process.Start();
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    Process.Exited -= Process_Exited;
                    Process.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    /// <summary>
    /// この子だけ特別扱い。
    /// </summary>
    public class MainApplicationItem: ApplicationItem
    {
        public MainApplicationItem(string workspaceDirectoryPath)
            : base(CommonUtility.GetMainApplication(CommonUtility.GetApplicationDirectory()).FullName)
        {
            Arguments = $"--workspace \"{workspaceDirectoryPath}\" --その他なんか いっぱい";
        }
    }
}
