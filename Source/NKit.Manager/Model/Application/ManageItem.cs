using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model.Application
{
    public class ManageItem
    {
        public ManageItem(uint manageId, ApplicationItem applicationItem)
        {
            ManageId = manageId;
            ApplicationItem = applicationItem;
        }

        #region property

        public uint ManageId {get;}
        public ApplicationItem ApplicationItem { get; }

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
