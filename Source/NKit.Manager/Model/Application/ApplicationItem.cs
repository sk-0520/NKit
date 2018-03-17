using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Log;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public struct ApplicationOutput
    {
        public ApplicationOutput(bool isError, string line)
        {
            IsError = isError;
            Line = line;
        }

        #region property

        public string Line { get; }
        public bool IsError { get; }

        #endregion
    }

    public class ApplicationItem : DisposerBase
    {
        #region event

        public event EventHandler<EventArgs> Exited;

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

        public string Path => Process.StartInfo.FileName;

        public string Arguments { get; set; }
        public string WorkingDirectoryPath { get; set; }
        public bool IsOutputReceive { get; set; }
        public bool CreateWindow { get; set; }

        public bool IsRunning { get; private set; }
        public bool IsExited { get; private set; }

        protected List<ApplicationOutput> _outputMessages;

        public IReadOnlyList<ApplicationOutput> OutputMessages => this._outputMessages;

        #endregion

        #region function

        public void Execute()
        {
            Process.StartInfo.Arguments = Arguments;
            Process.StartInfo.WorkingDirectory = WorkingDirectoryPath;

            Process.StartInfo.CreateNoWindow = !CreateWindow;
            Process.StartInfo.UseShellExecute = !IsOutputReceive;

            Process.StartInfo.RedirectStandardOutput = IsOutputReceive;
            Process.StartInfo.RedirectStandardError = IsOutputReceive;
            if(IsOutputReceive) {
                this._outputMessages = new List<ApplicationOutput>();
                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
            }

            Process.Start();

            IsRunning = true;

            if(IsOutputReceive) {
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
                    Process.Exited -= Process_Exited;
                    if(IsOutputReceive) {
                        Process.OutputDataReceived -= Process_ErrorDataReceived;
                        Process.ErrorDataReceived -= Process_ErrorDataReceived;
                    }

                    Process.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected virtual void ReceiveOutputData(ApplicationOutput output)
        {
            this._outputMessages.Add(output);

        }

        #endregion

        private void Process_Exited(object sender, EventArgs e)
        {
            Process.Exited -= Process_Exited;
            Process.OutputDataReceived -= Process_ErrorDataReceived;
            Process.ErrorDataReceived -= Process_ErrorDataReceived;

            IsRunning = false;
            IsExited = false;

            if(Exited != null) {
                Exited(this, e);
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null) {
                ReceiveOutputData(new ApplicationOutput(false, e.Data));
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data!= null) {
                ReceiveOutputData(new ApplicationOutput(true, e.Data));
            }
        }
    }

    public class NKitApplicationItem : ApplicationItem
    {
        public NKitApplicationItem(NKitApplicationKind kind, IApplicationLogFactory logFactory)
            : base(GetApplicationPath(kind))
        {
            Kind = kind;

            Logger = logFactory.CreateLogger(Kind);

            if(kind == NKitApplicationKind.Main) {
                IsOutputReceive = true;
                CreateWindow = true;
            } else {
                IsOutputReceive = true;
                CreateWindow = false;
            }
        }

        #region property

        public NKitApplicationKind Kind { get; }

        ILogger Logger { get; }

        #endregion

        #region function

        static string GetApplicationPath(NKitApplicationKind kind)
        {
            switch(kind) {
                case NKitApplicationKind.Main:
                    return CommonUtility.GetMainApplication(CommonUtility.GetApplicationDirectory()).FullName;

                case NKitApplicationKind.Rocket:
                    return CommonUtility.GetRocketApplication(CommonUtility.GetApplicationDirectory()).FullName;

                case NKitApplicationKind.Cameraman:
                    return CommonUtility.GetCameramanApplication(CommonUtility.GetApplicationDirectory()).FullName;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region ApplicationItem

        protected override void ReceiveOutputData(ApplicationOutput output)
        {
            base.ReceiveOutputData(output);

            if(output.IsError) {
                Logger.Error(output.Line);
            } else {
                Logger.Information(output.Line);
            }
        }

        #endregion
    }
}
