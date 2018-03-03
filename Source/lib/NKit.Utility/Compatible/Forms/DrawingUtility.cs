using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;

namespace ContentTypeTextNet.NKit.Utility.Compatible.Forms
{
    /// <summary>
    /// <see cref="System.Drawing"/>名前空間の互換処理群。
    /// <para>...WPFでも<see cref="System.Drawing"/>は選択的に非推奨じゃないのね……。</para>
    /// </summary>
    public static class DrawingUtility
    {
        public static Drawing.Size Convert(Size size)
        {
            return new Drawing.Size((int)size.Width, (int)size.Height);
        }
        public static Size Convert(Drawing.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        public static Drawing.Point Convert(Point point)
        {
            return new Drawing.Point((int)point.X, (int)point.Y);
        }
        public static Point Convert(Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

        public static Drawing.Rectangle Convert(Rect rect)
        {
            return new Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
        public static Rect Convert(Drawing.Rectangle rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static double ConvertFontSizeFromDrawing(double drawingFontPoint)
        {
            return drawingFontPoint * 96 / 72;
        }
        public static float ConvertFontSizeFromWpf(double wpfFontSize)
        {
            return (float)(wpfFontSize / 96.0 * 72.0);
        }

        public static ImageSource ImageSourceFromIcon(Drawing.Icon icon)
        {
            var result = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return result;
        }

        public static ImageSource ImageSourceFromBinaryIcon(byte[] binayIcon, Size iconSize)
        {
            using(var ms = new MemoryStream(binayIcon)) {
                var size = new Drawing.Size((int)iconSize.Width, (int)iconSize.Height);
                using(var icon = new Drawing.Icon(ms, size)) {
                    return ImageSourceFromIcon(icon);
                }
            }
        }

        public static Color Convert(Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
