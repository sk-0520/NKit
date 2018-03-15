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

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll
{
    public class InternetExplorerScrollCamera : ScrollCameraBase
    {
        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan delayTime)
            : base(hWnd, delayTime)
        { }

        #region nproperty

        public TimeSpan SendMessageWaitTime { get; set; }
        public TimeSpan DocumentWaitTime { get; set; }

        #endregion

        #region WindowHandleCamera

        protected override Image TaskShotCore()
        {
            using(var ie = new InternetExplorerWrapper(WindowHandle)) {
                ie.SendMessageWaitTime = SendMessageWaitTime;
                ie.DocumentWaitTime = DocumentWaitTime;

                if(!ie.Initialize()) {
                    Logger.Warning($"{nameof(InternetExplorerWrapper)}.{nameof(InternetExplorerWrapper.Initialize)}: failure");
                    return null;
                }

                // IE が取得出来たらまずは一番上まで移動する
                ie.ScrollTo(0, 0);
                Wait();

                var scale = ie.GetScale();
                var clientSize = ie.GetClientSize();
                var scrollSize = ie.GetScrollSize();

                // キャプチャ取得用の画像作成
                var blockSize = new Size((int)(clientSize.Width * scale.X), (int)(clientSize.Height * scale.Y));
                var imageSize = new Size((int)(scrollSize.Width * scale.X), (int)(scrollSize.Height * scale.Y));
                var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                using(var g = Graphics.FromImage(bitmap)) {
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
                            Wait();

                            // スクロール中にウィンドウを動かすバカのために毎度毎度座標を取得する
                            NativeMethods.GetWindowRect(WindowHandle, out var rect);
                            g.CopyFromScreen(rect.Left - diffPoint.X, rect.Top - diffPoint.Y, imageX, imageY, diffSize);
#if DEBUG
                            g.DrawString($"{imageX} * {imageY}, {diffPoint}, {diffSize}", SystemFonts.DialogFont, SystemBrushes.ActiveCaption, new PointF(imageX, imageY));
#endif

                        }
                    }
                }
                return bitmap;
            }


            //return null;
        }

        #endregion
    }

    public class InternetExplorerWrapper : ModelBase
    {
        #region define

        const string HtmlGetObjectWindowMessage = "WM_HTML_GETOBJECT";

        #endregion

        public InternetExplorerWrapper(IntPtr hWnd)
        {
            ServerWindowHandle = hWnd;
        }

        #region property

        IntPtr ServerWindowHandle { get; }

        public TimeSpan SendMessageWaitTime { get; set; } 
        public TimeSpan DocumentWaitTime { get; set; } 

        ComModel<IHTMLDocument2> HtmlDocument { get; set; }
        ComModel<IHTMLWindow2> HtmlWindow { get; set; }
        ComModel<IHTMLScreen> HtmlScreen { get; set; }
        ComModel<IHTMLScreen2> HtmlScreen2 { get; set; }
        ComModel<IWebBrowser2> WebBrowser { get; set; }

        ComModel<IHTMLDocument2> Document2 { get; set; }
        ComModel<IHTMLDocument3> Document3 { get; set; }

        ComModel<IHTMLElement2> Body2 { get; set; }
        ComModel<IHTMLElement2> Body3_2 { get; set; }
        ComModel<IHTMLElement3> Body3_3 { get; set; }


        #endregion

        #region function

        public bool Initialize()
        {
            var message = WindowHandleUtility.RegisterWindowMessage(HtmlGetObjectWindowMessage);
            if(message == 0) {
                Logger.Warning($"${HtmlGetObjectWindowMessage} is 0");
                return false;
            }

            var timeoutResult = NativeMethods.SendMessageTimeout(ServerWindowHandle, message, UIntPtr.Zero, IntPtr.Zero, SMTO.SMTO_ABORTIFHUNG, (uint)SendMessageWaitTime.TotalMilliseconds, out UIntPtr sendMessageResult);
            if(timeoutResult == IntPtr.Zero) {
                Logger.Warning($"${nameof(NativeMethods.SendMessageTimeout)} is 0");
                return false;
            }
            // なんか知らんけど 1 秒まったらサクサク動き始めてもう意味が分からんから適当に待つことにしたよ
            Logger.Trace($"sleep: {DocumentWaitTime}");
            Thread.Sleep(DocumentWaitTime);

            var IID_IHTMLDocument2 = typeof(IHTMLDocument2).GUID;
            IHTMLDocument2 rawHtmlDocument = (IHTMLDocument2)WindowHandleUtility.ObjectFromLresult(sendMessageResult, IID_IHTMLDocument2, IntPtr.Zero);
            if(rawHtmlDocument == null) {
                Logger.Warning($"{nameof(IHTMLDocument2)} is null, {sendMessageResult}, {IID_IHTMLDocument2}");
                return false;
            }

            HtmlDocument = ComModel.Create(rawHtmlDocument);
            HtmlWindow = ComModel.Create(HtmlDocument.Com.parentWindow);
            HtmlScreen = ComModel.Create(HtmlWindow.Com.screen);
            HtmlScreen2 = ComModel.Create((IHTMLScreen2)HtmlWindow.Com.screen);

            using(var serviceProvider = ComModel.Create((WindowHandleUtility.IServiceProvider)HtmlDocument.Com.parentWindow)) {
                var SID_SWebBrowserApp = typeof(IWebBrowserApp).GUID;
                var IID_webBrowser = typeof(IWebBrowser2).GUID;
                serviceProvider.Com.QueryService(ref SID_SWebBrowserApp, ref IID_webBrowser, out object queryResult);
                var rawWebBrowser = (IWebBrowser2)queryResult;
                if(rawWebBrowser == null) {
                    Logger.Warning($"{nameof(IWebBrowser2)} is null, {SID_SWebBrowserApp}, {IID_webBrowser}");
                    return false;
                }
                WebBrowser = ComModel.Create(rawWebBrowser);
            }

            Document2 = ComModel.Create((IHTMLDocument2)WebBrowser.Com.Document);
            Document3 = ComModel.Create((IHTMLDocument3)WebBrowser.Com.Document);

            Body2 = ComModel.Create((IHTMLElement2)Document2.Com.body);
            Body3_2 = ComModel.Create((IHTMLElement2)Document3.Com.documentElement);
            Body3_3 = ComModel.Create((IHTMLElement3)Document3.Com.documentElement);

            Logger.Information($"c: {Body2.Com.clientWidth} * {Body2.Com.clientHeight}");
            Logger.Information($"s: {Body2.Com.scrollWidth} * {Body2.Com.scrollHeight}");
            Logger.Information($"c: {Body3_2.Com.clientWidth} * {Body3_2.Com.clientHeight}");
            Logger.Information($"s: {Body3_2.Com.scrollWidth} * {Body3_2.Com.scrollHeight}");


            Logger.Information($"*: {HtmlScreen.Com.width} * {HtmlScreen.Com.height}");
            Logger.Information($"a: {HtmlScreen.Com.availWidth} * {HtmlScreen.Com.availWidth}");

            Logger.Information($"d: {HtmlScreen2.Com.deviceXDPI} * {HtmlScreen2.Com.deviceYDPI}");
            Logger.Information($"l: {HtmlScreen2.Com.logicalXDPI} * {HtmlScreen2.Com.logicalYDPI}");


            return true;
        }

        public Size GetClientSize()
        {
            return new Size(
                Body3_2.Com.clientWidth,
                Body3_2.Com.clientHeight
            );
        }
        public Size GetScrollSize()
        {
            return new Size(
                Body3_2.Com.scrollWidth,
                Body3_2.Com.scrollHeight
            );
        }

        public PointF GetScale()
        {
            return new PointF(
                HtmlScreen2.Com.deviceXDPI / (float)HtmlScreen2.Com.logicalXDPI,
                HtmlScreen2.Com.deviceYDPI / (float)HtmlScreen2.Com.logicalYDPI
            );
        }

        /// <summary>
        /// 絶対座標でスクロール。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ScrollTo(int x, int y)
        {
            HtmlWindow.Com.scroll(x, y);
        }

        /// <summary>
        /// 相対座標でのスクロール。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ScrollBy(int x, int y)
        {
            HtmlWindow.Com.scrollBy(x, y);
        }


        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                Body3_3?.Dispose();
                Body3_2?.Dispose();
                Body2?.Dispose();

                Document3?.Dispose();
                Document2?.Dispose();

                WebBrowser?.Dispose();
                HtmlScreen2?.Dispose();
                HtmlScreen?.Dispose();
                HtmlWindow?.Dispose();
                HtmlDocument?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

}
