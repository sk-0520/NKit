using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ManageItem
    {
        public ManageItem(uint manageId, ApplicationItem applicationItem, string exitedEventName, IReadOnlyDictionary<string, string> nkitArguments)
        {
            ManageId = manageId;
            ApplicationItem = applicationItem;
            NKitArguments = nkitArguments;

            if(NKitArguments.TryGetValue(CommonUtility.ManagedStartup.AloneSuicideEventName, out var name)) {
                AloneSuicideEvent = new EventWaitHandle(false, EventResetMode.AutoReset, name);
            }

            ExitedEventName = exitedEventName;
            ExitedEvent = new EventWaitHandle(false, EventResetMode.ManualReset, ExitedEventName);
        }

        #region property

        public uint ManageId { get; }
        public ApplicationItem ApplicationItem { get; }
        public IReadOnlyDictionary<string, string> NKitArguments { get; }

        EventWaitHandle AloneSuicideEvent { get; }
        EventWaitHandle ExitedEvent { get; }

        public string ExitedEventName { get; }

        #endregion

        #region function

        /// <summary>
        /// この管理アプリケーションが死んだ。
        /// </summary>
        public void Exited()
        {
            ExitedEvent.Set();
        }

        #endregion
    }
}
