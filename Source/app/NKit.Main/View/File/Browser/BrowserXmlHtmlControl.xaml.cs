using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using System.Xml.Linq;
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;
using ContentTypeTextNet.NKit.Utility.Model;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserXmlHtmlControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserXmlHtmlControl : UserControl, IBrowserDetail
    {
        public BrowserXmlHtmlControl()
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
             typeof(BrowserXmlHtmlControl),
             new PropertyMetadata(default(TabItem), new PropertyChangedCallback(SelectedTabItemChanged)));

        private static void SelectedTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BrowserXmlHtmlControl).SelectedTabItem = (TabItem)e.NewValue;
        }

        #endregion


        #region property

        BrowserDetail<BrowserXmlHtmlControl> Detail { get; }

        #endregion

        #region function

        IEnumerable<HtmlTreeNode> GetHtmlTreeNodes(BrowserViewModel browser)
        {
            var doc = new HtmlAgilityPack.HtmlDocument() {
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = browser.Encoding,
                OptionReadEncoding = true,
            };

            doc.Load(browser.FileInfo.OpenRead());

            return doc.DocumentNode.ChildNodes.Cast<HtmlNode>().Select(n => new HtmlTreeNode(n));
        }

        IEnumerable<XmlTreeNode> GetXmlTreeNodes(BrowserViewModel browser)
        {
            var doc = new XmlDocument();

            doc.Load(browser.FileInfo.OpenRead());

            return doc.ChildNodes.Cast<XmlNode>().Select(n => new XmlTreeNode(n));
        }

        #endregion

        #region IBrowserDetail

        public bool CanBrowse(BrowserViewModel browser)
        {
            return browser.CanBrowse(BrowserKind.Xml) || browser.CanBrowse(BrowserKind.Html);
        }

        public void BuildControl(BrowserViewModel browser)
        {
            SelectedTabItem = this.treeView;
            try {
                var nodes = new List<XmlHtmlTreeNodeBase>();

                if(browser.BrowserKind == BrowserKind.Html) {
                    nodes.AddRange(GetHtmlTreeNodes(browser));
                    this.webBrowser.NavigateToStream(browser.FileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read));
                } else {
                    nodes.AddRange(GetXmlTreeNodes(browser));
                }
                this.nodes.ItemsSource = nodes.Where(n => n.Showable);
            } catch(Exception ex) {
                this.nodes.ItemsSource = new[] { new ExceptionNode(ex) };
            }

        }

        #endregion

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic activeX = this.webBrowser.GetType().InvokeMember(
                "ActiveXInstance",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                this.webBrowser,
                new object[] { }
            );

            activeX.Silent = true;

        }
    }
}
