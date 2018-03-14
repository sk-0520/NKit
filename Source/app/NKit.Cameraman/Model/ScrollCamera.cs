using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class ScrollCamera : WindowHandleCamera
    {
        #region define

        enum ScrollWindowKind
        {
            Unknown,
            InternetExplorer,
        }

        #endregion

        public ScrollCamera(IntPtr hWnd)
            : base(hWnd, CaptureMode.TargetClient)
        {
        }

        #region function

        ScrollWindowKind GetScrollWindowKind()
        {
            return ScrollWindowKind.Unknown;
        }

        Image TaskShotInternetExplorer()
        {
            return null;
        }

        #endregion

        #region WindowHandleCamera

        protected override Image TaskShotCore()
        {
            // 自身がスクロール可能なウィンドウか調査
            var kind = GetScrollWindowKind();

            switch(kind) {
                case ScrollWindowKind.Unknown:
                    return base.TaskShotCore();

                case ScrollWindowKind.InternetExplorer:
                    return TaskShotInternetExplorer() ?? base.TaskShotCore();

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
