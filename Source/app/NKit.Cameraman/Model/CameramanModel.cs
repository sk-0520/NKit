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
            var command = new CommandLineApplication(false);

            var modeOption = command.Option("--mode", "mode", CommandOptionType.SingleValue);
            var clipboardOption = command.Option("--clipboard", "use clipboard", CommandOptionType.NoValue);
            var saveDirOption = command.Option("--save_directory", "save directory", CommandOptionType.SingleValue);
            var saveEventOption = command.Option("--save_event_name", "save event", CommandOptionType.SingleValue);
            var exitEventOption = command.Option("--exit_event_name", "exit event, pair --save_event_name", CommandOptionType.SingleValue);
            var continuationOption = command.Option("--continuation", "one/continuation", CommandOptionType.NoValue);
            var immediatelySelectOption = command.Option("--immediately_select", "start select", CommandOptionType.NoValue);
            var shotKeyOption = command.Option("--photo_opportunity_key", $"shot normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var selectKeyOption = command.Option("--wait_opportunity_key", $"select normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var shotDelayTimeOption = command.Option("--photo_opportunity_delay_time", "shot deilay time", CommandOptionType.SingleValue);
            var cameraBorderColorOption = command.Option("--camera_border_color", "color", CommandOptionType.SingleValue);
            var cameraBorderWidthOption = command.Option("--camera_border_width", "color", CommandOptionType.SingleValue);
            var scrollDelayTimeOption = command.Option("--scroll_delay_time", "color", CommandOptionType.SingleValue);
            var scrollIeInitializeTimeOption = command.Option("--scroll_ie_init_time", "color", CommandOptionType.SingleValue);

            command.Execute(arguments);

            CaptureMode = EnumUtility.Parse<CaptureMode>(modeOption.Value());
            IsEnabledClipboard = clipboardOption.HasValue();
            if(saveDirOption.HasValue()) {
                SaveDirectory = new DirectoryInfo(saveDirOption.Value());
            }
            if(saveEventOption.HasValue()) {
                if(!exitEventOption.HasValue()) {
                    throw new ArgumentException("--save_event_name, --exit_event_name");
                }
                SaveNoticeEvent = EventWaitHandle.OpenExisting(saveEventOption.Value());
                ExitNoticeEvent = EventWaitHandle.OpenExisting(exitEventOption.Value());
            }
            IsContinuation = continuationOption.HasValue();
            ImmediatelySelect = immediatelySelectOption.HasValue();

            var needKey = true;
            if(CaptureMode == CaptureMode.Screen && !IsContinuation) {
                needKey = false;
            }

            if(needKey) {
                if(!shotKeyOption.HasValue() && !selectKeyOption.HasValue()) {
                    throw new ArgumentException("shot key!");
                }

                if(shotKeyOption.HasValue()) {
                    ShotKeys = EnumUtility.Parse<Keys>(shotKeyOption.Value());
                }
                if(selectKeyOption.HasValue()) {
                    SelectKeys = EnumUtility.Parse<Keys>(selectKeyOption.Value());
                }
                if(ShotKeys == Keys.None && SelectKeys == Keys.None) {
                    throw new ArgumentException("shot/select key: none!");
                }
                if(ShotKeys == SelectKeys) {
                    throw new ArgumentException("shot key: dup!");
                }
                if(ShotKeys.HasFlag(ExitKey) || SelectKeys.HasFlag(ExitKey)) {
                    throw new ArgumentException($"shot key: reserved {ExitKey}");
                }
            }

            if(shotDelayTimeOption.HasValue()) {
                ShotDelayTime = TimeSpan.Parse(shotDelayTimeOption.Value());
            }

            if(cameraBorderColorOption.HasValue()) {
                BorderColor = ColorTranslator.FromHtml(cameraBorderColorOption.Value());
            }
            if(cameraBorderWidthOption.HasValue()) {
                BorderWidth = int.Parse(cameraBorderWidthOption.Value());
            }

            if(CaptureMode == CaptureMode.Scroll) {
                if(scrollDelayTimeOption.HasValue()) {
                    ScrollDelayTime = TimeSpan.Parse(scrollDelayTimeOption.Value());
                }
                if(scrollIeInitializeTimeOption.HasValue()) {
                    ScrollInternetExplorerInitializeTime = TimeSpan.Parse(scrollIeInitializeTimeOption.Value());
                }
            }

        }

        #region property

        public Keys ExitKey { get; } = Keys.Escape;

        bool NowSelecting { get; set; }

        CaptureMode CaptureMode { get; }

        bool IsEnabledClipboard { get; }

        DirectoryInfo SaveDirectory { get; }

        EventWaitHandle SaveNoticeEvent { get; }
        EventWaitHandle ExitNoticeEvent { get; }

        public bool IsContinuation { get; }
        public bool ImmediatelySelect { get; }

        public Keys ShotKeys { get; } = Keys.None;
        public Keys SelectKeys { get; } = Keys.None;

        TimeSpan ShotDelayTime { get; } = Constants.ShotDelayTime;

        public Color BorderColor { get; } = Constants.CameraBorderColor;
        public int BorderWidth { get; } = Constants.CameraBorderWidth;

        TimeSpan ScrollDelayTime { get; } = Constants.ScrollDelayTime;
        TimeSpan ScrollInternetExplorerInitializeTime { get; } = Constants.ScrollInternetExplorerInitializeTime;


        InformationForm CameramanForm { get; set; }

        IKeyboardMouseEvents HookEvents { get; set; }

        IntPtr TargetWindowHandle { get; set; }

        #endregion

        #region function

        CameraBase GetCammera(IntPtr hWnd)
        {
            if(CaptureMode == CaptureMode.Screen) {
                return new ScreenCamera();
            }

            Debug.Assert(hWnd != IntPtr.Zero);
            if(CaptureMode == CaptureMode.Scroll) {
                return new ScrollCamera(hWnd, ScrollDelayTime) {
                    ScrollInternetExplorerInitializeTime = ScrollInternetExplorerInitializeTime,
                };
            }

            return new WindowHandleCamera(hWnd, CaptureMode);

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
            if(IsEnabledClipboard) {
                Clipboard.SetImage(image);
            }
            if(SaveDirectory != null) {
                var fileName = $"{DateTime.Now: yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(SaveDirectory.FullName, fileName);
                image.Save(filePath, ImageFormat.Png);
            }

            if(SaveNoticeEvent != null) {
                SaveNoticeEvent.Set();
            }
        }

        public void Execute(InformationForm form)
        {
            if(CaptureMode == CaptureMode.Screen && !IsContinuation) {
                CaptureScreen();
                Exit();
            } else {
                HookEvents = Hook.GlobalEvents();

                if(ShotKeys != Keys.None || SelectKeys != Keys.None) {
                    HookKeyboardInput();
                }

                if(CaptureMode == CaptureMode.Screen) {
                    Application.Run();
                } else {
                    CameramanForm = form;
                    CameramanForm.Shown += Form_Shown;
                    Application.Run(CameramanForm);
                }

            }
        }

        void StartSelectView()
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

            Thread.Sleep(ShotDelayTime);
            captureAction();

            if(IsContinuation) {
                // 継続するなら前提条件としてキー入力が可能となっているのでキーフック開始
                HookEvents = Hook.GlobalEvents();
                HookKeyboardInput();

                CameramanForm.ShowNavigation();
            } else {
                Exit();
            }
        }

        void Exit()
        {
            Logger.Information("ばいばい");

            if(HookEvents != null) {
                UnhookMouseInput();
                UnhookKeyboardInput();

                HookEvents.Dispose();
                HookEvents = null;
            }

            if(ExitNoticeEvent != null) {
                ExitNoticeEvent.Set();
            }

            Application.Exit();
        }

        void SelectViewAndFocus(Point mousePoint)
        {
            var hWnd = WindowHandleUtility.GetView(mousePoint, CaptureMode);

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

            var rect = WindowHandleUtility.GetViewArea(hWnd, CaptureMode);
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
            if(CheckInputKey(e, SelectKeys)) {
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
            if(CheckInputKey(e, ShotKeys)) {
                Logger.Information("shot");
                if(CaptureMode == CaptureMode.Screen) {
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
                        var hWnd = WindowHandleUtility.GetActiveWindow(CaptureMode);
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
            if(CheckInputKey(e, ExitKey)) {
                e.Handled = true;

                if(NowSelecting) {
                    Logger.Information("exit select");
                    EndSelectView();

                    // 即時起動で継続使用しないのであれば選択待機よりは終了
                    if(ImmediatelySelect && !IsContinuation) {
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
            if(ImmediatelySelect) {
                StartSelectView();
            } else {
                CameramanForm.ShowNavigation();
            }
        }

    }
}
