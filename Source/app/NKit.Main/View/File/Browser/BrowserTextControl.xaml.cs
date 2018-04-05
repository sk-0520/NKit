using System;
using System.Collections.Generic;
using System.IO;
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
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserTextControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserTextControl : UserControl
    {
        public BrowserTextControl()
        {
            InitializeComponent();
        }


        #region function

        void SetText(BrowserViewModel browser)
        {
            //var reader = new StreamReader(, browser.Encoding);
            this.editor.Load(browser.FileInfo.OpenRead());
        }

        #endregion

        #region UserControl
        #endregion

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;
            if(browser != null) {
                SetText(browser);
            }
        }
    }
}
