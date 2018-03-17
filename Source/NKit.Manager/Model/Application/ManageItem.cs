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
        public ManageItem(uint manageId, ApplicationItem applicationItem, IReadOnlyDictionary<string, string> nkitArguments)
        {
            ManageId = manageId;
            ApplicationItem = applicationItem;
            NKitArguments = nkitArguments;

            if(NKitArguments.TryGetValue(CommonUtility.ManagedStartup.AloneSuicideEventName, out var name)) {
                AloneSuicideEvent = new EventWaitHandle(false, EventResetMode.AutoReset, name);
            }
        }

        #region property

        public uint ManageId {get;}
        public ApplicationItem ApplicationItem { get; }
        public IReadOnlyDictionary<string, string> NKitArguments { get; }

        public EventWaitHandle AloneSuicideEvent { get; }

        public string AloneSuicideEventName
        {
            get
            {
                if(NKitArguments.TryGetValue(CommonUtility.ManagedStartup.AloneSuicideEventName, out var name)) {
                    return name;
                }
                return string.Empty;
            }
        }

        #endregion

        #region function

        /// <summary>
        /// この管理アプリケーションが死んだ。
        /// </summary>
        public void Exited()
        {
        }

        #endregion
    }
}
