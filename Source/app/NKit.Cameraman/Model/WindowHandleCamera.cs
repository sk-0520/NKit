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
        public WindowHandleCamera(IntPtr hWnd, CaptureMode captureMode)
        {
            WindowHandle = hWnd;
            CaptureMode = captureMode;
        }

        #region property

        IntPtr WindowHandle { get; }
        CaptureMode CaptureMode { get; }

        #endregion

        #region CameraBase

        protected override Image TaskShotCore()
        {
            var area = WindowHandleUtility.GetViewArea(WindowHandle, CaptureMode);

            var bitmap = new Bitmap(area.Width, area.Height);
            using(var g = Graphics.FromImage(bitmap)) {
                g.CopyFromScreen(area.Location, new Point(), area.Size);
            }

            return bitmap;
        }

        #endregion
    }
}
