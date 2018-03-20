using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContentTypeTextNet.NKit.Main.View.Capture
{
    /// <summary>
    /// InternetExplorerScrollHideCaptureControl.xaml の相互作用ロジック
    /// </summary>
    public partial class InternetExplorerScrollHideCaptureControl : UserControl
    {
        public InternetExplorerScrollHideCaptureControl()
        {
            InitializeComponent();
        }

        #region HideTitle

        public string HideTitle
        {
            get { return (string)GetValue(HideTitleProperty); }
            set { SetValue(HideTitleProperty, value); }
        }

        public static readonly DependencyProperty HideTitleProperty = DependencyProperty.Register(
             nameof(HideTitle),
             typeof(string),
             typeof(InternetExplorerScrollHideCaptureControl),
             new PropertyMetadata(default(string), new PropertyChangedCallback(HideTitleChanged)));

        private static void HideTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InternetExplorerScrollHideCaptureControl).HideTitle = (string)e.NewValue;
        }


        #endregion

        #region IsEnabledHide

        public bool IsEnabledHide
        {
            get { return (bool)GetValue(IsEnabledHideProperty); }
            set { SetValue(IsEnabledHideProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledHideProperty = DependencyProperty.Register(
             nameof(IsEnabledHide),
             typeof(bool),
             typeof(InternetExplorerScrollHideCaptureControl),
             new PropertyMetadata(default(bool), new PropertyChangedCallback(IsEnabledHideChanged)));

        private static void IsEnabledHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InternetExplorerScrollHideCaptureControl).IsEnabledHide = (bool)e.NewValue;
        }


        #endregion

        #region HideElement

        public string HideElement
        {
            get { return (string)GetValue(HideElementProperty); }
            set { SetValue(HideElementProperty, value); }
        }

        public static readonly DependencyProperty HideElementProperty = DependencyProperty.Register(
             nameof(HideElement),
             typeof(string),
             typeof(InternetExplorerScrollHideCaptureControl),
             new PropertyMetadata(default(string), new PropertyChangedCallback(HideElementChanged)));

        private static void HideElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InternetExplorerScrollHideCaptureControl).HideElement = (string)e.NewValue;
        }


        #endregion

    }
}
