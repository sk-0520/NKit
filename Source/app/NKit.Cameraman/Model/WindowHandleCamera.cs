using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class WindowHandleCamera : CameraBase
    {
        public WindowHandleCamera(IntPtr hWnd, CaptureTarget captureTarget)
        {
            WindowHandle = hWnd;
            CaptureTarget = captureTarget;
        }

        #region property

        protected IntPtr WindowHandle { get; }
        CaptureTarget CaptureTarget { get; }

        #endregion

        #region CameraBase

        protected override Image TakeShotCore()
        {
            var area = WindowHandleUtility.GetViewArea(WindowHandle, CaptureTarget);

            var bitmap = new Bitmap(area.Width, area.Height);
            using(var g = Graphics.FromImage(bitmap)) {
                g.CopyFromScreen(area.Location, new Point(), area.Size);
            }

            return bitmap;
        }

        #endregion
    }
}
