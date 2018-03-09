using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        /// 実際にログ出力として使用されたデータ。
        /// </summary>
        public string WriteValue { get; set; }
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

        void OnLogWrite(string writeValue, DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            if(LogWrite != null) {
                var e = new LogEventArgs() {
                    WriteValue = writeValue,
                    Timestamp = timestamp,
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

            var writeValue = $"{timestamp} {senderApplication} {logKind} {subject} {message} {detail}";
            Debug.WriteLine(writeValue);
            foreach(var data in Writers) {
                data.Writer.WriteLine(writeValue);
            }

            OnLogWrite(writeValue, timestamp, senderApplication, logKind, subject, message, detail, threadId, callerFilePath, callerFilePath, callerLineNumber);
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
