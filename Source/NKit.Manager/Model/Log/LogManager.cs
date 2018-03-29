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

        string TrimFilePath(string filePath)
        {
#if DEBUG
            var debug = true;
            if(debug) {
                return filePath;
            }
#endif
            var head = @"\Source\";
            var index = filePath.LastIndexOf(head, StringComparison.InvariantCultureIgnoreCase);
            return filePath.Substring(index);
        }

        void Write(DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogData logData)
        {
            var logTimestamp = CommonUtility.ReplaceNKitText(Constants.LogTimestampFormat, utcTimestamp);

            // YYYY-MM-DDThh:mm:ss XXXXXXXX YYYYYYYY ZZZZZZZZ mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm, PPPPPPP/TTTTTT MMMMMMMMMMMMM, SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS(NNNN)
            // くらいは確保しといてあげようじゃないか
            var writeBuffer = new StringBuilder(160);

            // タイムスタンプ
            writeBuffer.Append(logTimestamp);
            writeBuffer.Append(' ');

            // 種別
            writeBuffer.Append(logData.Kind);

            // 呼び出しプログラム
            writeBuffer.Append(" [");
            writeBuffer.Append(senderApplication);
            writeBuffer.Append("] ");

            // 件名
            if(!string.IsNullOrEmpty(logData.Subject)) {
                writeBuffer.Append(logData.Subject);
                writeBuffer.Append(": ");
            }

            // メッセージ
            writeBuffer.Append(logData.Message);
            writeBuffer.Append(", ");

            // プロセス ID とスレッド ID
            writeBuffer.Append(logData.ProcessId);
            writeBuffer.Append(':');
            writeBuffer.Append(logData.TheadId);
            writeBuffer.Append(' ');

            // 呼び出し元情報
            writeBuffer.Append(logData.CallerMemberName);
            writeBuffer.Append(", ");
            writeBuffer.Append(TrimFilePath(logData.CallerFilePath));
            writeBuffer.Append('(');
            writeBuffer.Append(logData.CallerLineNumber);
            writeBuffer.Append(')');

            // 詳細情報
            if(!string.IsNullOrEmpty(logData.Detail)) {
                writeBuffer.AppendLine();
                foreach(var s in TextUtility.ReadLines(logData.Detail)) {
                    writeBuffer.Append(">\t");
                    writeBuffer.Append(s);
                    writeBuffer.AppendLine();
                }
            }

            var writeValue = writeBuffer.ToString();

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
