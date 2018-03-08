using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Main.Model.NKit
{
    //TODO: たぶんこいつはいなくなる
    public class NKitCliApplicationExecutor: CliApplicationExecutor
    {
        public NKitCliApplicationExecutor(Func<DirectoryInfo, FileInfo> applicationFileGetter, string arguments)
            :base(applicationFileGetter(CommonUtility.GetApplicationDirectoryForApplication()).FullName, arguments)
        { }


        #region CliApplicationExecutor

        protected override void ReceivedErrorData(DataReceivedEventArgs e)
        {
            Debug.WriteLine($"{Path.GetFileNameWithoutExtension(ExecuteFilePath)}[E]: {e.Data}");
        }

        protected override void ReceivedOutputData(DataReceivedEventArgs e)
        {
            Debug.WriteLine($"{Path.GetFileNameWithoutExtension(ExecuteFilePath)}[I]: {e.Data}");
        }

        protected override void ExitedProcess()
        {
            base.ExitedProcess();

            Debug.WriteLine($"{Path.GetFileNameWithoutExtension(ExecuteFilePath)}[I]: {ExecuteProcess.ExitCode}, {ExecuteProcess.ExitTime - ExecuteProcess.StartTime}");
        }

        #endregion
    }
}
