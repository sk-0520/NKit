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
        #region define

        static Keys ExitKey { get; } = Keys.Escape;

        #endregion

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


        bool NowSelecting { get; set; }

        CaptureMode CaptureMode { get; }

        bool IsEnabledClipboard { get; }

        DirectoryInfo SaveDirectory { get; }

        EventWaitHandle SaveNoticeEvent { get; }
        EventWaitHandle ExitNoticeEvent { get; }

        bool IsContinuation { get; }
        bool ImmediatelySelect { get; }

        Keys ShotKeys { get; } = Keys.None;
        Keys SelectKeys { get; } = Keys.None;

        public Color BorderColor { get; } = Constants.CameraBorderColor;
        public int BorderWidth { get; } = Constants.CameraBorderWidth;

        TimeSpan ScrollDelayTime { get; } = Constants.ScrollDelayTime;
        TimeSpan ScrollInternetExplorerInitializeTime { get; } = Constants.ScrollInternetExplorerInitializeTime;


        CameramanForm Form { get; set; }

        IKeyboardMouseEvents HookEvents { get; set; }

        IntPtr TargetWindowHandle { get; set; }

        #endregion

        #region function

        /// <summary>
        /// 撮影。
        /// </summary>
        /// <returns></returns>
        Image TakeShot(IntPtr hWnd)
        {
            if(CaptureMode == CaptureMode.Screen) {
                var camera = new ScreenCamera();
                return camera.TaskShot();
            } else if(hWnd != IntPtr.Zero) {
                if(CaptureMode == CaptureMode.Scroll) {
                    var camera = new ScrollCamera(hWnd, ScrollDelayTime) {
                        ScrollInternetExplorerInitializeTime = ScrollInternetExplorerInitializeTime,
                    };
                    return camera.TaskShot();
                } else {
                    var camera = new WindowHandleCamera(hWnd, CaptureMode);
                    return camera.TaskShot();
                }
            }

            throw new NotImplementedException();
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

        public void Execute(CameramanForm form)
        {
            if(CaptureMode == CaptureMode.Screen && !IsContinuation) {
                CaptureScreen();
            } else {
                HookEvents = Hook.GlobalEvents();

                if(ShotKeys != Keys.None || SelectKeys != Keys.None) {
                    HookEvents.KeyDown += HookEvents_KeyDown;
                }

                if(CaptureMode == CaptureMode.Screen) {
                    Application.Run();
                } else {
                    Form = form;
                    if(ImmediatelySelect) {
                        Form.Shown += Form_Shown;
                    }
                    Application.Run(Form);
                }

            }
        }

        void StartViewSelect()
        {
            Logger.Debug("start selecting");
            Debug.Assert(!NowSelecting);

            NowSelecting = true;

            HookEvents.MouseMove += HookEvents_MouseMove;
            HookEvents.MouseDown += HookEvents_MouseDown;

            //Form.Opacity = 0;
            //Form.Visible = true;

            SelectViewAndFocus(Cursor.Position);
        }

        void EndViewSelect()
        {
            Logger.Debug("end selecting");
            Debug.Assert(NowSelecting);

            TargetWindowHandle = IntPtr.Zero;
            NowSelecting = false;

            Form.HideStatus();

            HookEvents.MouseMove -= HookEvents_MouseMove;
            HookEvents.MouseDown -= HookEvents_MouseDown;
        }

        void CaptureScreen()
        {
            var image = TakeShot(TargetWindowHandle);
            Develop(image);

            if(!IsContinuation) {
                Exit();
            }
        }

        void CaptureSelect()
        {
            Debug.Assert(TargetWindowHandle != IntPtr.Zero);

            // 選択してたら選択終了するんでウィンドウハンドル消えちゃうので退避
            var hWnd = TargetWindowHandle;
            if(NowSelecting) {
                EndViewSelect();
            }

            var image = TakeShot(hWnd);
            Develop(image);


            if(!IsContinuation) {
                Exit();
            }
        }

        void Exit()
        {
            Logger.Information("ばいばい");

            if(HookEvents != null) {
                HookEvents.KeyDown -= HookEvents_KeyDown;
                HookEvents.MouseMove -= HookEvents_MouseMove;
                HookEvents.MouseDown -= HookEvents_MouseDown;
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
                Form.HideStatus();
                return;
            }

            if(Form.IsSelfHandle(hWnd)) {
                // 選ばれたのが自分とその関係者で前回選択ウィンドウが有効なら別段何もしない
                if(TargetWindowHandle != IntPtr.Zero) {
                    return;
                }

                // 自分じゃないし死んどく
                Form.HideStatus();
                return;
            }

            var rect = WindowHandleUtility.GetViewArea(hWnd, CaptureMode);
            // 枠用にサイズ補正
            Form.ShowStatus(hWnd, rect);

            TargetWindowHandle = hWnd;
        }

        bool CheckKey(KeyEventArgs e, Keys key)
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


        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(HookEvents != null) {
                        HookEvents.KeyDown -= HookEvents_KeyDown;
                        HookEvents.MouseMove -= HookEvents_MouseMove;
                        HookEvents.MouseDown -= HookEvents_MouseDown;

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
            if(CheckKey(e, SelectKeys)) {
                if(!NowSelecting) {
                    Logger.Information("select");

                    e.Handled = true;
                    StartViewSelect();
                } else {
                    Logger.Information("select shot!");
                    if(TargetWindowHandle != IntPtr.Zero) {
                        CaptureSelect();
                    }
                    e.Handled = true;
                }
            }

            // キャプチャ処理
            if(CheckKey(e, ShotKeys)) {
                Logger.Information("shot");
                if(CaptureMode == CaptureMode.Screen) {
                    CaptureScreen();
                } else {
                    if(NowSelecting) {
                        if(TargetWindowHandle != IntPtr.Zero) {
                            CaptureSelect();
                        } else {
                            Logger.Warning("window handle: no select");
                        }
                    } else {
                        // 今時点でアクティブなやつからキャプチャ
                        var hWnd = WindowHandleUtility.GetActiveWindow(CaptureMode);
                        if(hWnd != IntPtr.Zero) {
                            Logger.Debug("window handle!");
                            TargetWindowHandle = hWnd;
                            CaptureSelect();
                        } else {
                            Logger.Debug("window handle is null");
                        }
                    }
                }
            }

            // 終了処理
            if(CheckKey(e, ExitKey)) {
                e.Handled = true;

                if(NowSelecting) {
                    Logger.Information("exit select");
                    EndViewSelect();

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
            if(TargetWindowHandle != IntPtr.Zero) {
                CaptureSelect();
            }
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            Form.Shown -= Form_Shown;
            StartViewSelect();
        }

    }
}
