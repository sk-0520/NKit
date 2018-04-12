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
using ContentTypeTextNet.NKit.Browser.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.View
{
    /// <summary>
    /// BrowserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserControl : UserControl
    {
        public BrowserControl()
        {
            InitializeComponent();
        }

        #region BrowserProperty

        public static readonly DependencyProperty BrowserProperty = DependencyProperty.Register(
            nameof(Browser),
            typeof(BrowserViewModel),
            typeof(BrowserControl),
            new FrameworkPropertyMetadata(default(BrowserViewModel), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnBrowserChanged))
        );

        private static void OnBrowserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BrowserControl;
            if(control != null) {
                control.Browser = e.NewValue as BrowserViewModel;
            }
            if(e.OldValue is BrowserViewModel old) {
                old.Dispose();
            }
        }

        public BrowserViewModel Browser
        {
            get { return GetValue(BrowserProperty) as BrowserViewModel; }
            set { SetValue(BrowserProperty, value); }
        }

        #endregion

    }
}
