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

    public class TalkApplicationWakeupEventArgs : TalkEventArgs
    {
        public TalkApplicationWakeupEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        public NKitApplicationKind TargetApplication { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectoryPath { get; set; }

        #endregion
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NKitApplicationTalkerHost : NKitTalkerHostBase<INKitApplicationTalker>, INKitApplicationTalker
    {
        #region event

        public event EventHandler<TalkApplicationWakeupEventArgs> ApplicationWakeup;

        #endregion

        public NKitApplicationTalkerHost(Uri serviceUri, string address)
            : base(serviceUri, address)
        { }

        #region function


        void OnWakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            if(ApplicationWakeup != null) {
                var e = new TalkApplicationWakeupEventArgs(sender) {
                    TargetApplication = target,
                    Arguments = arguments,
                    WorkingDirectoryPath = workingDirectoryPath,
                };
                ApplicationWakeup(this, e);
            }
        }

        #endregion

        #region INKitApplicationTalker

        public void WakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            OnWakeupApplication(sender, target, arguments, workingDirectoryPath);
        }

        #endregion
    }

    public class TalkLoggingWriteEventArgs : TalkEventArgs
    {
        public TalkLoggingWriteEventArgs(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        public DateTime Timestamp { get; set; }

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

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode =ConcurrencyMode.Multiple)]
    public class NKitLoggingTalkerHost : NKitTalkerHostBase<INKitLoggingTalker>, INKitLoggingTalker
    {
        #region event

        public event EventHandler<TalkLoggingWriteEventArgs> LoggingWrite;

        #endregion

        public NKitLoggingTalkerHost(Uri serviceUri, string address)
            : base(serviceUri, address)
        { }

        #region function

        private void OnWrite(DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int threadId, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            if(LoggingWrite != null) {
                var e = new TalkLoggingWriteEventArgs(senderApplication) {
                    Timestamp = timestamp,
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
