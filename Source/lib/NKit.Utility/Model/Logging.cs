using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public delegate void WriteMessageDelegate(NKitLogKind logKind, string subject, string message, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteDetailDelegate(NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber);
    public delegate void WriteExceptionDelegate(NKitLogKind logKind, string subject, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber);

    public class SwitchLogger : DisposerBase, ILogger
    {
        public SwitchLogger(string subject, WriteMessageDelegate messageWriter, WriteDetailDelegate detailWriter, WriteExceptionDelegate exceptionWriter)
        {
            Subject = subject;
            MessageWriter = messageWriter;
            DetailWriter = detailWriter;
            ExceptionWriter = exceptionWriter;
        }

        #region property

        string Subject { get; }
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

        public void Trace(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Trace, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Trace(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Trace, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Trace(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Trace, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Debug(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Debug, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Debug(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Debug, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Debug(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Debug, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Information(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Information, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Information(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Information, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Information(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Information, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Warning(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Warning, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Warning(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Warning, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Warning(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Warning, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Error, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Error(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Error, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Error(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Error, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Fatal(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            MessageWriter(NKitLogKind.Fatal, Subject, message, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Fatal(string message, string detail, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            DetailWriter(NKitLogKind.Fatal, Subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        public void Fatal(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            ExceptionWriter(NKitLogKind.Fatal, Subject, ex, callerMemberName, callerFilePath, callerLineNumber);
        }

        #endregion
    }

    public class LogSwitcher: DisposerBase, ILogFactory
    {
        public LogSwitcher(NKitApplicationKind senderApplication, Uri serviceUri)
        {
            SenderApplication = senderApplication;
            if(serviceUri != null) {
                LoggingClient = new NKitLoggingTalkerClient(SenderApplication, serviceUri);
            }
        }

        #region property

        NKitApplicationKind SenderApplication { get; }
        NKitLoggingTalkerClient LoggingClient { get; }

        NKitTalkerSwicher Swicther { get; } = new NKitTalkerSwicher();
        public NKitTalkerSwicherBase SwictherSetting => Swicther;

        #endregion

        #region function

        public void Initialize()
        {
            if(LoggingClient != null) {
                LoggingClient.Open();
            }
        }

        void Write(NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Swicther.DoSwitch(
                LoggingClient,
                timestamp => {
                    LoggingClient.Write(logKind, subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
                },
                (timestamp, talkerException) => {
                    var writeValue = $"@ {timestamp} {SenderApplication} {logKind} {subject} {message} {detail}";
                    switch(logKind) {
                        case NKitLogKind.Trace:
                        case NKitLogKind.Debug:
                        case NKitLogKind.Information:
                            Trace.WriteLine(writeValue);
                            break;

                        default:
                            Trace.TraceError(writeValue);
                            break;
                    }

                    // WCF死んだ旨をログ出力
                    if(talkerException != null) {
                        Write(NKitLogKind.Error, nameof(LogSwitcher), talkerException.Message, talkerException.ToString(), $"{nameof(Write)}/{callerMemberName}", callerFilePath, callerLineNumber);
                    }
                }
            );
        }

        void WriteMessage(NKitLogKind logKind, string subject, string message, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(logKind, subject, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }
        void WriteDetail(NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(logKind, subject, message, detail, callerMemberName, callerFilePath, callerLineNumber);
        }
        void WriteException(NKitLogKind logKind, string subject, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            Write(logKind, subject, ex.Message, ex.ToString(), callerMemberName, callerFilePath, callerLineNumber);
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(LoggingClient != null) {
                        LoggingClient.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region ILogCreator

        public ILogger CreateLogger()
        {
            return CreateLogger(null);
        }

        public ILogger CreateLogger(string subject)
        {
            return new SwitchLogger(subject, WriteMessage, WriteDetail, WriteException);
        }

        #endregion
    }

    public static class Log
    {
        static Log()
        {
            var factory = new LogSwitcher(NKitApplicationKind.Others, null);
            factory.SwictherSetting.LastErrorTimestamp = DateTime.MaxValue;
            factory.SwictherSetting.RetrySpan = TimeSpan.Zero;

            LogFactory = factory;
            Out = LogFactory.CreateLogger("{OUT}");
        }

        #region property

#if DEBUG
        static bool IsInitialized { get; set; } = false;
#endif
        static ILogFactory LogFactory { get; set; }

        public static ILogger Out { get; set; }

        public static ILogger CreateLogger()
        {
            return LogFactory.CreateLogger();
        }

        public static ILogger CreateLogger(string subject)
        {
            return LogFactory.CreateLogger(subject);
        }

        public static ILogger CreateLogger(object obj)
        {
            return LogFactory.CreateLogger(obj.GetType().Name);
        }

        #endregion

        #region function

        public static void Initialize(ILogFactory logFactory)
        {
#if DEBUG
            if(IsInitialized) {
                throw new InvalidOperationException();
            }
#endif

            LogFactory = logFactory;
            Out = LogFactory.CreateLogger("<OUT>");

#if DEBUG
            IsInitialized = true;
#endif
        }


        #endregion
    }

}
