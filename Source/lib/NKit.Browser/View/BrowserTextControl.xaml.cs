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
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Utility.Model;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ContentTypeTextNet.NKit.Browser.View
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
        IHighlightingDefinition GetDefaultHighlighting(BrowserViewModel browser)
        {
            switch(browser.BrowserKind) {
                case BrowserKind.CSharp:
                    return HighlightingManager.Instance.GetDefinition("C#");

                case BrowserKind.Xml:
                    return HighlightingManager.Instance.GetDefinition("XML");

                case BrowserKind.Html:
                    return HighlightingManager.Instance.GetDefinition("HTML");

                default:
                    return null;
            }
        }

        IHighlightingDefinition GetCustomHighlighting(BrowserViewModel browser)
        {
            return null;
        }

        IHighlightingDefinition GetHighlighting(BrowserViewModel browser)
        {
            var defaultHighlighting = GetDefaultHighlighting(browser);

            if(defaultHighlighting != null) {
                return defaultHighlighting;
            }

            return GetCustomHighlighting(browser);
        }

        #endregion

        #region IBrowserDetail

        public bool CanBrowse(BrowserViewModel browser)
        {
            return browser.CanBrowse(browser.BrowserKind);
        }

        public void BuildControl(BrowserViewModel browser)
        {
            //var reader = new StreamReader(, browser.Encoding);
            lock(browser) {
                this.editor.Load(browser.GetSharedStream());
            }
            this.editor.SyntaxHighlighting = GetHighlighting(browser);

        }

        #endregion
    }
}
