//#define NEED_DEBUG
#if NEED_DEBUG
#   if DEBUG
#   elif BETA
#   else
#       error Release! defined NEED_DEBUG
#   endif
#endif

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

            Style = ComModel.Create(Element.Com.style);
            CurrentStyle = ComModel.Create(Element2.Com.currentStyle);

#if NEED_DEBUG
            D = new DEBUG(this);
#endif
        }

        #region property

        public ComModel<IHTMLElement> Element { get; }
        public ComModel<IHTMLElement2> Element2 { get; }

        public ComModel<IHTMLStyle> Style { get; }
        public ComModel<IHTMLCurrentStyle> CurrentStyle { get; }

        public IDictionary<string, string> StockStyle { get; } = new Dictionary<string, string>();

#if NEED_DEBUG
        DEBUG D { get; }
#endif

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                CurrentStyle.Dispose();
                Style.Dispose();
                Element2.Dispose();
                Element.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    // COM の中身をデバッガで見るの一苦労なんすよ
    public class DEBUG
    {
        public DEBUG(ElementStocker es)
        {
            Urn = es.Element2.Com.tagUrn;
            TagName = es.Element.Com.tagName;
            Id = es.Element.Com.getAttribute("id");
            ClassName = es.Element.Com.getAttribute("className");

            var styleType = typeof(IHTMLCurrentStyle);
            foreach(var prop in styleType.GetProperties()) {
                Style[prop.Name] = Convert.ToString(prop.GetValue(es.Element2.Com.currentStyle));
            }
        }

        #region property

        public string Urn { get; }
        public string TagName { get; }
        public string Id { get; }
        public string ClassName { get; }
        public IDictionary<string, string> Style { get; } = new Dictionary<string, string>();

        #endregion
    }

    /// <summary>
    /// かなり現定期的なリスト。
    /// <para>面倒なことは全部面倒見させる</para>
    /// </summary>
    public class ElementStockerList : Collection<ElementStocker>, IDisposable
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
        public void SetRange(IEnumerable<ElementStocker> items)
        {
            DisposeItems();
            Clear();

            foreach(var item in items) {
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
