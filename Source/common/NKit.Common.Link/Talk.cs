using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public enum NKitApplicationKind
    {
        Main,
        Rocket,
    }

    public interface INKitTalkMessage
    {
        #region property

        NKitApplicationKind SenderApplication { get; }

        #endregion
    }

    public interface INKitApplicationTalkWakeupMessage : INKitTalkMessage
    {
        #region property

        NKitApplicationKind TargetApplication { get; }
        string Arguments { get; }
        string WorkingDirectoryPath { get; }

        #endregion
    }

    [ServiceContract]
    public interface INKitApplicationTalker
    {
        #region function

        [OperationContract]
        void WakeupApplication(INKitApplicationTalkWakeupMessage message);

        #endregion
    }

}
