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
    public abstract class NKitTalkerClientBase : DisposerBase
    {
        public NKitTalkerClientBase(NKitApplicationKind senderApplication, Uri serviceUri, string address)
        {
            SenderApplication = senderApplication;
            ServiceUri = serviceUri;
            Address = address;
        }

        #region property

        protected NKitApplicationKind SenderApplication { get; }

        public Uri ServiceUri { get; }
        public string Address { get; }

        public abstract bool IsOpend {get;}

        #endregion

    }

    public abstract class NKitTalkerClientBase<TChannel> : NKitTalkerClientBase
    {
        public NKitTalkerClientBase(NKitApplicationKind senderApplication, Uri serviceUri, string address)
            :base(senderApplication, serviceUri, address)
        { }

        #region property

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

        #region NKitTalkerClientBase

        public override bool IsOpend
        {
            get
            {
                if(Channel == null) {
                    return false;
                }
                return Channel.State == CommunicationState.Opened;
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
        public NKitApplicationTalkerClient(NKitApplicationKind senderApplication, Uri serviceUri)
            : base(senderApplication, serviceUri, CommonUtility.AppAddress)
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

    public class NKitLoggingTalkerClient : NKitTalkerClientBase<INKitLoggingTalker>
    {
        public NKitLoggingTalkerClient(NKitApplicationKind senderApplication, Uri serviceUri)
            : base(senderApplication, serviceUri, CommonUtility.LogAddress)
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

    public delegate void TalkerSwicthDelegate(DateTime timestamp);
    public delegate void LocalSwicthDelegate(DateTime timestamp, Exception takerException);

    public class NKitTalkerSwicherBase
    {
        #region property

        public DateTime LastErrorTimestamp { get; set; } = DateTime.MinValue;
        public TimeSpan RetrySpan { get; set; } = TimeSpan.FromMinutes(10);

        #endregion
    }

    public class NKitTalkerSwicher: NKitTalkerSwicherBase
    {
        #region function

        public void DoSwitch(NKitTalkerClientBase client, TalkerSwicthDelegate talker, LocalSwicthDelegate local)
        {
            var timestamp = DateTime.Now;
            Exception talkerException = null;

            if(client != null) {
                if(LastErrorTimestamp + RetrySpan < timestamp) {
                    try {
                        talker(timestamp);
                        return;
                    } catch(CommunicationException ex) {
                        talkerException = ex;
                    }
                    LastErrorTimestamp = timestamp;
                }
            }

            // WCFが死んだか、単体で動いている場合
            if(client == null || talkerException != null) {
                local(timestamp, talkerException);
            }
        }

        #endregion
    }
}
