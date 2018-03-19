using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;

namespace ContentTypeTextNet.NKit.Utility.Model
{

    // ぜんぜん対話しない
    public abstract class ScrollNotificationBase: Notification
    {
        #region property
        #endregion
    }

    public class ScrollNotification<T>: ScrollNotificationBase
    {
        public ScrollNotification(T target)
        {
            Target = target;
        }

        #region property

        public T Target { get; }

        #endregion
    }
}
