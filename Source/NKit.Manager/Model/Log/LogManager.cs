using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Log
{
    public delegate void WriteDetailDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteMessageDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteExceptionDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, Exception ex, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber);


    public class LogEventArgs : EventArgs
    {
        #region property

        /// <summary>
        /// ローカル。
        /// </summary>
        public DateTime Timestamp { get; set; }

        public NKitApplicationKind SenderApplication { get; set; }

        public NKitLogKind LogKind { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public int TheadId { get; set; }
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }

        #endregion
    }

    public class LogManager : ManagerBase, ILogCreator
    {
        #region event

        public event EventHandler<LogEventArgs> LogWrite;

        #endregion

        #region property
        #endregion

        #region function

        void OnLogWrite(DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            if(LogWrite != null) {
                var e = new LogEventArgs() {
                    SenderApplication = senderApplication,
                    LogKind = logKind,
                    Message = message,
                    Detail = detail,
                    TheadId = threadId,
                    CallerMemberName = callerMemberName,
                    CallerFilePath = callerFilePath,
                    CallerLineNumber = callerLineNumber,
                };
                LogWrite(this, e);
            }
        }

        void Write(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var timestamp = DateTime.Now;
            Debug.WriteLine($"{timestamp} {senderApplication} {logKind} {subject} {message} {detail}");
            OnLogWrite(timestamp, senderApplication, logKind, subject, message, detail, threadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteDetail(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(senderApplication, logKind, subject, message, detail, threadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteMessage(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(senderApplication, logKind, subject, message, null, threadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteException(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, Exception ex, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(senderApplication, logKind, subject, ex.Message, ex.ToString(), threadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        public void WriteTalkLog(TalkLoggingWriteEventArgs e)
        {
            Write(e.SenderApplication, e.LogKind, "Talk", e.Message, e.Detail, e.TheadId, e.CallerMemberName, e.CallerFilePath, e.CallerLineNumber);
        }

        #endregion

        #region ILogCreator

        public ILogger CreateLogger(NKitApplicationKind senderApplication)
        {
            return CreateLogger(senderApplication, null);
        }

        public ILogger CreateLogger(NKitApplicationKind senderApplication, string subject)
        {
            var logger = new Logger(senderApplication, subject, WriteMessage, WriteDetail, WriteException);

            return logger;
        }

        #endregion

    }
}
