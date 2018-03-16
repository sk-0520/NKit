using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// かなり現定期的なリスト。
    /// <para>面倒なことは全部面倒見させる</para>
    /// </summary>
    public class ElementStockerList: Collection<ElementStocker>, IDisposable
    {
        public ElementStockerList()
        { }

        ~ElementStockerList()
        {
            Dispose(false);
        }

        #region function

        void DisposeItems()
        {
            foreach(var item in Items) {
                item.Dispose();
            }
        }

        /// <summary>
        /// 集合をどさっと設定。
        /// <para>既に存在する要素は<see cref="IDisposable.Dispose"/>後にクリアされる</para>
        /// </summary>
        /// <param name="items"></param>
        public void SetRange(IEnumerable<ComModel<IHTMLElement2>> items)
        {
            DisposeItems();
            Clear();

            foreach(var item in items.Select(i => new ElementStocker(i))) {
                Add(item);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>時に呼び出されるイベント。
        /// <para>呼び出し時点では<see cref="IsDisposed"/>は偽のまま。</para>
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Disposing;

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>されたか。
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected void ThrowIfDisposed([CallerMemberName] string _callerMemberName = "")
        {
            if(IsDisposed) {
                throw new ObjectDisposedException(_callerMemberName);
            }
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>の内部処理。
        /// <para>継承先クラスでは本メソッドを呼び出す必要がある。</para>
        /// </summary>
        /// <param name="disposing">CLRの管理下か。</param>
        protected virtual void Dispose(bool disposing)
        {
            if(IsDisposed) {
                return;
            }

            DisposeItems();

            if(Disposing != null) {
                Disposing(this, EventArgs.Empty);
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }

            IsDisposed = true;
        }

        /// <summary>
        /// 解放。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
