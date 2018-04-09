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
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserXmlHtmlControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserXmlHtmlControl : UserControl
    {
        public BrowserXmlHtmlControl()
        {
            InitializeComponent();
        }

        #region function

        void SetBrowser(BrowserViewModel browser)
        {
        }

        #endregion

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;
            if(browser != null && (browser.CanBrowse(BrowserKind.Xml) || browser.CanBrowse(BrowserKind.Html))) {
                SetBrowser(browser);
            }
        }
    }
}
