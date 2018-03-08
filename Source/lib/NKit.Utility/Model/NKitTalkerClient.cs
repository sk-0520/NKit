using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
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
            var baseUri = ServiceUri.ToString();
            var targetUri = new Uri(baseUri + "/" + Address); // Path.Combine 的なことしたいんだけどなぁ
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
            try {
                Channel.Close();
            } catch(Exception ex) {
                Debug.WriteLine(ex);
                Channel.Abort();
            }
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

    public class NKitApplicationTalkerClient : NKitTalkerClientBase<INKitApplicationTalker>
    {
        public NKitApplicationTalkerClient(NKitApplicationKind senderApplication)
            : base(senderApplication)
        {
            ServiceUri = new Uri("net.pipe://localhost/cttn-nkit");
            Address = "app";
        }

        #region property

        NKitApplicationKind SenderApplication { get; }

        #endregion

        #region function

        public void WakeupApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            Host.WakeupApplication(SenderApplication, targetApplication, arguments, workingDirectoryPath);
        }

        #endregion
    }
}
