using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public enum NKitApplicationKind
    {
        Manager,
        Main,
        Rocket,
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

    public interface INKitLoggingTalker
    {
        #region function


        void Write(NKitApplicationKind senderApplication, NKitLogKind logKind, string message, string detail, int theadId, string callerMemberName, string callerFileName, int callerLineNumber);

        #endregion
    }
}
