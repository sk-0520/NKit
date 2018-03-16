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
        #region define

        struct TargetElement
        {
            static readonly Regex Splitter = new Regex(@"(?<TAG>[\w\-]+)?(#(?<ID>[\w\-]+))?(\.(?<CLASS>[\w\-]+))?", RegexOptions.ExplicitCapture);
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

        #endregion

        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan delayTime)
            : base(hWnd, delayTime)
        { }

        #region property

        public TimeSpan SendMessageWaitTime { get; set; }
        public TimeSpan DocumentWaitTime { get; set; }

        /// <summary>
        /// スクロール中にヘッダーと思しきものが固定表示なら非表示にするか。
        /// </summary>
        public bool HideFixedHeader { get; set; } = true;
        /// <summary>
        /// スクロール中に隠すヘッダー要素設定。
        /// <para><seealso cref="TargetElement"/>を参照。</para>
        /// </summary>
        public string HideFixedHeaderElements { get; set; } = Constants.HideHeaderTagClassItems;
        /// <summary>
        /// スクロール中にフッターと思しきものが固定表示なら非表示にするか。
        /// </summary>
        public bool HideFixedFooter { get; set; } = true;
        /// <summary>
        /// スクロール中に隠すフッター要素設定。
        /// <para><seealso cref="TargetElement"/>を参照。</para>
        /// </summary>
        public string HideFixedFooterElements { get; set; } = Constants.HideFooterTagClassItems;

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

        IEnumerable<ElementStocker> GetFixedElementsCore(InternetExplorerWrapper ie, TargetElement targetElements, ElementStocker parentStocker)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ElementStocker> GetFixedElements(InternetExplorerWrapper ie, IEnumerable<TargetElement> targetElements)
        {
            var stopwatchAll = Stopwatch.StartNew();
            foreach(var targetElement in targetElements.Where(t => t.IsEnabled)) {
                // getElementById で済むやつを先に対応
                if(targetElement.HasId && !targetElement.HasTag) {
                    var stopwatchId = Stopwatch.StartNew();
                    var element = ie.GetElementById<IHTMLElement>(targetElement.Id);
                    Logger.Debug($"get id time: {stopwatchId.Elapsed}");

                    if(element != null) {
                        var stocker = new ElementStocker(element);
                        if(IsFixed(stocker.CurrentStyle.Com)) {
                            Logger.Debug($"get id result time: {stopwatchId.Elapsed}");
                            yield return stocker;
                            continue;
                        }
                        stocker.Dispose();
                        Logger.Debug($"get id result none: {stopwatchId.Elapsed}");
                    }
                    continue;
                }

                var stopwatchTag = Stopwatch.StartNew();
                using(var collection = ie.GetElementsByTagName(targetElement.TagName)) {
                    Logger.Debug($"get tag time: {stopwatchTag.Elapsed}");

                    foreach(var element in ie.CollctionToElements<IHTMLElement>(collection)) {
                        var stopwatchStock = Stopwatch.StartNew();
                        var stocker = new ElementStocker(element);

                        Logger.Debug($"get stock time: {stopwatchStock.Elapsed}");

                        if(targetElement.HasId) {
                            var id = stocker.Element.Com.id;
                            if(string.IsNullOrEmpty(id)) {
                                Logger.Debug($"get stock id empty: {stopwatchStock.Elapsed}");
                                stocker.Dispose();
                                continue;
                            }
                            if(!string.Equals(id, targetElement.Id, StringComparison.InvariantCultureIgnoreCase)) {
                                Logger.Debug($"get stock id unmatch: {stopwatchStock.Elapsed}");
                                stocker.Dispose();
                                continue;
                            }
                        }
                        if(targetElement.HasClass) {
                            // これがクッソみたいに遅い
                            var className = stocker.Element.Com.className;
                            if(string.IsNullOrEmpty(className)) {
                                stocker.Dispose();
                                Logger.Debug($"get stock class empty: {stopwatchStock.Elapsed}");
                                continue;
                            }
                            if(!className.Split(' ').Any(s => string.Equals(s, targetElement.Class, StringComparison.InvariantCultureIgnoreCase))) {
                                stocker.Dispose();
                                Logger.Debug($"get stock class unmatch: {stopwatchStock.Elapsed}");
                                continue;
                            }
                        }

                        if(IsFixed(stocker.CurrentStyle.Com)) {
                            Logger.Debug($"get stock result time: {stopwatchTag.Elapsed}");
                            yield return stocker;
                            continue;
                        }

                        // いらない要素
                        stocker.Dispose();
                        Logger.Debug($"get stock result none: {stopwatchStock.Elapsed}");
                    }
                }
            }
            Logger.Information($"get all time: {stopwatchAll.Elapsed}");
        }

        void HideStockItems(IEnumerable<ElementStocker> items)
        {
            // 表示要素に限定して処理していく
            var filterdItems = items.Where(i => IsShow(i.CurrentStyle.Com));
            foreach(var item in filterdItems) {
                SetStyle(item, "display", "none");
            }
        }

        void ShowStockItems(IEnumerable<ElementStocker> items)
        {
            var filterdItems = items
                .Where(i => !IsShow(i.CurrentStyle.Com))
                .Where(i => i.StockStyle.ContainsKey("display"))
            ;
            foreach(var item in filterdItems) {
                SetStyle(item, "display", item.StockStyle["display"]);
            }
        }

        IReadOnlyList<TargetElement> SplitTargetElements(string source)
        {
            return source
                .Split(':')
                .Select(s => s.Trim())
                .Select(s => new TargetElement(s))
                .ToList()
            ;
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

                IReadOnlyList<TargetElement> headerTagClassItems = HideFixedHeader ? SplitTargetElements(HideFixedHeaderElements) : null;
                IReadOnlyList<TargetElement> footerTagClassItems = HideFixedFooter ? SplitTargetElements(HideFixedFooterElements) : null;


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
                                var stopwatch = Stopwatch.StartNew();
                                if(HideFixedHeader) {
                                    // 最上位以外をスクロール中ならヘッダ要素を隠す
                                    if(0 < imageY) {
                                        // スクロール毎に取得しないと世の中わけわからんことがいっぱいで死にたい
                                        headerStockItems.SetRange(GetFixedElements(ie, headerTagClassItems));
                                    }
                                }
                                if(HideFixedFooter) {
                                    // スクロール中ならフッタ要素を隠す
                                    if(imageY + blockSize.Height < imageSize.Height) {
                                        footerStockItems.SetRange(GetFixedElements(ie, footerTagClassItems));
                                    }
                                }
                                var stockItems = headerStockItems.Concat(footerStockItems).ToList();
                                HideStockItems(stockItems);

                                WaitMinus(stopwatch.Elapsed);

                                // スクロール中にウィンドウを動かすバカのために毎度毎度座標を取得する
                                NativeMethods.GetWindowRect(WindowHandle, out var rect);
                                g.CopyFromScreen(rect.Left - diffPoint.X, rect.Top - diffPoint.Y, imageX, imageY, diffSize);
                                Logger.Trace($"{imageX} * {imageY}, {diffPoint}, {diffSize}");

                                if(Constants.ScrollInternetExplorerDebug) {
                                    g.DrawString($"{imageX} * {imageY}, {diffPoint}, {diffSize}", SystemFonts.DialogFont, SystemBrushes.ActiveCaption, new PointF(imageX, imageY));
                                    g.DrawLine(SystemPens.AppWorkspace, imageX, imageY, imageX + diffSize.Width, imageY + diffSize.Height);
                                }

                                // 毎回取得する代わりに毎回戻してあげないともう何が何だか
                                if(stockItems.Any()) {
                                    ShowStockItems(stockItems);
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
