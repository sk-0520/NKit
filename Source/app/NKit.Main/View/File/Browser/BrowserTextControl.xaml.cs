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
    public partial class BrowserTextControl : UserControl, IBrowserDetail
    {
        public BrowserTextControl()
        {
            InitializeComponent();

            Detail = BrowserDetail.Create(this);
        }

        #region property

        BrowserDetail<BrowserTextControl> Detail { get; }

        #endregion

        #region function
        #endregion

        #region IBrowserDetail

        public bool CanBrowse(BrowserViewModel browser)
        {
            return browser.CanBrowse(browser.BrowserKind);
        }

        public void BuildControl(BrowserViewModel browser)
        {
            //var reader = new StreamReader(, browser.Encoding);
            this.editor.Load(browser.FileInfo.OpenRead());
        }

        #endregion
    }
}
