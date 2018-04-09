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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserImageControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserImageControl : UserControl, IBrowserDetail
    {
        public BrowserImageControl()
        {
            InitializeComponent();

            Detail = BrowserDetail.Create(this);
        }

        #region IsAnimation


        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }

        public static readonly DependencyProperty IsAnimationProperty = DependencyProperty.Register(
             nameof(IsAnimation),
             typeof(bool),
             typeof(BrowserImageControl),
             new PropertyMetadata(default(bool), new PropertyChangedCallback(IsAnimationChanged)));

        private static void IsAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).IsAnimation = (bool)e.NewValue;
        }

        #endregion

        #region IsReplay


        public bool IsReplay
        {
            get { return (bool)GetValue(IsReplayProperty); }
            set { SetValue(IsReplayProperty, value); }
        }

        public static readonly DependencyProperty IsReplayProperty = DependencyProperty.Register(
             nameof(IsReplay),
             typeof(bool),
             typeof(BrowserImageControl),
             new PropertyMetadata(true, new PropertyChangedCallback(IsReplayChanged)));

        private static void IsReplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).IsReplay = (bool)e.NewValue;
        }

        #endregion

        #region Scale


        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
             nameof(Scale),
             typeof(double),
             typeof(BrowserImageControl),
             new PropertyMetadata(1.0, new PropertyChangedCallback(ScaleChanged)));

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).Scale = (double)e.NewValue;
        }

        #endregion

        #region AnimationWidth


        public double AnimationWidth
        {
            get { return (double)GetValue(AnimationWidthProperty); }
            set { SetValue(AnimationWidthProperty, value); }
        }

        public static readonly DependencyProperty AnimationWidthProperty = DependencyProperty.Register(
             nameof(AnimationWidth),
             typeof(double),
             typeof(BrowserImageControl),
             new PropertyMetadata(default(double), new PropertyChangedCallback(AnimationWidthChanged)));

        private static void AnimationWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).AnimationWidth = (double)e.NewValue;
        }

        #endregion

        #region AnimationHeight


        public double AnimationHeight
        {
            get { return (double)GetValue(AnimationHeightProperty); }
            set { SetValue(AnimationHeightProperty, value); }
        }

        public static readonly DependencyProperty AnimationHeightProperty = DependencyProperty.Register(
             nameof(AnimationHeight),
             typeof(double),
             typeof(BrowserImageControl),
             new PropertyMetadata(default(double), new PropertyChangedCallback(AnimationHeightChanged)));

        private static void AnimationHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).AnimationHeight = (double)e.NewValue;
        }

        #endregion

        #region MinimumScale

        public double MinimumScale
        {
            get { return (double)GetValue(MinimumScaleProperty); }
            set { SetValue(MinimumScaleProperty, value); }
        }

        public static readonly DependencyProperty MinimumScaleProperty = DependencyProperty.Register(
             nameof(MinimumScale),
             typeof(double),
             typeof(BrowserImageControl),
             new PropertyMetadata(0.05, new PropertyChangedCallback(MinimumScaleChanged)));

        private static void MinimumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).MinimumScale = (double)e.NewValue;
        }

        #endregion

        #region MaximumScale

        public double MaximumScale
        {
            get { return (double)GetValue(MaximumScaleProperty); }
            set { SetValue(MaximumScaleProperty, value); }
        }

        public static readonly DependencyProperty MaximumScaleProperty = DependencyProperty.Register(
             nameof(MaximumScale),
             typeof(double),
             typeof(BrowserImageControl),
             new PropertyMetadata(8.0, new PropertyChangedCallback(MaximumScaleChanged)));

        private static void MaximumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserImageControl).MaximumScale = (double)e.NewValue;
        }

        #endregion

        #region property

        BrowserDetail<BrowserImageControl> Detail { get; }

        Point ScrollMousePoint { get; set; }
        Point Offset { get; set; } = new Point(1, 1);

        #endregion

        #region command

        public ICommand ResetCommand => new DelegateCommand(() => {
            Scale = 1;
        });

        public DelegateCommand PlayAnimationCommand => new DelegateCommand(
            () => {
                this.player.Stop();
                this.player.Position = TimeSpan.FromMilliseconds(1);
                this.player.Play();
            }
        );

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

        bool IsAnimationImage(BrowserKind browserKind, FileInfo fileInfo)
        {
            if(browserKind != BrowserKind.Gif) {
                return false;
            }

            using(var stream = fileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)) {
                var gifDecoder = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return 1 < gifDecoder.Frames.Count;
            }
        }

        #endregion

        #region IBrowserDetail

        public bool CanBrowse(BrowserViewModel browser)
        {
            return browser.IsImage;
        }

        public void BuildControl(BrowserViewModel browser)
        {
            IsAnimation = IsAnimationImage(browser.BrowserKind, browser.FileInfo);
            if(IsAnimation) {
                this.image.Visibility = Visibility.Collapsed;
                this.player.Visibility = Visibility.Visible;
                this.player.Source = new Uri(browser.FileInfo.FullName);
                this.player.Play();
            } else {
                this.player.Stop();
                this.player.Source = null;

                this.player.Visibility = Visibility.Collapsed;
                this.image.Visibility = Visibility.Visible;
                this.image.Source = GetImageSource(browser.FileInfo);
            }
        }

        #endregion

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

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if(IsReplay) {
                this.player.Position = TimeSpan.FromMilliseconds(1);
            }
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            AnimationWidth = this.player.NaturalVideoWidth;
            AnimationHeight = this.player.NaturalVideoHeight;
        }
    }
}
