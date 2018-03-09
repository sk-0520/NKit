using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public interface ILogger
    {
        void Trace(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Trace(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Trace(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Debug(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Debug(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Debug(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Information(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Information(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Information(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Warning(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Warning(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Warning(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Error(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Error(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Error(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Fatal(string message, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Fatal(string message, string detail, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Fatal(Exception ex, int threadId = 0, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
    }

    public interface ILogCreator
    {
        #region function

        ILogger CreateLogger(NKitApplicationKind senderApplication);
        ILogger CreateLogger(NKitApplicationKind senderApplication, string subject);

        #endregion
    }

}
