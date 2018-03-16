using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using mshtml;
using SHDocVw;

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll.InternetExplorer
{
    public class InternetExplorerScrollCamera : ScrollCameraBase
    {
        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan delayTime)
            : base(hWnd, delayTime)
        { }

        #region property

        public TimeSpan SendMessageWaitTime { get; set; }
        public TimeSpan DocumentWaitTime { get; set; }

        /// <summary>
        /// スクロール中に header 要素(と子要素) が固定表示なら非表示にするか。
        /// </summary>
        bool HideFixedInHeader { get; set; } = true;

        bool HideFixedInFooter { get; set; } = !true;

        #endregion

        #region function

        bool IsFixed(ComModel<IHTMLElement2> element2)
        {
            using(var style = ComModel.Create(element2.Com.currentStyle)) {
                return style.Com.position == "fixed";
            }
        }
        bool IsShow(ComModel<IHTMLElement2> element2)
        {
            using(var style = ComModel.Create(element2.Com.currentStyle)) {
                return style.Com.display != "none";
            }
        }

        void SetStyleCore(ComModel<IHTMLElement> element, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            using(var style = ComModel.Create(element.Com.style)) {
                foreach(var pair in pairs) {
                    style.Com.setAttribute(pair.Key, pair.Value);
                }
            }
        }
        void SetStyle(ComModel<IHTMLElement> element, string property, string value)
        {
            SetStyleCore(element, new[] { new KeyValuePair<string, string>(property, value) });
        }

        struct TagClass
        {
            public TagClass(string tagProperty)
            {
                var items = tagProperty.Split('.');
                TagName = items[0];

                if(items.Length == 1) {
                    ClassName = null;
                } else {
                    ClassName = items[1];
                }
            }

            #region property

            public string TagName { get; }
            public string ClassName { get; }

            public bool HasClass => ClassName != null;

            #endregion
        }

        IEnumerable<ComModel<IHTMLElement2>> GetFixedElements(InternetExplorerWrapper ie, IEnumerable<TagClass> tagClassItems)
        {
            foreach(var tagClass in tagClassItems) {
                using(var collection = ie.GetElementsByTagName(tagClass.TagName)) {
                    foreach(var targetElement2 in ie.CollctionToElements<IHTMLElement2>(collection)) {
                        using(var targetElement = ComModel.Create((IHTMLElement)targetElement2.Com)) {
                            if(tagClass.HasClass) {
                                if(string.IsNullOrEmpty(targetElement.Com.className)) {
                                    targetElement2.Dispose();
                                    continue;
                                }
                                if(Array.IndexOf(targetElement.Com.className.Split(' '), tagClass.ClassName) == -1) {
                                    targetElement2.Dispose();
                                    continue;
                                }
                            }

                            // 対象が固定されていれば子を考慮する必要なし
                            if(IsFixed(targetElement2)) {
                                yield return targetElement2;
                                continue;
                            }

                            // 子の固定状況を確認する
                            // あんまりやる気ないけど、一応
                            using(var children = ComModel.Create((IHTMLElementCollection)targetElement.Com.all)) {
                                foreach(var childElement in ie.CollctionToElements<IHTMLElement2>(children)) {
                                    if(IsFixed(childElement)) {
                                        yield return childElement;
                                        continue;
                                    }

                                    // いらない子
                                    childElement.Dispose();
                                }
                            }
                        }

                        // いらない指定タグ要素
                        targetElement2.Dispose();
                    }
                }
            }
        }

        void HideStockItems(IEnumerable<ElementStocker> items)
        {
            // 表示要素に限定して処理していく
            foreach(var item in items/*.Where(i => IsShow(i.Element2))*/) {
                SetStyle(item.Element, "display", "none");
            }
        }

        void ShowStockItems(IEnumerable<ElementStocker> items)
        {
            foreach(var item in items/*.Where(i => !IsShow(i.Element2))*/) {
                SetStyle(item.Element, "display", item.StockStyle["display"]);
            }
        }


        #endregion

        #region WindowHandleCamera

        protected override Image TakeShotCore()
        {
            using(var ie = new InternetExplorerWrapper(WindowHandle)) {
                ie.SendMessageWaitTime = SendMessageWaitTime;
                ie.DocumentWaitTime = DocumentWaitTime;

                if(!ie.Initialize()) {
                    Logger.Warning($"{nameof(InternetExplorerWrapper)}.{nameof(InternetExplorerWrapper.Initialize)}: failure");
                    return null;
                }

                var scale = ie.GetScale();
                var clientSize = ie.GetClientSize();
                var scrollSize = ie.GetScrollSize();

                IReadOnlyList<TagClass> headerTagClassItems = HideFixedInHeader ? Constants.HideHeaderTagClassItems.Select(s => new TagClass(s)).ToList() : null;
                IReadOnlyList<TagClass> footerTagClassItems = HideFixedInHeader ? Constants.HideFooterTagClassItems.Select(s => new TagClass(s)).ToList() : null;


                // キャプチャ取得用の画像作成
                var blockSize = new Size((int)(clientSize.Width * scale.X), (int)(clientSize.Height * scale.Y));
                var imageSize = new Size((int)(scrollSize.Width * scale.X), (int)(scrollSize.Height * scale.Y));
                var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                using(var g = Graphics.FromImage(bitmap)) {
                    // 一番上(0, 0)から順次取得
                    for(var imageX = 0; imageX < imageSize.Width; imageX += blockSize.Width) {
                        for(var imageY = 0; imageY < imageSize.Height; imageY += blockSize.Height) {
                            // スクロールの位置と貼り付け先画像位置は一致しない(特に最後の方)ので補正
                            var diffPoint = new Point(
                                imageX + blockSize.Width < imageSize.Width ? 0 : imageSize.Width - (imageX + blockSize.Width),
                                imageY + blockSize.Height < imageSize.Height ? 0 : imageSize.Height - (imageY + blockSize.Height)
                            );
                            var diffSize = new Size(
                                blockSize.Width + diffPoint.X,
                                blockSize.Height + diffPoint.Y
                            );

                            ie.ScrollTo((int)(imageX / scale.X), (int)(imageY / scale.Y));

                            using(var headerStockItems = new ElementStockerList())
                            using(var footerStockItems = new ElementStockerList()) {
                                if(HideFixedInHeader) {
                                    if(0 < imageY) {
                                        // スクロール中ならヘッダ要素を隠す
                                        // スクロール毎に取得しないと世の中わけわからんことがいっぱいで死にたい
                                        headerStockItems.SetRange(GetFixedElements(ie, headerTagClassItems));
                                        HideStockItems(headerStockItems);
                                    }
                                }
                                if(HideFixedInFooter) {
                                    if(imageY + blockSize.Height < imageSize.Height) {
                                        headerStockItems.SetRange(GetFixedElements(ie, footerTagClassItems));
                                        HideStockItems(headerStockItems);
                                    }
                                }

                                Wait();

                                // スクロール中にウィンドウを動かすバカのために毎度毎度座標を取得する
                                NativeMethods.GetWindowRect(WindowHandle, out var rect);
                                g.CopyFromScreen(rect.Left - diffPoint.X, rect.Top - diffPoint.Y, imageX, imageY, diffSize);
                                Logger.Trace($"{imageX} * {imageY}, {diffPoint}, {diffSize}");
#if DEBUG
                                g.DrawString($"{imageX} * {imageY}, {diffPoint}, {diffSize}", SystemFonts.DialogFont, SystemBrushes.ActiveCaption, new PointF(imageX, imageY));
                                g.DrawLine(SystemPens.AppWorkspace, imageX, imageY, imageX + diffSize.Width, imageY + diffSize.Height);
#endif
                                if(headerStockItems.Any()) {
                                    // 毎回取得する代わりに毎回戻してあげないともう何が何だか
                                    ShowStockItems(headerStockItems);
                                }
                            }
                        }
                    }
                }



                return bitmap;
            }


            //return null;
        }

        #endregion
    }
}
