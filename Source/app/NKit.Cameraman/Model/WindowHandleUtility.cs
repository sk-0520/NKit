using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public static class WindowHandleUtility
    {
        #region define

        const int WindowClassMaxSize = 256;

        #endregion

        #region function

        public static IntPtr GetView(Point point, CaptureTarget captureTarget)
        {
            var podPoint = new POINT(point.X, point.Y);
            var hWnd = NativeMethods.WindowFromPoint(podPoint);

            if(captureTarget == CaptureTarget.Control) {
                return hWnd;
            }

            // 親取得
            return NativeMethods.GetAncestor(hWnd, GA.GA_ROOT);
        }

        public static Rectangle GetViewArea(IntPtr hWnd, CaptureTarget captureTarget)
        {
            RECT windowRect;

            NativeMethods.GetWindowRect(hWnd, out windowRect);

            if(captureTarget == CaptureTarget.Client) {
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

        public static IntPtr GetActiveWindow(CaptureTarget captureTarget)
        {
            if(captureTarget == CaptureTarget.Screen) {
                throw new ArgumentException(nameof(captureTarget));
            }

            var hWnd = NativeMethods.GetForegroundWindow();
            if(captureTarget != CaptureTarget.Control) {
                return hWnd;
            }

            if(hWnd == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            // なんとかしてフォーカスウィンドウまで飛んでいく
            int targetProcessId;
            var targetThreadId = NativeMethods.GetWindowThreadProcessId(hWnd, out targetProcessId);
            var myThreadId = NativeMethods.GetCurrentThreadId();
            if(NativeMethods.AttachThreadInput(myThreadId, targetThreadId, true)) {
                try {
                    return NativeMethods.GetFocus();
                } finally {
                    NativeMethods.AttachThreadInput(myThreadId, targetThreadId, false);
                }
            }

            return IntPtr.Zero;
        }

        public static string GetWindowClassName(IntPtr hWnd)
        {
            var windowClassNameBuffer = new StringBuilder(WindowClassMaxSize);
            var windowClassResult = NativeMethods.GetClassName(hWnd, windowClassNameBuffer, windowClassNameBuffer.Capacity);
            if(windowClassResult == 0) {
                return string.Empty;
            }

            return windowClassNameBuffer.ToString();
        }

        #endregion
    }
}
