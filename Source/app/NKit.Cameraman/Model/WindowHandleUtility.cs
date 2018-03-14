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
        #region PInvoke
        // PInvoke 待つの疲れた

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, NativeMethods.EnumWindowsProc lpEnumFunc, IntPtr lParam);

        #endregion

        #region define

        const int WindowClassMaxSize = 256;

        #endregion

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
            if(captureMode == CaptureMode.Screen) {
                throw new ArgumentException(nameof(captureMode));
            }

            var hWnd = NativeMethods.GetForegroundWindow();
            if(captureMode != CaptureMode.TargetControl) {
                return hWnd;
            }

            if(hWnd == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            // なんとかしてフォーカスウィンドウまで飛んでいく
            int targetProcessId;
            var targetThreadId = GetWindowThreadProcessId(hWnd, out targetProcessId);
            var myThreadId = GetCurrentThreadId();
            if(AttachThreadInput(myThreadId, targetThreadId, true)) {
                try {
                    return NativeMethods.GetFocus();
                } finally {
                    AttachThreadInput(myThreadId, targetThreadId, false);
                }
            }

            return IntPtr.Zero;
        }

        public static string GetWindowClassName(IntPtr hWnd)
        {
            var windowClassNameBuffer = new StringBuilder(WindowClassMaxSize);
            var windowClassResult = GetClassName(hWnd, windowClassNameBuffer, windowClassNameBuffer.Capacity);
            if(windowClassResult == 0) {
                return string.Empty;
            }

            return windowClassNameBuffer.ToString();
        }

        #endregion
    }
}
