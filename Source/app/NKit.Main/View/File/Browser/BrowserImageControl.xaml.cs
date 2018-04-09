using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserImageControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserImageControl : UserControl
    {
        public BrowserImageControl()
        {
            InitializeComponent();
        }

        #region property

        Point ScrollMousePoint { get; set; }
        Point Offset { get; set; } = new Point(1, 1);

        #endregion

        #region command

        public ICommand ResetCommand => new DelegateCommand(() => {
            this.zoom.Value = 1;
        });

        #endregion

        #region function

        ImageSource GetImageSource(FileInfo fileInfo)
        {
            using(var stream = fileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)) {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
                var bitmap = new WriteableBitmap(decoder.Frames[0]);
                bitmap.Freeze();
                return bitmap;
            }
        }

        void SetBrowser(BrowserViewModel browser)
        {
            this.image.Source = GetImageSource(browser.FileInfo);
        }

        #endregion

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;
            if(browser != null && browser.IsImage) {
                SetBrowser(browser);
            }
        }

        private void imageScroller_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!this.selectOrigin.IsChecked.GetValueOrDefault()) {
                return;
            }

            var element = (UIElement)e.MouseDevice.DirectlyOver;
            if(!UIUtility.IsEnabledEventArea(element, new[] { typeof(ScrollViewer) }, new[] { typeof(ScrollBar) })) {
                return;
            }

            ScrollMousePoint = e.GetPosition(this.imageScroller);
            Offset = new Point(
                this.imageScroller.HorizontalOffset,
                this.imageScroller.VerticalOffset
            );
            this.imageScroller.CaptureMouse();
        }

        private void imageScroller_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(this.imageScroller.IsMouseCaptured) {
                var current = e.GetPosition(this.imageScroller);
                this.imageScroller.ScrollToHorizontalOffset(Offset.X + (ScrollMousePoint.X - current.X));
                this.imageScroller.ScrollToVerticalOffset(Offset.Y + (ScrollMousePoint.Y - current.Y));
            }
        }

        private void imageScroller_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.imageScroller.ReleaseMouseCapture();
        }
    }
}
