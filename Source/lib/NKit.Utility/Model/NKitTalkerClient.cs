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

        #region function

        public abstract void Open();
        public abstract void Close();

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

        public override void Open()
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

        public override void Close()
        {
            using(Channel) {
                try {
                    Channel.Close();
                } catch(Exception ex) {
                    // ログ出力通そうと思ったけどなんかクッソおかしなことになりそうだったのでやめた
                    Console.Error.WriteLine(ex);
                    Channel.Abort();
                }
            }
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

    public class NKitApplicationTalkerClient : NKitTalkerClientBase<INKitApplicationTalker>
    {
        public NKitApplicationTalkerClient(NKitApplicationKind senderApplication, Uri serviceUri)
            : base(senderApplication, serviceUri, CommonUtility.AppAddress)
        { }

        #region property

        #endregion

        #region function

        public uint PreparateApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            return Host.PreparateApplication(SenderApplication, targetApplication, arguments, workingDirectoryPath);
        }

        public bool WakeupApplication(uint manageId)
        {
            return Host.WakeupApplication(SenderApplication, manageId);
        }

        public NKitApplicationStatus GetStatus(uint manageId)
        {
            return Host.GetStatus(SenderApplication, manageId);
        }

        public bool Shutdown(uint manageId, bool force)
        {
            return Host.Shutdown(SenderApplication, manageId, force);
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
            var utcTimestamp = DateTime.UtcNow;
            var threadid = Thread.CurrentThread.ManagedThreadId;
            var processId = Process.GetCurrentProcess().Id;

            Host.Write(utcTimestamp, SenderApplication, logKind, subject, message, detail, processId, threadid, callerMemberName, callerFilePath, callerLineNumber);
        }


        #endregion
    }

    public delegate void TalkerSwicthDelegate(DateTime timestamp);
    public delegate void LocalSwicthDelegate(DateTime timestamp, Exception takerException);

    public class NKitTalkerSwicherBase
    {
        #region property

        public DateTime LastErrorTimestamp { get; set; } = DateTime.MinValue;
        public TimeSpan RetrySpan { get; set; } = TimeSpan.FromMinutes(4);

        #endregion
    }

    public class NKitTalkerSwicher: NKitTalkerSwicherBase
    {
        #region function

        public void DoSwitch(NKitTalkerClientBase client, TalkerSwicthDelegate talker, LocalSwicthDelegate local)
        {
            var utcTimestamp = DateTime.UtcNow;

            Exception talkerException = null;
            var sentMessage = false;

            if(client != null) {
                if(LastErrorTimestamp + RetrySpan < utcTimestamp) {
                    try {
                        if(!client.IsOpend) {
                            client.Open();
                        }

                        talker(utcTimestamp);
                        sentMessage = true;
                        return;
                    } catch(CommunicationException ex) {
                        talkerException = ex;
                        client.Close();
                    }
                    LastErrorTimestamp = utcTimestamp;
                }
            }

            // WCFが死んだか、単体で動いている場合
            if(client == null || !sentMessage || talkerException != null) {
                local(utcTimestamp, talkerException);
            }
        }

        #endregion
    }
}
