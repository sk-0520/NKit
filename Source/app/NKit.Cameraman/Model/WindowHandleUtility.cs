using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public static class WindowHandleUtility
    {
        #region function

        public static IntPtr GetView(Point point, CaptureMode captureMode)
        {
            var podPoint = new POINT(point.X, point.Y);
            var hWnd = NativeMethods.WindowFromPoint(podPoint);

            if(captureMode == CaptureMode.TargetControl) {
                return hWnd;
            }

            // 親取得
            return NativeMethods.GetAncestor(hWnd, GA.GA_ROOT);
        }

        public static Rectangle GetViewArea(IntPtr hWnd, CaptureMode captureMode)
        {
            RECT windowRect;

            NativeMethods.GetWindowRect(hWnd, out windowRect);

            if(captureMode == CaptureMode.TargetClient) {
                RECT clientRect;
                NativeMethods.GetClientRect(hWnd, out clientRect);
                var top = new POINT(clientRect.Left, clientRect.Top);
                var bottom = new POINT(clientRect.Right, clientRect.Bottom);
                NativeMethods.ClientToScreen(hWnd, ref top);
                NativeMethods.ClientToScreen(hWnd, ref bottom);

                windowRect.Left = top.X;
                windowRect.Top = top.Y;
                windowRect.Right = bottom.X;
                windowRect.Bottom = bottom.Y;
            }
            var result = new Rectangle(windowRect.Left, windowRect.Top, windowRect.Width, windowRect.Height);

            return result;
        }

        public static IntPtr GetActiveWindow(CaptureMode captureMode)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
