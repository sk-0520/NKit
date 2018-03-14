using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class ScrollCamera : WindowHandleCamera
    {
        #region PInvoke

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        #endregion

        #region define

        const int WindowClassMaxSize = 256;

        const string WindowClassInternetExplorer = "Internet Explorer_Server";


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
            var windowClassNameBuffer = new StringBuilder(WindowClassMaxSize);
            var windowClassResult = GetClassName(WindowHandle, windowClassNameBuffer, windowClassNameBuffer.Capacity);
            if(windowClassResult == 0) {
                Logger.Debug("window class name: empty");
                return ScrollWindowKind.Unknown;
            }

            var windowClassName = windowClassNameBuffer.ToString();
            Logger.Debug($"window class name: {windowClassName}");

            if(windowClassName == WindowClassInternetExplorer) {
                return ScrollWindowKind.InternetExplorer;
            }


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
