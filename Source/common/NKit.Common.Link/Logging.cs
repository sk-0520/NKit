using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public interface ILogger
    {
        void Trace(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Trace(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Trace(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Debug(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Debug(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Debug(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Information(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Information(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Information(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Warning(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Warning(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Warning(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Error(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Error(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Error(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);

        void Fatal(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Fatal(string message, string detail, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
        void Fatal(Exception ex, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0);
    }

    public interface ILogFactory
    {
        #region function

        ILogger CreateLogger();
        ILogger CreateLogger(string subject);

        #endregion
    }
}
