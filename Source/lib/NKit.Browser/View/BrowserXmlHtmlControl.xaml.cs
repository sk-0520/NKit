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

        #region UserControl

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);

        //    var items = UIUtility.FindChildren<TreeViewItem>(this.treeView)
        //        .Where(t => t.IsVisible)
        //        .Select(t => new { View = t, Data = (XmlHtmlTreeNodeBase)t.DataContext })
        //        .Where(i => !i.Data.HasText)
        //        .ToList()
        //    ;

        //    //this.treeHeader.Children.Clear();

        //    var typeface = new Typeface(
        //        FontFamily,
        //        FontStyle,
        //        FontWeight,
        //        FontStretch
        //    );
        //    foreach(var item in items) {
        //        var trans = item.View.TransformToAncestor(this.treeView);
        //        var position = trans.Transform(new Point(0, 0));
        //        if(position.Y < 0) {
        //            continue;
        //        }
                
        //        if(this.treeView.ActualHeight < position.Y) {
        //            continue;
        //        }
        //        position.X = 0;
        //        position.Y = 20;
        //        var header = new TextBlock() {
        //            Text = item.Data.Name,
        //        };
        //        var formattedText = new FormattedText(item.Data.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, FontSize, Foreground);
        //        header.SetValue(Canvas.TopProperty, position.Y);
        //        //this.treeHeader.Children.Add(header);
        //        drawingContext.DrawText(formattedText, position);

        //        drawingContext.DrawRoundedRectangle(
        //        Brushes.Red,
        //        null,
        //        new Rect(0, 0, ActualWidth, ActualHeight),
        //        4.0d, 4.0d);
        //    }
        //}

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
            var items = UIUtility.FindChildren<TreeViewItem>(this.treeView)
                .Where(t => t.IsVisible)
                .Select(t => new { View = t, Data = (XmlHtmlTreeNodeBase)t.DataContext })
                .Where(i => !i.Data.HasText)
                .Select(i => new { i.View, i.Data, Position = i.View.TransformToAncestor(this.treeView).Transform(new Point(0, 0)) })
                .Where(i => 0  < i.Position.Y&& i.Position.Y < this.treeView.ActualHeight)
                .ToList()
            ;

            this.treeHeader.Children.Clear();

            foreach(var item in items) {
                var header = new TextBlock() {
                    Text = item.Data.Name,
                };
                header.SetValue(Canvas.TopProperty, item.Position.Y);
                this.treeHeader.Children.Add(header);
            }

        }
    }
}
