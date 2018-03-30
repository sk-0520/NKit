using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ContentTypeTextNet.NKit.Utility.Attribute;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.Library.PInvoke.Windows;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public static class UIUtility
    {
        /// <summary>
        /// <para>http://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if(depObj != null) {
                var childCount = VisualTreeHelper.GetChildrenCount(depObj);
                for(int i = 0; i < childCount; i++) {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if(child != null) {
                        var childObj = child as T;
                        if(childObj != null) {
                            yield return childObj;
                        }
                    }

                    foreach(var childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if(depObj != null) {
                foreach(var child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>()) {
                    if(child != null) {
                        var childObj = child as T;
                        if(childObj != null) {
                            yield return childObj;
                        }
                        foreach(var childOfChild in FindLogicalChildren<T>(child)) {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> FindChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            return FindLogicalChildren<T>(depObj).Concat(FindVisualChildren<T>(depObj));
        }

        /// <summary>
        /// 指定要素の表示要素から親要素を取得する。
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static DependencyObject GetVisualParent(DependencyObject depObj)
        {
            return VisualTreeHelper.GetParent(depObj);
        }

        /// <summary>
        /// 指定要素の表示要素から指定した祖先要素を取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static DependencyObject GetVisualClosest<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if(depObj == null) {
                return null;
            }

            var parent = GetVisualParent(depObj);
            var element = parent as T;
            if(element != null) {
                return element;
            } else {
                return GetVisualClosest<T>(parent);
            }
        }

        public static void RecursiveApplyTemplate(IEnumerable<FrameworkElement> elements)
        {
            foreach(var element in elements) {
                element.ApplyTemplate();
                RecursiveApplyTemplate(FindChildren<FrameworkElement>(element));
            }
        }

        /// <summary>
        /// http://grabacr.net/archives/1105
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public static Point GetDpiScale(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if(source != null && source.CompositionTarget != null) {
                return new Point(
                    source.CompositionTarget.TransformToDevice.M11,
                    source.CompositionTarget.TransformToDevice.M22
                );
            }

            return new Point(1.0, 1.0);
        }

        [return: PixelKind(Px.Device)]
        public static double ToDevicePixelFromX(Visual visual, double x)
        {
            var dpiScale = GetDpiScale(visual);

            return x * dpiScale.X;
        }

        [return: PixelKind(Px.Logical)]
        public static double ToLogicalPixelX(Visual visual, double x)
        {
            var dpiScale = GetDpiScale(visual);

            return x / dpiScale.X;
        }

        [return: PixelKind(Px.Device)]
        public static double ToDevicePixelFromY(Visual visual, double y)
        {
            var dpiScale = GetDpiScale(visual);

            return y * dpiScale.Y;
        }

        [return: PixelKind(Px.Logical)]
        public static double ToLogicalPixelY(Visual visual, double y)
        {
            var dpiScale = GetDpiScale(visual);

            return y / dpiScale.Y;
        }

        [return: PixelKind(Px.Device)]
        public static Point ToDevicePixel(Visual visual, Point point)
        {
            var dpiScale = GetDpiScale(visual);

            return new Point(
                point.X * dpiScale.X,
                point.Y * dpiScale.Y
            );
        }

        [return: PixelKind(Px.Logical)]
        public static Point ToLogicalPixel(Visual visual, Point point)
        {
            var dpiScale = GetDpiScale(visual);

            return new Point(
                point.X / dpiScale.X,
                point.Y / dpiScale.Y
            );
        }

        [return: PixelKind(Px.Device)]
        public static Size ToDevicePixel(Visual visual, Size size)
        {
            var dpiScale = GetDpiScale(visual);

            return new Size(
                size.Width * dpiScale.X,
                size.Height * dpiScale.Y
            );
        }

        [return: PixelKind(Px.Logical)]
        public static Size ToLogicalPixel(Visual visual, Size size)
        {
            var dpiScale = GetDpiScale(visual);

            return new Size(
                size.Width / dpiScale.X,
                size.Height / dpiScale.Y
            );
        }

        [return: PixelKind(Px.Device)]
        public static Rect ToDevicePixel(Visual visual, Rect rect)
        {
            var dpiScale = GetDpiScale(visual);

            return new Rect(
                rect.X * dpiScale.X,
                rect.Y * dpiScale.Y,
                rect.Width * dpiScale.X,
                rect.Height * dpiScale.Y
            );
        }

        [return: PixelKind(Px.Logical)]
        public static Rect ToLogicalPixel(Visual visual, Rect rect)
        {
            var dpiScale = GetDpiScale(visual);

            return new Rect(
                rect.X / dpiScale.X,
                rect.Y / dpiScale.Y,
                rect.Width / dpiScale.X,
                rect.Height / dpiScale.Y
            );
        }

        public static void SetStyleToolWindow(Window window, bool enabledMax, bool enabledMin)
        {
            var helper = new WindowInteropHelper(window);
            var hWnd = helper.Handle;

            int exStyle = (int)WindowsUtility.GetWindowLong(hWnd, (int)GWL.GWL_EXSTYLE);
            exStyle |= (int)WS_EX.WS_EX_TOOLWINDOW;
            WindowsUtility.SetWindowLong(hWnd, (int)GWL.GWL_EXSTYLE, (IntPtr)exStyle);

            if(!enabledMax || !enabledMin) {
                var style = (int)WindowsUtility.GetWindowLong(hWnd, (int)GWL.GWL_STYLE);
                WS ws = (WS)0;
                if(!enabledMax) {
                    ws |= WS.WS_MAXIMIZEBOX;
                }
                if(!enabledMin) {
                    ws |= WS.WS_MINIMIZEBOX;
                }
                style &= ~(int)ws;
                WindowsUtility.SetWindowLong(hWnd, (int)GWL.GWL_STYLE, (IntPtr)style);
            }
        }

        public static bool IsEnabledEventArea(DependencyObject dependencyObject, Type[] enableElementTypes)
        {
            if(enableElementTypes == null) {
                throw new ArgumentNullException(nameof(enableElementTypes));
            }

            // かなり TextSearchMatchControl に依存してる
            if(dependencyObject is System.Windows.Documents.Run) {
                return true;
            }

            while(dependencyObject != null) {
                var type = dependencyObject.GetType();
                if(enableElementTypes.Any(t => t == type)) {
                    return true;
                }
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return false;
        }
    }
}

