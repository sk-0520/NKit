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
        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan waitTime)
            : base(hWnd, waitTime)
        { }


        #region WindowHandleCamera

        protected override Image TaskShotCore()
        {
            using(var ie = new InternetExplorerWrapper(WindowHandle)) {
                if(!ie.Initialize()) {
                    Logger.Warning($"{nameof(InternetExplorerWrapper)}.{nameof(InternetExplorerWrapper.Initialize)}: failure");
                    return null;
                }

                // IE が取得出来たらまずは一番上まで移動する
                ie.Scroll(0, 0);
                Wait();

                

            }


            return null;
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

        public TimeSpan SendMessageWaitTime { get; } = TimeSpan.FromMilliseconds(500);
        public TimeSpan DocumentWaitTime { get; } = TimeSpan.FromMilliseconds(500);

        ComModel<IHTMLDocument2> HtmlDocument { get; set; }
        ComModel<IHTMLWindow2> HtmlWindow { get; set; }
        ComModel<IHTMLScreen> HtmlScreen { get; set; }
        ComModel<IHTMLScreen2> HtmlScreen2 { get; set; }
        ComModel<IWebBrowser2> WebBrowser { get; set; }

        ComModel<IHTMLDocument2> Document2 { get; set; }
        ComModel<IHTMLDocument3> Document3 { get; set; }

        ComModel<IHTMLElement2> Body2 { get; set; }
        ComModel<IHTMLElement3> Body3 { get; set; }


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
            Body3 = ComModel.Create((IHTMLElement3)Document3.Com.documentElement);

            Logger.Information($"c: {Body2.Com.clientWidth} * {Body2.Com.clientHeight}");
            Logger.Information($"s: {Body2.Com.scrollWidth} * {Body2.Com.scrollHeight}");

            Logger.Information($"*: {HtmlScreen.Com.width} * {HtmlScreen.Com.height}");
            Logger.Information($"a: {HtmlScreen.Com.availWidth} * {HtmlScreen.Com.availWidth}");

            Logger.Information($"d: {HtmlScreen2.Com.deviceXDPI} * {HtmlScreen2.Com.deviceYDPI}");
            Logger.Information($"l: {HtmlScreen2.Com.logicalXDPI} * {HtmlScreen2.Com.logicalYDPI}");

            
            return true;
        }

        public void Scroll(int x, int y)
        {
            HtmlWindow.Com.scroll(0, 0);
        }

        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                Body3?.Dispose();
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
