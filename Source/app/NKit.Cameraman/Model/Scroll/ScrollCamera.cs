using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Cameraman.Model.Scroll.InternetExplorer;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Cameraman.Model.Scroll
{
    public abstract class ScrollCameraBase : WindowHandleCamera
    {
        public ScrollCameraBase(IntPtr hWnd, TimeSpan delayTime)
            : base(hWnd, CaptureTarget.Client)
        {
            DelayTime = delayTime;
        }

        #region property

        protected TimeSpan DelayTime { get; }

        #endregion

        #region function

        /// <summary>
        /// 設定時間待機する。
        /// </summary>
        protected void Wait()
        {
            Thread.Sleep(DelayTime);
        }

        /// <summary>
        /// 指定した時間を設定時間から差し引いた時間だけ待機する
        /// </summary>
        /// <param name="minusTime"></param>
        protected void WaitMinus(TimeSpan minusTime)
        {
            // 設定時間より長い時間なら待つ必要なし
            if(DelayTime < minusTime) {
                return;
            }

            Thread.Sleep(DelayTime - minusTime);
        }

        #endregion
    }

    public sealed class ScrollCamera : ScrollCameraBase
    {
        #region define


        const string WindowClass_InternetExplorer_Frame = "IEFrame";
        const string WindowClass_InternetExplorer_Server = "Internet Explorer_Server";

        enum ScrollWindowKind
        {
            Unknown,
            InternetExplorer,
        }

        #endregion

        public ScrollCamera(IntPtr hWnd, TimeSpan delayTime)
            : base(hWnd, delayTime)
        { }

        #region property

        /// <summary>
        /// 実際に使用するウィンドウハンドル。
        /// <para>対象によっては渡されたウィンドウハンドルからさらに冒険するがあるためこちらを使用すること。</para>
        /// </summary>
        IntPtr TargetWindowHandle { get; set; }

        public TimeSpan ScrollInternetExplorerInitializeTime { get; set; }
        public bool ScrollInternetExplorerHideFixedHeader { get; set; }
        public string ScrollInternetExplorerHideFixedHeaderElements { get; set; }
        public bool ScrollInternetExplorerHideFixedFooter { get; set; }
        public string ScrollInternetExplorerHideFixedFooterElements { get; set; }

        #endregion

        #region function

        ScrollWindowKind GetScrollWindowKind()
        {
            var windowClassName = WindowHandleUtility.GetWindowClassName(WindowHandle);
            Logger.Debug($"window class name: {windowClassName}");

            // IE 用判定処理
            if(windowClassName == WindowClass_InternetExplorer_Frame) {
                Logger.Debug("search ieframe start");
                // IFrame なら中を探検していく
                var findWindowHandle = IntPtr.Zero;
                var result = NativeMethods.EnumChildWindows(
                    WindowHandle,
                    (hWnd, lParam) => {
                        var childWindowClassName = WindowHandleUtility.GetWindowClassName(hWnd);
                        Logger.Debug($"search ieframe: {childWindowClassName}");
                        if(childWindowClassName == WindowClass_InternetExplorer_Server) {
                            Logger.Trace("おったどー");
                            findWindowHandle = hWnd;
                            return false;
                        }
                        return true;
                    },
                    IntPtr.Zero
                );
                if(findWindowHandle != IntPtr.Zero) {
                    Logger.Debug($"handle: {WindowHandle} -> {findWindowHandle}");
                    TargetWindowHandle = findWindowHandle;
                    return ScrollWindowKind.InternetExplorer;
                }
            }
            if(windowClassName == WindowClass_InternetExplorer_Server) {
                TargetWindowHandle = WindowHandle;
                return ScrollWindowKind.InternetExplorer;
            }


            return ScrollWindowKind.Unknown;
        }

        Image TaskShotInternetExplorer()
        {
            using(var camera = new InternetExplorerScrollCamera(TargetWindowHandle, DelayTime) {
                SendMessageWaitTime = ScrollInternetExplorerInitializeTime,
                DocumentWaitTime = ScrollInternetExplorerInitializeTime,

                HideFixedHeader = ScrollInternetExplorerHideFixedHeader,
                HideFixedHeaderElements = ScrollInternetExplorerHideFixedHeaderElements,

                HideFixedFooter = ScrollInternetExplorerHideFixedFooter,
                HideFixedFooterElements = ScrollInternetExplorerHideFixedFooterElements,
            }) {
                return camera.TakeShot();
            }
        }

        #endregion

        #region WindowHandleCamera

        protected override Image TakeShotCore()
        {
            // 自身がスクロール可能なウィンドウか調査
            var kind = GetScrollWindowKind();

            switch(kind) {
                case ScrollWindowKind.Unknown:
                    return base.TakeShotCore();

                case ScrollWindowKind.InternetExplorer:
                    return TaskShotInternetExplorer() ?? base.TakeShotCore();

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
