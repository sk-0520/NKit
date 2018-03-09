using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Log
{
    public class Logger : DisposerBase, ILogger
    {
        public Logger(NKitApplicationKind senderApplication, string subject, WriteMessageDelegate messageWriter, WriteDetailDelegate detailWriter, WriteExceptionDelegate exceptionWriter)
        {
            SenderApplication = senderApplication;
            Subject = subject;
            MessageWriter = messageWriter;
            DetailWriter = detailWriter;
            ExceptionWriter = exceptionWriter;
        }

        #region property

        public NKitApplicationKind SenderApplication { get; }
        public string Subject { get; }

        WriteMessageDelegate MessageWriter { get; set; }
        WriteDetailDelegate DetailWriter { get; set; }
        WriteExceptionDelegate ExceptionWriter { get; set; }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    MessageWriter = null;
                    DetailWriter = null;
                    ExceptionWriter = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region ILogger

        public void Trace(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Trace, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Trace(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Trace, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Trace(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Trace, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Debug(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Debug, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Debug(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Debug, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Debug(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Debug, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Information(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Information, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Information(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Information, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Information(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Information, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Warning(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Warning, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Warning(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Warning, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Warning(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Warning, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Error, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Error(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Error, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Error, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }


        public void Fatal(string message, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(SenderApplication, NKitLogKind.Fatal, Subject, message, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Fatal(string message, string detail, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(SenderApplication, NKitLogKind.Fatal, Subject, message, detail, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Fatal(Exception ex, int threadId = 0, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(SenderApplication, NKitLogKind.Fatal, Subject, ex, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        #endregion

    }
}
