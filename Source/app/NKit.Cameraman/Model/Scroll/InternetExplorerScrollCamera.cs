using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll
{
    public class InternetExplorerScrollCamera: ScrollCameraBase
    {
        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan waitTime)
            :base(hWnd, waitTime)
        { }

        #region WindowHandleCamera

        protected override Image TaskShotCore()
        {
            return base.TaskShotCore();
        }

        #endregion
    }
}
