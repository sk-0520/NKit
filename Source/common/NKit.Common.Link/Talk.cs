using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public enum NKitApplicationKind
    {
        /// <summary>
        /// よその子。
        /// </summary>
        Others,
        Manager,
        Main,
        Rocket,
        Cameraman,
    }

    [ServiceContract]
    public interface INKitApplicationTalker
    {
        #region function

        /// <summary>
        /// NKit プログラムを起動する。
        /// </summary>
        /// <param name="sender">起動要求プログラム。</param>
        /// <param name="target">起動対象プログラム。</param>
        /// <param name="arguments">引数。</param>
        /// <param name="workingDirectoryPath">作業ディレクトリパス。</param>
        /// <returns>管理 ID。 0 は無効ID。</returns>
        [OperationContract]
        int WakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath);

        #endregion
    }

    public enum NKitLogKind
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }

    [ServiceContract]
    public interface INKitLoggingTalker
    {
        #region function

        [OperationContract]
        void Write(DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int theadId, string callerMemberName, string callerFileName, int callerLineNumber);

        #endregion
    }
}
