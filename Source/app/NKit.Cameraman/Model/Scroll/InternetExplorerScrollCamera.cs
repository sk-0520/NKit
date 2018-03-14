using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using mshtml;
using SHDocVw;

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll
{
    public class InternetExplorerScrollCamera : ScrollCameraBase
    {
        #region define

        const string HtmlGetObjectWindowMessage = "WM_HTML_GETOBJECT";

        #endregion

        public InternetExplorerScrollCamera(IntPtr hWnd, TimeSpan waitTime)
            : base(hWnd, waitTime)
        { }

        #region property

        TimeSpan SendMessageTimeout { get; } = TimeSpan.FromSeconds(1);
        int RetryCount { get; } = 10;
        TimeSpan RetryWait { get; } = TimeSpan.FromMilliseconds(100);

        #endregion

        #region WindowHandleCamera

        protected override Image TaskShotCore()
        {
            var message = WindowHandleUtility.RegisterWindowMessage(HtmlGetObjectWindowMessage);
            if(message == 0) {
                Logger.Warning($"${HtmlGetObjectWindowMessage} is 0");
                return null;
            }

            var timeoutResult = NativeMethods.SendMessageTimeout(WindowHandle, message, UIntPtr.Zero, IntPtr.Zero, SMTO.SMTO_ABORTIFHUNG, (uint)SendMessageTimeout.TotalMilliseconds, out UIntPtr sendMessageResult);
            if(timeoutResult == IntPtr.Zero) {
                Logger.Warning($"${nameof(NativeMethods.SendMessageTimeout)} is 0");
                return null;
            }

            var IID_IHTMLDocument3 = typeof(IHTMLDocument3).GUID;
            IHTMLDocument2 rawDocument = null;
            foreach(var counter in new Counter(RetryCount)) {
                try {
                    rawDocument = (IHTMLDocument2)WindowHandleUtility.ObjectFromLresult(sendMessageResult, IID_IHTMLDocument3, IntPtr.Zero);
                    break;
                } catch(COMException ex) {
                    Logger.Warning(ex);
                }
                if(!counter.IsLast) {
                    Logger.Trace($"threep: {RetryWait}");
                    Thread.Sleep(RetryWait);
                }
            }
            if(rawDocument == null) {
                Logger.Warning($"{nameof(IHTMLDocument2)} is null, {sendMessageResult}, {IID_IHTMLDocument3}");
                return null;
            }

            using(var htmlDocument = ComModel.Create(rawDocument)) {
                using(var serviceProvider = ComModel.Create((WindowHandleUtility.IServiceProvider)htmlDocument.Com.parentWindow)) {
                    var SID_SWebBrowserApp = typeof(IWebBrowserApp).GUID;
                    var IID_webBrowser = typeof(IWebBrowser2).GUID;
                    serviceProvider.Com.QueryService(ref SID_SWebBrowserApp, ref IID_webBrowser, out object queryResult);
                    var rawWebBrowser = (IWebBrowser2)queryResult;
                    if(rawWebBrowser == null) {
                        Logger.Warning($"{nameof(IWebBrowser2)} is null, {SID_SWebBrowserApp}, {IID_webBrowser}");
                        return null;
                    }

                    using(var webBrowser = ComModel.Create(rawWebBrowser)) {
                        // ばかにしてんのか
                        using(var document2 = ComModel.Create((IHTMLDocument2)webBrowser.Com.Document)) {
                            var body = ComModel.Create((IHTMLElement2)document2.Com.body);
                            Logger.Trace($"{body.Com.clientWidth} * {body.Com.clientHeight}");
                        }
                    }
                }

            }


            return base.TaskShotCore();
        }

        #endregion
    }
}
