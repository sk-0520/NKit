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
    { }

    public abstract class NKitTalkerHostBase : DisposerBase
    {
        #region property

        ServiceHost ServiceHost { get; set; }
        public bool IsOpend { get; private set; }

        protected Uri ServiceUri { get; set; }
        protected string Address { get; set; }
        protected Type ImplementedContract { get; set; }

        #endregion

        #region function

        public void Open()
        {
            ServiceHost = new ServiceHost(GetType(), ServiceUri);

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
            ServiceHost.AddServiceEndpoint(ImplementedContract, processLinkBinding, Address);

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

        public INKitApplicationTalkWakeupMessage Message { get; set; }

        #endregion
    }

    public class NKitApplicationTalkerHost : NKitTalkerHostBase, INKitApplicationTalker
    {
        #region event

        public event EventHandler<TaskWakeupApplicationEventArgs> TaskWakeupApplication;

        #endregion

        public NKitApplicationTalkerHost()
        {
            ServiceUri = new Uri("net.pipe://localhost/cttn-nkit");
            Address = "app";
            ImplementedContract = typeof(INKitApplicationTalker);
        }

        #region function


        void OnWakeupApplication(INKitApplicationTalkWakeupMessage message)
        {
            if(TaskWakeupApplication != null) {
                var e = new TaskWakeupApplicationEventArgs() {
                    Message = message,
                };
                TaskWakeupApplication(this, e);
            }
        }

        #endregion

        #region INKitApplicationTasker

        public void WakeupApplication(INKitApplicationTalkWakeupMessage message)
        {
            OnWakeupApplication(message);
        }

        #endregion

        #region NKitTalkerHost


        #endregion

    }
}
