using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ApplicationItem : DisposerBase
    {
        #region event

        public event EventHandler<EventArgs> Exited;
        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;

        #endregion

        public ApplicationItem(string path)
        {
            Process = new Process();
            Process.StartInfo.FileName = path;

            Process.EnableRaisingEvents = true;
            Process.Exited += Process_Exited;
        }

        #region proeprty

        protected Process Process { get; }

        public string Arguments { get; set; }
        public string WorkingDirectoryPath { get; set; }
        public bool RedirectOutput { get; set; }
        public bool CreateWindow { get; set; }

        #endregion

        #region function

        public void Execute()
        {
            Process.StartInfo.Arguments = Arguments;
            Process.StartInfo.WorkingDirectory = WorkingDirectoryPath;

            Process.StartInfo.CreateNoWindow = !CreateWindow;
            Process.StartInfo.UseShellExecute = !RedirectOutput;

            Process.StartInfo.RedirectStandardOutput = RedirectOutput;
            Process.StartInfo.RedirectStandardError = RedirectOutput;
            if(RedirectOutput) {
                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
            }

            Process.Start();

            if(RedirectOutput) {
                Process.BeginErrorReadLine();
                Process.BeginOutputReadLine();
            }
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(RedirectOutput) {
                        Process.OutputDataReceived -= OutputDataReceived;
                        Process.ErrorDataReceived -= Process_ErrorDataReceived;
                    }
                    Process.Exited -= Process_Exited;

                    Process.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        private void Process_Exited(object sender, EventArgs e)
        {
            if(RedirectOutput) {
                Process.OutputDataReceived -= OutputDataReceived;
                Process.ErrorDataReceived -= Process_ErrorDataReceived;
            }

            if(Exited != null) {
                Exited(this, e);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(OutputDataReceived != null) {
                OutputDataReceived(this, e);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(ErrorDataReceived != null) {
                ErrorDataReceived(this, e);
            }
        }
    }

    public class NKitApplicationItem : ApplicationItem
    {
        public NKitApplicationItem(NKitApplicationKind kind)
            : base(GetApplicationPath(kind))
        {
            Kind = kind;

            if(kind == NKitApplicationKind.Main) {
                RedirectOutput = true;
                CreateWindow = true;
            } else {
                RedirectOutput = true;
                CreateWindow = false;
            }
        }

        #region property

        public NKitApplicationKind Kind { get; }

        #endregion

        #region function

        static string GetApplicationPath(NKitApplicationKind kind)
        {
            switch(kind) {
                case NKitApplicationKind.Main:
                    return CommonUtility.GetMainApplication(CommonUtility.GetApplicationDirectory()).FullName;

                case NKitApplicationKind.Rocket:
                    return CommonUtility.GetRocketApplication(CommonUtility.GetApplicationDirectory()).FullName;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }

    /// <summary>
    /// この子だけ特別扱い。
    /// </summary>
    public class NKitMainApplicationItem : NKitApplicationItem
    {
        public NKitMainApplicationItem(string workspaceDirectoryPath)
            : base(NKitApplicationKind.Main)
        {
            Arguments = $"--workspace \"{workspaceDirectoryPath}\" --その他なんか いっぱい";
        }
    }
}
