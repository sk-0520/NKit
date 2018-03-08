using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public abstract class NKitTalkerClientBase<TChannel> : DisposerBase
    {
        public NKitTalkerClientBase(NKitApplicationKind senderApplication)
        {
            SenderApplication = senderApplication;
        }

        #region property

        public bool IsOpend { get; private set; }
        NKitApplicationKind SenderApplication { get; }

        protected Uri ServiceUri { get; set; }
        protected string Address { get; set; }

        ChannelFactory<INKitApplicationTalker> Channel { get; set; }
        protected INKitApplicationTalker Host { get; private set; }

        #endregion

        #region function

        public void Open()
        {
            var targetUri = new Uri(ServiceUri, Address);
            var endpointAddr = new EndpointAddress(targetUri);
            Binding binding;
            switch(targetUri.Scheme) {
                case "net.pipe":
                    binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    break;

                default:
                    throw new NotImplementedException();
            }

            Channel = new ChannelFactory<INKitApplicationTalker>(binding, endpointAddr);
            Host = Channel.CreateChannel();
            IsOpend = true;
        }

        public void Close()
        {
            Channel.Close();
            IsOpend = false;
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(IsOpend) {
                        Channel.Close();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    public class NKitApplicationTaskWakeupMessage : INKitApplicationTalkWakeupMessage
    {
        public NKitApplicationTaskWakeupMessage(NKitApplicationKind senderApplication)
        {
            SenderApplication = senderApplication;
        }

        #region INKitApplicationTaskWakeupMessage

        public NKitApplicationKind SenderApplication { get; }

        public NKitApplicationKind TargetApplication { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectoryPath { get; set; }

        #endregion
    }

    public class NKitApplicationTalkerClient : NKitTalkerClientBase<INKitApplicationTalker>
    {
        public NKitApplicationTalkerClient(NKitApplicationKind senderApplication)
            : base(senderApplication)
        { }

        #region property

        NKitApplicationKind SenderApplication { get; }

        #endregion

        #region function

        public void WakeupApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            var message = new NKitApplicationTaskWakeupMessage(SenderApplication) {
                Arguments = arguments,
                WorkingDirectoryPath = workingDirectoryPath,
            };
            Host.WakeupApplication(message);
        }

        #endregion
    }
}
