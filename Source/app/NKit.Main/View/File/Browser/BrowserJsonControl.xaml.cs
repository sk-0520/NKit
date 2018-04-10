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
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser.ViewWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserJsonControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserJsonControl : UserControl, IBrowserDetail
    {
        public BrowserJsonControl()
        {
            InitializeComponent();

            Detail = BrowserDetail.Create(this);
        }

        #region SelectedTabItem

        public TabItem SelectedTabItem
        {
            get { return (TabItem)GetValue(SelectedTabItemProperty); }
            set { SetValue(SelectedTabItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedTabItemProperty = DependencyProperty.Register(
             nameof(SelectedTabItem),
             typeof(TabItem),
             typeof(BrowserJsonControl),
             new PropertyMetadata(default(TabItem), new PropertyChangedCallback(SelectedTabItemChanged)));

        private static void SelectedTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserJsonControl).SelectedTabItem = (TabItem)e.NewValue;
        }

        #endregion

        #region property

        BrowserDetail<BrowserJsonControl> Detail { get; }

        #endregion

        #region function
        #endregion

        #region IBrowserDetail

        public bool CanBrowse(BrowserViewModel browser)
        {
            return browser.IsJson;
        }

        public void BuildControl(BrowserViewModel browser)
        {
            SelectedTabItem = this.treeView;

            using(var reader = new JsonTextReader(new StreamReader(browser.FileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read), browser.Encoding))) {
                var json = JObject.Load(reader);
                this.node.ItemsSource = json.Children().Select(c => new JsonNode(c));
            }
        }

        #endregion
    }
}
