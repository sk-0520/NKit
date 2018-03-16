using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class ScreenCamera : CameraBase
    {
        #region CameraBase

        protected override Image TakeShotCore()
        {
            var allScreens = Screen.AllScreens;

            var minLeft = allScreens.Min(s => s.Bounds.Left);
            var minTop = allScreens.Min(s => s.Bounds.Top);
            var maxRight = allScreens.Max(s => s.Bounds.Right);
            var maxBottom = allScreens.Max(s => s.Bounds.Bottom);

            // 原点が(0,0)の左上座標へ補正してあげる
            var addX = minLeft < 0 ? -minLeft : 0;
            var addY = minTop < 0 ? -minTop : 0;

            var width = maxRight + addX;
            var height = maxBottom + addY;

            var bitmap = new Bitmap(width, height);
            using(var g = Graphics.FromImage(bitmap)) {
                foreach(var screen in allScreens) {
                    var screenBounds = screen.Bounds;
                    var srcPoint = new Point(screenBounds.X, screenBounds.Y);
                    var dstPoint = new Point(screenBounds.X + addX, screenBounds.Y + addY);
                    var dstSize = new Size(screenBounds.Width, screenBounds.Height);
                    g.CopyFromScreen(srcPoint, dstPoint, dstSize);
                }
            }

            return bitmap;
        }

        #endregion
    }
}
