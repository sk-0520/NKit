using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public abstract class TalkEventArgs : EventArgs
    {
        public TalkEventArgs(NKitApplicationKind senderApplication)
        {
            SenderApplication = senderApplication;
        }

        #region property

        public NKitApplicationKind SenderApplication { get; }

        #endregion
    }

    public abstract class NKitTalkerHostBase<TChannel> : DisposerBase
    {
        public NKitTalkerHostBase(Uri serviceUri, string address)
        {
            ServiceUri = serviceUri;
            Address = address;
        }

        #region property

        ServiceHost ServiceHost { get; set; }
        public bool IsOpend { get; private set; }

        protected Uri ServiceUri { get; }
        protected string Address { get; }

        #endregion

        #region function

        public void Open()
        {
            ServiceHost = new ServiceHost(this, ServiceUri);

            Binding mexBinding;
            Binding processLinkBinding;
            switch(ServiceUri.Scheme) {
                case "net.pipe":
                    mexBinding = MetadataExchangeBindings.CreateMexNamedPipeBinding();
                    processLinkBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    break;

                default:
                    throw new NotImplementedException();
            }

            var serviceMetadata = ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if(serviceMetadata == null) {
                serviceMetadata = new ServiceMetadataBehavior();
                ServiceHost.Description.Behaviors.Add(serviceMetadata);
            }

            ServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, mexBinding, Address + "/" + "mex");
            ServiceHost.AddServiceEndpoint(typeof(TChannel), processLinkBinding, Address);

            ServiceHost.Open();
            IsOpend = true;
        }

        public void Close()
        {
            ServiceHost.Close();
            IsOpend = false;
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(IsOpend) {
                        Close();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    public class TalkApplicationPreparateEventArgs : TalkEventArgs
    {
        public TalkApplicationPreparateEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        public NKitApplicationKind TargetApplication { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectoryPath { get; set; }

        /// <summary>
        /// 管理ID。
        /// </summary>
        public uint ManageId { get; set; }

        #endregion
    }

    public class TalkApplicationWakeupEventArgs : TalkEventArgs
    {
        public TalkApplicationWakeupEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        /// <summary>
        /// 管理ID。
        /// </summary>
        public uint ManageId { get; set; }

        public bool Success { get; set; }

        #endregion
    }

    public class TalkApplicationStatusEventArgs: TalkEventArgs
    {
        public TalkApplicationStatusEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        /// <summary>
        /// 管理ID。
        /// </summary>
        public uint ManageId { get; set; }

        public NKitApplicationStatus Status { get; set; } = new NKitApplicationStatus();

        #endregion
    }

    public class TalkApplicationShutdownEventArgs : TalkEventArgs
    {
        public TalkApplicationShutdownEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        /// <summary>
        /// 管理ID。
        /// </summary>
        public uint ManageId { get; set; }

        public bool Force { get; set; }

        public bool Success { get; set; }

        #endregion
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NKitApplicationTalkerHost : NKitTalkerHostBase<INKitApplicationTalker>, INKitApplicationTalker
    {
        #region event
        // イベント対応はほんとミスった

        public event EventHandler<TalkApplicationPreparateEventArgs> ApplicationPreparate;
        public event EventHandler<TalkApplicationWakeupEventArgs> ApplicationWakeup;
        public event EventHandler<TalkApplicationStatusEventArgs> ApplicationStatus;
        public event EventHandler<TalkApplicationShutdownEventArgs> ApplicationShutdown;
        

        #endregion

        public NKitApplicationTalkerHost(Uri serviceUri, string address)
            : base(serviceUri, address)
        { }

        #region function


        uint OnPreparateApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            if(ApplicationPreparate != null) {
                var e = new TalkApplicationPreparateEventArgs(sender) {
                    TargetApplication = target,
                    Arguments = arguments,
                    WorkingDirectoryPath = workingDirectoryPath,
                };
                ApplicationPreparate(this, e);

                return e.ManageId;
            }

            return 0;
        }

        bool OnWakeupApplication(NKitApplicationKind sender, uint manageId)
        {
            if(ApplicationWakeup != null) {
                var e = new TalkApplicationWakeupEventArgs(sender) {
                    ManageId = manageId,
                };

                ApplicationWakeup(this, e);

                return e.Success;
            }

            return false;
        }

        private NKitApplicationStatus OnGetStatus(NKitApplicationKind sender, uint manageId)
        {
            if(ApplicationStatus != null) {
                var e = new TalkApplicationStatusEventArgs(sender) {
                    ManageId = manageId,
                };

                ApplicationStatus(this, e);

                return e.Status;
            }

            return new NKitApplicationStatus() {
                IsEnabled = false,
            };
        }

        bool OnShutdown(NKitApplicationKind sender, uint manageId, bool force)
        {
            if(ApplicationShutdown != null) {
                var e = new TalkApplicationShutdownEventArgs(sender) {
                    ManageId = manageId,
                    Force = force,
                };
                ApplicationShutdown(this, e);

                return e.Success;
            }

            return false;
        }

        #endregion

        #region INKitApplicationTalker

        public uint PreparateApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            return OnPreparateApplication(sender, target, arguments, workingDirectoryPath);
        }

        public bool WakeupApplication(NKitApplicationKind sender, uint manageId)
        {
            return OnWakeupApplication(sender, manageId);
        }

        public NKitApplicationStatus GetStatus(NKitApplicationKind sender, uint manageId)
        {
            return OnGetStatus(sender, manageId);
        }

        public bool Shutdown(NKitApplicationKind sender, uint manageId, bool force)
        {
            return OnShutdown(sender, manageId, force);
        }

        #endregion
    }

    public class TalkLoggingWriteEventArgs : TalkEventArgs
    {
        public TalkLoggingWriteEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        public DateTime UtcTimestamp { get; set; }

        public NKitLogKind LogKind { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public int ProcessId { get; set; }
        public int TheadId { get; set; }
        public string CallerMemberName { get; set; }
        public string CallerFilePath { get; set; }
        public int CallerLineNumber { get; set; }

        #endregion
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NKitLoggingTalkerHost : NKitTalkerHostBase<INKitLoggingTalker>, INKitLoggingTalker
    {
        #region event

        public event EventHandler<TalkLoggingWriteEventArgs> LoggingWrite;

        #endregion

        public NKitLoggingTalkerHost(Uri serviceUri, string address)
            : base(serviceUri, address)
        { }

        #region function

        private void OnWrite(DateTime utcTimestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            if(LoggingWrite != null) {
                var e = new TalkLoggingWriteEventArgs(senderApplication) {
                    UtcTimestamp = utcTimestamp,
                    LogKind = logKind,
                    Subject = subject,
                    Message = message,
                    Detail = detail,
                    ProcessId = processId,
                    TheadId = threadId,
                    CallerMemberName = callerMemberName,
                    CallerFilePath = callerFilePath,
                    CallerLineNumber = callerLineNumber,
                };
                LoggingWrite(this, e);
            }
        }

        #endregion

        #region INKitLoggingTalker

        public void Write(DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            OnWrite(timestamp, senderApplication, logKind, subject, message, detail, processId, threadId, callerMemberName, callerFilePath, callerLineNumber);
        }

        #endregion
    }


}
