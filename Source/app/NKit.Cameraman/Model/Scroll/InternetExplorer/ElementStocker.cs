using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using mshtml;

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll.InternetExplorer
{
    public class ElementStocker : DisposerBase
    {
        #region define

        static string[] stringProperties = new[] {
            nameof(IHTMLStyle.position),
            nameof(IHTMLStyle.display),
        };

        #endregion

        public ElementStocker(ComModel<IHTMLElement2> element)
        {
            Element = ComModel.Create((IHTMLElement)element.Com);
            Element2 = element;
            using(var style = ComModel.Create(Element2.Com.currentStyle)) {
                foreach(var property in stringProperties) {
                    var value = (string)style.Com.getAttribute(property);
                    StockStyle[property] = value;
                }

                StockStyle[nameof(style.Com.position)] = style.Com.position;
                StockStyle[nameof(style.Com.display)] = style.Com.display;
            }
        }

        #region property

        public ComModel<IHTMLElement> Element { get; }
        public ComModel<IHTMLElement2> Element2 { get; }

        public IDictionary<string, string> StockStyle { get; } = new Dictionary<string, string>();

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                Element2.Dispose();
                Element.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
