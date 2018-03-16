using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Cameraman.Model.Scroll;
using ContentTypeTextNet.NKit.Cameraman.View;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class CameramanModel : ModelBase
    {
        public CameramanModel(string[] arguments)
        {
            Bag = new CameramanBag(arguments);
        }

        #region property

        public bool NowSelecting { get; private set; }

        public CameramanBag Bag { get; }

        InformationForm CameramanForm { get; set; }

        IKeyboardMouseEvents HookEvents { get; set; }

        IntPtr TargetWindowHandle { get; set; }

        #endregion

        #region function

        CameraBase GetCammera(IntPtr hWnd)
        {
            if(Bag.CaptureMode == CaptureMode.Screen) {
                return new ScreenCamera();
            }

            Debug.Assert(hWnd != IntPtr.Zero);
            if(Bag.CaptureMode == CaptureMode.Scroll) {
                return new ScrollCamera(hWnd, Bag.ScrollDelayTime) {
                    ScrollInternetExplorerInitializeTime = Bag.ScrollInternetExplorerInitializeTime,

                    ScrollInternetExplorerHideFixedHeader = Bag.ScrollInternetExplorerHideFixedHeader,
                    ScrollInternetExplorerHideFixedHeaderElements = Bag.ScrollInternetExplorerHideFixedHeaderElements,

                    ScrollInternetExplorerHideFixedFooter = Bag.ScrollInternetExplorerHideFixedFooter,
                    ScrollInternetExplorerHideFixedFooterElements = Bag.ScrollInternetExplorerHideFixedFooterElements,
                };
            }

            return new WindowHandleCamera(hWnd, Bag.CaptureMode);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 撮影。
        /// </summary>
        /// <returns></returns>
        Image TakeShot(IntPtr hWnd)
        {
            using(var cammera = GetCammera(hWnd)) {
                return cammera.TakeShot();
            }
        }

        /// <summary>
        /// 現像。
        /// </summary>
        /// <param name="image"></param>
        void Develop(Image image)
        {
            if(Bag.IsEnabledClipboard) {
                Clipboard.SetImage(image);
            }
            if(Bag.SaveDirectory != null) {
                var fileName = $"{DateTime.Now: yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(Bag.SaveDirectory.FullName, fileName);
                image.Save(filePath, ImageFormat.Png);
            }

            if(Bag.SaveNoticeEvent != null) {
                Bag.SaveNoticeEvent.Set();
            }
        }

        public void Execute(InformationForm form)
        {
            if(Bag.CaptureMode == CaptureMode.Screen && !Bag.IsContinuation) {
                CaptureScreen();
                Exit();
            } else {
                HookEvents = Hook.GlobalEvents();

                if(Bag.ShotKeys != Keys.None || Bag.SelectKeys != Keys.None) {
                    HookKeyboardInput();
                }

                if(Bag.CaptureMode == CaptureMode.Screen) {
                    Application.Run();
                } else {
                    CameramanForm = form;
                    CameramanForm.Shown += Form_Shown;
                    Application.Run(CameramanForm);
                }

            }
        }

        public void StartSelectView()
        {
            Logger.Debug("start selecting");
            Debug.Assert(!NowSelecting);

            NowSelecting = true;

            HookMouseInput();

            //Form.Opacity = 0;
            //Form.Visible = true;

            SelectViewAndFocus(Cursor.Position);
        }

        void EndSelectView()
        {
            Logger.Debug("end selecting");
            Debug.Assert(NowSelecting);

            TargetWindowHandle = IntPtr.Zero;
            NowSelecting = false;

            CameramanForm.Detach();

            UnhookMouseInput();
        }

        void CaptureScreen()
        {
            using(var image = TakeShot(TargetWindowHandle)) {
                Develop(image);
            }
        }

        void CaptureSelect()
        {
            Debug.Assert(TargetWindowHandle != IntPtr.Zero);

            // 選択してたら選択終了するんでウィンドウハンドル消えちゃうので退避
            var hWnd = TargetWindowHandle;
            if(NowSelecting) {
                EndSelectView();
            }

            using(var image = TakeShot(hWnd)) {
                Develop(image);
            }
        }

        /// <summary>
        /// フック中にキャプチャする処理。
        /// </summary>
        /// <param name="captureAction"></param>
        /// <returns></returns>
        void CaptureInHooking(Action captureAction)
        {
            UnhookMouseInput();
            UnhookKeyboardInput();

            HookEvents.Dispose();

            Thread.Sleep(Bag.ShotDelayTime);
            captureAction();

            if(Bag.IsContinuation) {
                // 継続するなら前提条件としてキー入力が可能となっているのでキーフック開始
                HookEvents = Hook.GlobalEvents();
                HookKeyboardInput();

                CameramanForm.ShowNavigation();
            } else {
                Exit();
            }
        }

        public void Exit()
        {
            Logger.Information("ばいばい");

            if(HookEvents != null) {
                UnhookMouseInput();
                UnhookKeyboardInput();

                HookEvents.Dispose();
                HookEvents = null;
            }

            if(Bag.ExitNoticeEvent != null) {
                Bag.ExitNoticeEvent.Set();
            }

            Application.Exit();
        }

        void SelectViewAndFocus(Point mousePoint)
        {
            var hWnd = WindowHandleUtility.GetView(mousePoint, Bag.CaptureMode);

            if(hWnd == IntPtr.Zero) {
                CameramanForm.Detach();
                return;
            }

            if(CameramanForm.IsSelfHandle(hWnd)) {
                // 選ばれたのが自分とその関係者で前回選択ウィンドウが有効なら別段何もしない
                if(TargetWindowHandle != IntPtr.Zero) {
                    return;
                }

                // 自分じゃないし死んどく
                CameramanForm.Detach();
                return;
            }

            var rect = WindowHandleUtility.GetViewArea(hWnd, Bag.CaptureMode);
            // 枠用にサイズ補正
            CameramanForm.Attach(hWnd, rect);

            TargetWindowHandle = hWnd;
        }

        bool CheckInputKey(KeyEventArgs e, Keys key)
        {
            var modMask = (Keys.Control | Keys.Shift | Keys.Alt);

            var normalKey = key & ~modMask;
            var modKeys = key & modMask;

            if(normalKey == Keys.None && modKeys != Keys.None) {
                // 装飾キーだけの可能性
                var result = true;
                if(modKeys.HasFlag(Keys.Control)) {
                    result &= e.Control || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey;
                }
                if(modKeys.HasFlag(Keys.Shift)) {
                    result &= e.Shift || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey;
                }
                if(modKeys.HasFlag(Keys.Alt)) {
                    result &= e.Alt || e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu;
                }
                return result;
            }

            if(e.KeyCode == normalKey) {
                // 装飾キーなし
                if(modKeys == Keys.None) {
                    return true;
                }

                var result = true;
                // 装飾キー確認
                if(modKeys.HasFlag(Keys.Shift)) {
                    result &= e.Shift;
                }
                if(modKeys.HasFlag(Keys.Control)) {
                    result &= e.Control;
                }
                if(modKeys.HasFlag(Keys.Alt)) {
                    result &= e.Alt;
                }

                return result;
            }

            return false;
        }

        void HookKeyboardInput()
        {
            Logger.Debug("hook: ket input");
            HookEvents.KeyDown += HookEvents_KeyDown;
        }

        void UnhookKeyboardInput()
        {
            Logger.Debug("unhook: key input");
            HookEvents.KeyDown -= HookEvents_KeyDown;
        }

        void HookMouseInput()
        {
            Logger.Debug("hook: mouse input");
            HookEvents.MouseMove += HookEvents_MouseMove;
            HookEvents.MouseDown += HookEvents_MouseDown;
        }

        void UnhookMouseInput()
        {
            Logger.Debug("unhook: mouse input");
            HookEvents.MouseMove -= HookEvents_MouseMove;
            HookEvents.MouseDown -= HookEvents_MouseDown;
        }

        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(HookEvents != null) {
                        UnhookMouseInput();
                        UnhookKeyboardInput();

                        HookEvents.Dispose();
                        HookEvents = null;
                    }
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        private void HookEvents_KeyDown(object sender, KeyEventArgs e)
        {
            // 選択処理
            if(CheckInputKey(e, Bag.SelectKeys)) {
                if(!NowSelecting) {
                    Logger.Information("select");

                    e.Handled = true;
                    StartSelectView();
                } else {
                    Logger.Information("select shot!");
                    if(TargetWindowHandle != IntPtr.Zero) {
                        CaptureInHooking(CaptureSelect);
                    }
                    e.Handled = true;
                }
            }

            // キャプチャ処理
            if(CheckInputKey(e, Bag.ShotKeys)) {
                Logger.Information("shot");
                if(Bag.CaptureMode == CaptureMode.Screen) {
                    CaptureInHooking(CaptureScreen);
                } else {
                    if(NowSelecting) {
                        if(TargetWindowHandle != IntPtr.Zero) {
                            CaptureInHooking(CaptureSelect);
                        } else {
                            Logger.Warning("window handle: no select");
                        }
                    } else {
                        // 今時点でアクティブなやつからキャプチャ
                        var hWnd = WindowHandleUtility.GetActiveWindow(Bag.CaptureMode);
                        if(hWnd != IntPtr.Zero) {
                            Logger.Debug("window handle!");
                            TargetWindowHandle = hWnd;
                            CaptureInHooking(CaptureSelect);
                        } else {
                            Logger.Debug("window handle is null");
                        }
                    }
                }
            }

            // 終了処理
            if(CheckInputKey(e,Bag.ExitKey)) {
                e.Handled = true;

                if(NowSelecting) {
                    Logger.Information("exit select");
                    EndSelectView();

                    // 即時起動で継続使用しないのであれば選択待機よりは終了
                    if(Bag.ImmediatelySelect && !Bag.IsContinuation) {
                        Logger.Information("exit program ^_^");
                        Exit();
                    }
                } else {
                    Logger.Information("exit program");
                    Exit();
                }
            }
        }

        private void HookEvents_MouseMove(object sender, MouseEventArgs e)
        {
            SelectViewAndFocus(e.Location);
        }

        private void HookEvents_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left) {
                if(TargetWindowHandle != IntPtr.Zero) {
                    CaptureInHooking(CaptureSelect);
                }
            }
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            CameramanForm.Shown -= Form_Shown;
            if(Bag.ImmediatelySelect) {
                StartSelectView();
            } else {
                CameramanForm.ShowNavigation();
            }
        }

    }
}
