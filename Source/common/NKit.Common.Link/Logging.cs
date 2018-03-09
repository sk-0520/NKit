using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public enum Log
    {
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

    public interface INKitLoggingTalker
    {
        #region function

        void Write(NKitApplicationKind senderApplication, string message, string detail, int theadId, int callerLineNumber, string callerMemberName, string callerFileName);

        #endregion
    }
}
