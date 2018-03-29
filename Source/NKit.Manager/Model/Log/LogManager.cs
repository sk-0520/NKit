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
        public NKitLogData LogData { get; set; }


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

        void OnLogWrite(string writeValue, DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogData logData)
        {
            if(LogWrite != null) {
                var e = new LogEventArgs() {
                    WriteValue = writeValue,
                    UtcTimestamp = utcTimestamp,
                    SenderApplication = senderApplication,
                    LogData = logData,
                };
                LogWrite(this, e);
            }
        }



        void Write(DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogData logData)
        {
            var logTimestamp = CommonUtility.ReplaceNKitText(Constants.LogTimestampFormat, utcTimestamp);

            var writeValue = $"{logTimestamp} {logData.Kind} [{senderApplication}] {(string.IsNullOrEmpty(logData.Subject) ? string.Empty: logData.Subject + ": ")}{logData.Message}, {logData.ProcessId}:{logData.TheadId}, {logData.CallerMemberName}, {logData.CallerFilePath}({logData.CallerLineNumber})";
            if(!string.IsNullOrEmpty(logData.Detail)) {
                writeValue += Environment.NewLine;
                writeValue += string.Join(Environment.NewLine, TextUtility.ReadLines(logData.Detail).Select(s => ">\t" + s));
            }

            foreach(var data in Writers) {
                data.Writer.WriteLine(writeValue);
            }

            OnLogWrite(writeValue, utcTimestamp, senderApplication, logData);
        }

        NKitLogData CreateLogData(NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            return new NKitLogData() {
                Kind = logKind,
                Subject = subject,
                Message = message,
                Detail = detail,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber,
                ProcessId = Process.GetCurrentProcess().Id,
                TheadId = Thread.CurrentThread.ManagedThreadId,
            };
        }

        void WriteDetail(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logData = CreateLogData(logKind, subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
            Write(DateTime.UtcNow, senderApplication, logData);
        }


        void WriteMessage(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logData = CreateLogData(logKind, subject, message, null, callerMemberName, callerFilePath, callerLineNumber);
            Write(DateTime.UtcNow, senderApplication, logData);
        }

        void WriteException(NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logData = CreateLogData(logKind, subject, ex.Message, ex.ToString(), callerMemberName, callerFilePath, callerLineNumber);
            Write(DateTime.UtcNow, senderApplication, logData);
        }

        public void WriteTalkLog(TalkLoggingWriteEventArgs e)
        {
            Write(e.UtcTimestamp, e.SenderApplication, e.LogData);
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
