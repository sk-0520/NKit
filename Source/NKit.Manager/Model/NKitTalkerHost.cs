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
    public abstract class TaskEventArgs : EventArgs
    {
        #region property

        public NKitApplicationKind SenderApplication { get; set; }

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

            ServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, mexBinding, "mex");
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

    public class TaskWakeupApplicationEventArgs : TaskEventArgs
    {
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

        public event EventHandler<TaskWakeupApplicationEventArgs> TalkWakeupApplication;

        #endregion

        public NKitApplicationTalkerHost(Uri serviceUri, string address)
            :base(serviceUri, address)
        { }

        #region function


        void OnWakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            if(TalkWakeupApplication != null) {
                var e = new TaskWakeupApplicationEventArgs() {
                    SenderApplication = sender,
                    TargetApplication = target,
                    Arguments = arguments,
                    WorkingDirectoryPath = workingDirectoryPath,
                };
                TalkWakeupApplication(this, e);
            }
        }

        #endregion

        #region INKitApplicationTasker

        public void WakeupApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath)
        {
            OnWakeupApplication(sender, target, arguments, workingDirectoryPath);
        }

        #endregion

        #region NKitTalkerHost


        #endregion

    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NKitLoggingTalkerHost: INKitLoggingTalker:  NKitTalkerHostBase<INKitLoggingTalker>, INKitLoggingTalker
    {
        public NKitLoggingTalkerHost(Uri serviceUri, string address)
            : base(serviceUri, address)
        { }
    }


}
