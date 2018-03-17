using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public enum NKitApplicationKind
    {
        /// <summary>
        /// よその子。
        /// </summary>
        Others,
        Manager,
        Main,
        Rocket,
        Cameraman,
    }

    [ServiceContract]
    public interface INKitApplicationTalker
    {
        #region function

        /// <summary>
        /// NKit プログラムを起動する準備を行う。
        /// </summary>
        /// <param name="sender">起動要求プログラム。</param>
        /// <param name="target">起動対象プログラム。</param>
        /// <param name="arguments">引数。</param>
        /// <param name="workingDirectoryPath">作業ディレクトリパス。</param>
        /// <returns>管理 ID。 0 は無効ID。</returns>
        [OperationContract]
        uint PreparateApplication(NKitApplicationKind sender, NKitApplicationKind target, string arguments, string workingDirectoryPath);

        /// <summary>
        /// <see cref="PreparateApplication"/>で準備したプログラムを起動する。
        /// <para>このメソッドが呼べるならマネージャ側でイベント等は構築済み。</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="manageId"></param>
        /// <returns>起動できたかどうか。</returns>
        [OperationContract]
        bool WakeupApplication(NKitApplicationKind sender, uint manageId);

        #endregion
    }

    public enum NKitLogKind
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }

    [ServiceContract]
    public interface INKitLoggingTalker
    {
        #region function

        [OperationContract]
        void Write(DateTime timestamp, NKitApplicationKind senderApplication, NKitLogKind logKind, string subject, string message, string detail, int processId, int theadId, string callerMemberName, string callerFileName, int callerLineNumber);

        #endregion
    }
}
