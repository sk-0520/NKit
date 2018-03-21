using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Log
{
    public delegate void WriteDetailDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteMessageDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteExceptionDelegate(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber);

    public interface IApplicationLogFactory : ILogFactory
    {
        #region function

        ILogger CreateLogger(NKitApplicationKind senderApplication);
        ILogger CreateLogger(NKitApplicationKind senderApplication, string subject);

        #endregion
    }


    public class LogEventArgs : EventArgs
    {
        #region property

        /// <summary>
        /// 実際にログ出力として使用されたデータ。
        /// </summary>
        public string WriteValue { get; set; }
        public DateTime UtcTimestamp { get; set; }

        public NKitApplicationKind SenderApplication { get; set; }

        public NKitLogKind LogKind { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }


        #endregion
    }

    public class LogManager : ManagerBase, IApplicationLogFactory
    {
        #region define

        struct WriterData
        {
            public WriterData(TextWriter writer, bool leaveOpen)
            {
                Writer = writer;
                LeaveOpen = leaveOpen;
            }

            #region property
            public TextWriter Writer { get; }
            public bool LeaveOpen { get; }
            #endregion
        }

        #endregion

        #region event

        public event EventHandler<LogEventArgs> LogWrite;

        #endregion

        #region property

        HashSet<WriterData> Writers { get; } = new HashSet<WriterData>();

        #endregion

        #region function

        void OnLogWrite(string writeValue, DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            if(LogWrite != null) {
                var e = new LogEventArgs() {
                    WriteValue = writeValue,
                    UtcTimestamp = utcTimestamp,
                    SenderApplication = senderApplication,
                    LogKind = logKind,
                    Message = message,
                    Detail = detail,
                    ProcessId = processId,
                    ThreadId = threadId,
                    CallerMemberName = callerMemberName,
                    CallerFilePath = callerFilePath,
                    CallerLineNumber = callerLineNumber,
                };
                LogWrite(this, e);
            }
        }

        void Write(DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logTimestamp = CommonUtility.ReplaceNKitText(Constants.LogTimestampFormat, utcTimestamp);
            var writeValue = $"{logTimestamp} {senderApplication} {logKind} {subject} {message} {detail}";
            foreach(var data in Writers) {
                data.Writer.WriteLine(writeValue);
            }

            OnLogWrite(writeValue, utcTimestamp, senderApplication, logKind, subject, message, detail, processId, threadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteDetail(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(DateTime.UtcNow, senderApplication, logKind, subject, message, detail, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteMessage(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(DateTime.UtcNow, senderApplication, logKind, subject, message, null, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        void WriteException(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(DateTime.UtcNow, senderApplication, logKind, subject, ex.Message, ex.ToString(), Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId, callerFilePath, callerFilePath, callerLineNumber);
        }

        public void WriteTalkLog(TalkLoggingWriteEventArgs e)
        {
            Write(e.UtcTimestamp, e.SenderApplication, e.LogKind, e.Subject, e.Message, e.Detail, e.ProcessId, e.TheadId, e.CallerMemberName, e.CallerFilePath, e.CallerLineNumber);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="leaveOpen">偽の場合は自動的に解放される。真なら渡し主が解放すること。</param>
        public void AttachOutputWriter(TextWriter writer, bool leaveOpen)
        {
            var data = new WriterData(writer, leaveOpen);
            Writers.Add(data);
        }

        public void DetachOutputWriter(TextWriter writer)
        {
            Writers.RemoveWhere(d => d.Writer == writer);
        }

        #endregion

        #region IApplicationLogFactory

        public ILogger CreateLogger()
        {
            return CreateLogger(null);
        }

        public ILogger CreateLogger(string subject)
        {
            return CreateLogger(NKitApplicationKind.Manager, subject);
        }

        public ILogger CreateLogger(NKitApplicationKind senderApplication)
        {
            return CreateLogger(senderApplication, null);
        }

        public ILogger CreateLogger(NKitApplicationKind senderApplication, string subject)
        {
            return new Logger(senderApplication, subject, WriteMessage, WriteDetail, WriteException);
        }

        #endregion

        #region ManagerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    foreach(var data in Writers.Where(d => !d.LeaveOpen)) {
                        data.Writer.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion

    }
}
