using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.Library.PInvoke.Windows;

namespace ContentTypeTextNet.NKit.Utility.Compatible.Windows
{
    /// <summary>
    /// POD構造体用ユーティリティ。
    /// </summary>
    public static class PodStructUtility
    {
        public static RECT Convert(Rect rect)
        {
            var result = new RECT() {
                Left = (int)rect.Left,
                Top = (int)rect.Top,
                Width = (int)rect.Width,
                Height = (int)rect.Height,
            };

            return result;
        }

        public static Rect Convert(RECT rect)
        {
            var result = new Rect(
                rect.Left,
                rect.Top,
                rect.Width,
                rect.Height
            );

            return result;
        }

        public static SIZE Convert(Size size)
        {
            return new SIZE((int)size.Width, (int)size.Height);
        }

        public static Size Convert(SIZE size)
        {
            return new Size(size.cx, size.cy);
        }

        public static POINT Convert(Point point)
        {
            return new POINT((int)point.X, (int)point.Y);
        }

        public static Point Convert(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
