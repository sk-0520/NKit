using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public abstract class NKitTalkerClientBase<TChannel> : DisposerBase
    {
        public NKitTalkerClientBase(NKitApplicationKind senderApplication, Uri serviceUri, string address)
        {
            SenderApplication = senderApplication;
            ServiceUri = serviceUri;
            Address = address;
        }

        #region property

        public bool IsOpend
        {
            get
            {
                if(Channel == null) {
                    return false;
                }
                return Channel.State == CommunicationState.Opened;
            }
        }
        protected NKitApplicationKind SenderApplication { get; }

        public Uri ServiceUri { get; }
        public string Address { get; }

        ChannelFactory<TChannel> Channel { get; set; }
        protected TChannel Host { get; private set; }

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

            Channel = new ChannelFactory<TChannel>(binding, endpointAddr);
            Host = Channel.CreateChannel();
        }

        public void Close()
        {
            try {
                Channel.Close();
            } catch(Exception ex) {
                // ログ出力通そうと思ったけどなんかクッソおかしなことになりそうだったのでやめた
                Console.Error.WriteLine(ex);
                Channel.Abort();
            }
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
        public NKitApplicationTalkerClient(NKitApplicationKind senderApplication, Uri serviceUri, string address)
            : base(senderApplication, serviceUri, address)
        { }

        #region property

        #endregion

        #region function

        public void WakeupApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            Host.WakeupApplication(SenderApplication, targetApplication, arguments, workingDirectoryPath);
        }

        #endregion
    }

    public class NKitLoggingtalkerClient : NKitTalkerClientBase<INKitLoggingTalker>
    {
        public NKitLoggingtalkerClient(NKitApplicationKind senderApplication, Uri serviceUri, string address)
            : base(senderApplication, serviceUri, address)
        { }

        #region function

        public void Write(NKitLogKind logKind, string subject, string message, string detail, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var timestamp = DateTime.Now;
            var threadid = Thread.CurrentThread.ManagedThreadId;
            var processId = Process.GetCurrentProcess().Id;

            Host.Write(timestamp, SenderApplication, logKind, subject, message, detail, processId, threadid, callerMemberName, callerFilePath, callerLineNumber);
        }


        #endregion
    }
}
