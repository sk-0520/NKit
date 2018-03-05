using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{

    public class ApplicationExecutor : RunnableModelBase<int>
    {
        public ApplicationExecutor(string executeFilePath, string arguments)
        {
            ExecuteFilePath = executeFilePath;
            Arguments = arguments;
        }

        #region property

        protected string ExecuteFilePath { get; }
        protected string Arguments { get; set; }

        protected Process ExecuteProcess { get; private set; }

        /// <summary>
        /// プログラムの実行タスク。
        /// </summary>
        protected TaskCompletionSource<int> ExecuteTask { get; set; }

        #endregion

        #region function

        protected virtual void AfterStarted()
        { }

        protected virtual void ExitedProcess()
        { }

        #endregion

        #region RunnableModelBase

        protected override Task<PreparaResult<int>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            ExecuteProcess = new Process();

            ExecuteProcess.StartInfo.FileName = ExecuteFilePath;
            ExecuteProcess.StartInfo.Arguments = Arguments;
            ExecuteProcess.EnableRaisingEvents = true;
            ExecuteProcess.Exited += Process_Exited;

            return base.PreparationCoreAsync(cancelToken);
        }

        protected sealed override Task<int> RunCoreAsync(CancellationToken cancelToken)
        {
            ExecuteTask = new TaskCompletionSource<int>();

            ExecuteProcess.Start();

            AfterStarted();

            return ExecuteTask.Task;
        }

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(ExecuteProcess != null) {
                    ExecuteProcess.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        private void Process_Exited(object sender, EventArgs e)
        {
            var process = (Process)sender;
            process.Exited -= Process_Exited;

            ExitedProcess();

            ExecuteTask.SetResult(process.ExitCode);
        }
    }

    public class CliApplicationExecutor : ApplicationExecutor
    {
        public CliApplicationExecutor(string executeFilePath, string arguments)
            : base(executeFilePath, arguments)
        { }

        #region function

        protected virtual void ReceivedOutputData(DataReceivedEventArgs e)
        { }

        protected virtual void ReceivedErrorData(DataReceivedEventArgs e)
        { }

        #endregion

        #region ApplicationExecutor

        protected override void AfterStarted()
        {
            base.AfterStarted();

            ExecuteProcess.BeginErrorReadLine();
            ExecuteProcess.BeginOutputReadLine();
        }

        protected override Task<PreparaResult<int>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            var result = base.PreparationCoreAsync(cancelToken);

            ExecuteProcess.StartInfo.UseShellExecute = false;
            ExecuteProcess.StartInfo.CreateNoWindow = true;
            ExecuteProcess.StartInfo.RedirectStandardOutput = true;
            ExecuteProcess.StartInfo.RedirectStandardError = true;
            ExecuteProcess.ErrorDataReceived += ExecuteProcess_ErrorDataReceived;
            ExecuteProcess.OutputDataReceived += ExecuteProcess_OutputDataReceived;

            return result;
        }

        protected override void ExitedProcess()
        {
            ExecuteProcess.ErrorDataReceived -= ExecuteProcess_ErrorDataReceived;
            ExecuteProcess.OutputDataReceived -= ExecuteProcess_OutputDataReceived;

            base.ExitedProcess();
        }

        #endregion

        private void ExecuteProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null) {
                return;
            }

            ReceivedOutputData(e);
        }

        private void ExecuteProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == null) {
                return;
            }

            ReceivedErrorData(e);
        }
    }

    public class ActionCliApplicationExecutor : CliApplicationExecutor
    {
        public ActionCliApplicationExecutor(string executeFilePath, string arguments)
            : base(executeFilePath, arguments)
        { }

        #region property

        public Action<DataReceivedEventArgs> ReceivedOutput { get; set; }
        public Action<DataReceivedEventArgs> ReceivedError { get; set; }

        public StreamWriter StandardInput => ExecuteProcess.StandardInput;

        #endregion

        #region function

        #endregion

        #region CliApplicationExecutor

        protected override void ReceivedOutputData(DataReceivedEventArgs e)
        {
            ReceivedOutput?.Invoke(e);
            base.ReceivedOutputData(e);
        }

        protected override void ReceivedErrorData(DataReceivedEventArgs e)
        {
            ReceivedError?.Invoke(e);
            base.ReceivedErrorData(e);
        }

        #endregion
    }
}
