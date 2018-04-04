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

        #region Winuser
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
        #endregion

        #region Wingdi

        public enum DeviceCap: int
        {
            /// <summary>
            /// Device driver version
            /// </summary>
            DRIVERVERSION = 0,
            /// <summary>
            /// Device classification
            /// </summary>
            TECHNOLOGY = 2,
            /// <summary>
            /// Horizontal size in millimeters
            /// </summary>
            HORZSIZE = 4,
            /// <summary>
            /// Vertical size in millimeters
            /// </summary>
            VERTSIZE = 6,
            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,
            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,
            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            BITSPIXEL = 12,
            /// <summary>
            /// Number of planes
            /// </summary>
            PLANES = 14,
            /// <summary>
            /// Number of brushes the device has
            /// </summary>
            NUMBRUSHES = 16,
            /// <summary>
            /// Number of pens the device has
            /// </summary>
            NUMPENS = 18,
            /// <summary>
            /// Number of markers the device has
            /// </summary>
            NUMMARKERS = 20,
            /// <summary>
            /// Number of fonts the device has
            /// </summary>
            NUMFONTS = 22,
            /// <summary>
            /// Number of colors the device supports
            /// </summary>
            NUMCOLORS = 24,
            /// <summary>
            /// Size required for device descriptor
            /// </summary>
            PDEVICESIZE = 26,
            /// <summary>
            /// Curve capabilities
            /// </summary>
            CURVECAPS = 28,
            /// <summary>
            /// Line capabilities
            /// </summary>
            LINECAPS = 30,
            /// <summary>
            /// Polygonal capabilities
            /// </summary>
            POLYGONALCAPS = 32,
            /// <summary>
            /// Text capabilities
            /// </summary>
            TEXTCAPS = 34,
            /// <summary>
            /// Clipping capabilities
            /// </summary>
            CLIPCAPS = 36,
            /// <summary>
            /// Bitblt capabilities
            /// </summary>
            RASTERCAPS = 38,
            /// <summary>
            /// Length of the X leg
            /// </summary>
            ASPECTX = 40,
            /// <summary>
            /// Length of the Y leg
            /// </summary>
            ASPECTY = 42,
            /// <summary>
            /// Length of the hypotenuse
            /// </summary>
            ASPECTXY = 44,
            /// <summary>
            /// Shading and Blending caps
            /// </summary>
            SHADEBLENDCAPS = 45,

            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90,

            /// <summary>
            /// Number of entries in physical palette
            /// </summary>
            SIZEPALETTE = 104,
            /// <summary>
            /// Number of reserved entries in palette
            /// </summary>
            NUMRESERVED = 106,
            /// <summary>
            /// Actual color resolution
            /// </summary>
            COLORRES = 108,

            // Printing related DeviceCaps. These replace the appropriate Escapes
            /// <summary>
            /// Physical Width in device units
            /// </summary>
            PHYSICALWIDTH = 110,
            /// <summary>
            /// Physical Height in device units
            /// </summary>
            PHYSICALHEIGHT = 111,
            /// <summary>
            /// Physical Printable Area x margin
            /// </summary>
            PHYSICALOFFSETX = 112,
            /// <summary>
            /// Physical Printable Area y margin
            /// </summary>
            PHYSICALOFFSETY = 113,
            /// <summary>
            /// Scaling factor x
            /// </summary>
            SCALINGFACTORX = 114,
            /// <summary>
            /// Scaling factor y
            /// </summary>
            SCALINGFACTORY = 115,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116,
            /// <summary>
            /// Vertical height of entire desktop in pixels
            /// </summary>
            DESKTOPVERTRES = 117,
            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118,
            /// <summary>
            /// Preferred blt alignment
            /// </summary>
            BLTALIGNMENT = 119
        }

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCap nIndex);
        #endregion

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
