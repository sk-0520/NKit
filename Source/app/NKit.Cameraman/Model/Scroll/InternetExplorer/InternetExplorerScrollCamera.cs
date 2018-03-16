using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

        bool HideFixedInFooter { get; set; } = true;

        #endregion

        #region function

        bool IsFixed(IHTMLCurrentStyle currentStyle)
        {
            return currentStyle.position == "fixed";
        }
        bool IsShow(IHTMLCurrentStyle currentStyle)
        {
            return currentStyle.display != "none";
        }

        void SetStyleCore(ElementStocker stocker, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            foreach(var pair in pairs) {
                stocker.Style.Com.setAttribute(pair.Key, pair.Value);
            }
        }
        void SetStyle(ElementStocker stocker, string property, string value)
        {
            SetStyleCore(stocker, new[] { new KeyValuePair<string, string>(property, value) });
        }

        struct TargetElement
        {
            static Regex Splitter = new Regex(@"(?<TAG>[\w\-]+)?(#(?<ID>[\w\-]+))?(\.(?<CLASS>[\w\-]+))?", RegexOptions.ExplicitCapture);
            public TargetElement(string tagProperty)
            {
                var match = Splitter.Match(tagProperty);
                TagName = match.Groups["TAG"].Value;
                Id = match.Groups["ID"].Value;
                Class = match.Groups["CLASS"].Value;
            }

            #region property

            public string TagName { get; }
            public string Id { get; }
            public string Class { get; }

            public bool HasTag => TagName != string.Empty;
            public bool HasId => Id != string.Empty;
            public bool HasClass => Class != string.Empty;

            public bool IsEnabled => HasId || HasTag || (HasTag && HasClass);

            #endregion
        }

        IEnumerable<ElementStocker> GetFixedElementsCore(InternetExplorerWrapper ie, TargetElement targetElements, ElementStocker parentStocker)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ElementStocker> GetFixedElements(InternetExplorerWrapper ie, IEnumerable<TargetElement> targetElements)
        {
            foreach(var targetElement in targetElements.Where(t => t.IsEnabled)) {
                if(targetElement.HasId && !targetElement.HasTag) {
                    var element2 = ie.GetElementById<IHTMLElement2>(targetElement.Id);
                    if(element2 != null) {
                        var stocker = new ElementStocker(element2);
                        if(IsFixed(stocker.CurrentStyle.Com)) {
                            yield return stocker;
                            continue;
                        }
                        stocker.Dispose();
                    }
                    continue;
                }

                using(var collection = ie.GetElementsByTagName(targetElement.TagName)) {
                    foreach(var stocker in ie.CollctionToElements<IHTMLElement2>(collection).Select(elm2 => new ElementStocker(elm2))) {

                        if(targetElement.HasId) {
                            var id = stocker.Element.Com.getAttribute("id");
                            if(string.IsNullOrEmpty(id)) {
                                stocker.Dispose();
                                continue;
                            }
                            if(!string.Equals(id, targetElement.Id, StringComparison.InvariantCultureIgnoreCase)) {
                                stocker.Dispose();
                                continue;
                            }
                        }
                        if(targetElement.HasClass) {
                            if(string.IsNullOrEmpty(stocker.Element.Com.className)) {
                                stocker.Dispose();
                                continue;
                            }
                            if(Array.IndexOf(stocker.Element.Com.className.Split(' '), targetElement.Class) == -1) {
                                stocker.Dispose();
                                continue;
                            }
                        }

                        // 対象が固定されていれば子を考慮する必要なし
                        if(IsFixed(stocker.CurrentStyle.Com)) {
                            yield return stocker;
                            continue;
                        }

                        // いらない要素
                        stocker.Dispose();
                    }
                }
            }
        }

        void HideStockItems(IEnumerable<ElementStocker> items)
        {
            // 表示要素に限定して処理していく
            foreach(var item in items.Where(i => IsShow(i.CurrentStyle.Com))) {
                SetStyle(item, "display", "none");
            }
        }

        void ShowStockItems(IEnumerable<ElementStocker> items)
        {
            foreach(var item in items.Where(i => !IsShow(i.CurrentStyle.Com))) {
                SetStyle(item, "display", item.StockStyle["display"]);
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

                IReadOnlyList<TargetElement> headerTagClassItems = HideFixedInHeader ? Constants.HideHeaderTagClassItems.Select(s => new TargetElement(s)).ToList() : null;
                IReadOnlyList<TargetElement> footerTagClassItems = HideFixedInHeader ? Constants.HideFooterTagClassItems.Select(s => new TargetElement(s)).ToList() : null;


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
                                        footerStockItems.SetRange(GetFixedElements(ie, footerTagClassItems));
                                        HideStockItems(footerStockItems);
                                    }
                                }

                                Wait();

                                // スクロール中にウィンドウを動かすバカのために毎度毎度座標を取得する
                                NativeMethods.GetWindowRect(WindowHandle, out var rect);
                                g.CopyFromScreen(rect.Left - diffPoint.X, rect.Top - diffPoint.Y, imageX, imageY, diffSize);
                                Logger.Trace($"{imageX} * {imageY}, {diffPoint}, {diffSize}");

                                if(Constants.ScrollInternetExplorerDebug) {
                                    g.DrawString($"{imageX} * {imageY}, {diffPoint}, {diffSize}", SystemFonts.DialogFont, SystemBrushes.ActiveCaption, new PointF(imageX, imageY));
                                    g.DrawLine(SystemPens.AppWorkspace, imageX, imageY, imageX + diffSize.Width, imageY + diffSize.Height);
                                }

                                // 毎回取得する代わりに毎回戻してあげないともう何が何だか
                                if(headerStockItems.Any()) {
                                    ShowStockItems(headerStockItems);
                                }
                                if(footerStockItems.Any()) {
                                    ShowStockItems(footerStockItems);
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
