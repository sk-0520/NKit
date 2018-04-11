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
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper;
using ContentTypeTextNet.NKit.Utility.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentTypeTextNet.NKit.Browser.View
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
            SelectedTabItem = this.treeTabItem;

            lock(browser) {
                using(var reader = new JsonTextReader(new StreamReader(browser.GetSharedStream(), browser.Encoding))) {
                    var json = JObject.Load(reader);
                    this.treeView.ItemsSource = json.Children().Select(c => new JsonNode(c));
                }
            }
        }

        #endregion

        private void treeView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.treeHeader.ItemsSource = HeaderNodeUtility.GetHeaderNodes<JsonNode>(this.treeView);
        }
    }
}
