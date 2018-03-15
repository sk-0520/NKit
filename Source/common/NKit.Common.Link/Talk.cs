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

        [OperationContract]
        void WakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath);

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
