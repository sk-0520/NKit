using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper;
using ContentTypeTextNet.NKit.Utility.Model;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Browser.View
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
            var doc = new HtmlDocument() {
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = browser.Encoding,
                OptionReadEncoding = true,
            };

            lock(browser) {
                doc.Load(browser.GetSharedStream());
            }

            return doc.DocumentNode.ChildNodes.Cast<HtmlNode>().Select(n => new HtmlTreeNode(n));
        }

        IEnumerable<XmlTreeNode> GetXmlTreeNodes(BrowserViewModel browser)
        {
            var doc = new XmlDocument();

            lock(browser) {
                doc.Load(browser.GetSharedStream());
            }

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
            SelectedTabItem = this.treeTabItem;
            try {
                var nodes = new List<XmlHtmlTreeNodeBase>();

                if(browser.BrowserKind == BrowserKind.Html) {
                    nodes.AddRange(GetHtmlTreeNodes(browser));
                    this.webBrowser.NavigateToStream(browser.GetSharedStream());
                } else {
                    nodes.AddRange(GetXmlTreeNodes(browser));
                }
                this.treeView.ItemsSource = nodes.Where(n => n.Showable);
            } catch(Exception ex) {
                this.treeView.ItemsSource = new[] { new ExceptionNode(ex) };
            }
        }

        #endregion

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.webBrowser.IsVisible) {
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

        private void treeView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.treeHeader.ItemsSource = HeaderNodeUtility.GetHeaderNodes<XmlHtmlTreeNodeBase>(this.treeView);
        }
    }
}
