using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.Library.PInvoke.Windows;
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
            var shotKeyOption = command.Option("--photo_opportunity_key", "shot", CommandOptionType.SingleValue);
            var selectKeyOption = command.Option("--wait_opportunity_key", "select", CommandOptionType.SingleValue);

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

            if(CaptureMode != CaptureMode.Screen && !IsContinuation) {
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
                    throw new ArgumentException("shot key: none!");
                }
                if(ShotKeys == SelectKeys) {
                    throw new ArgumentException("shot key: dup!");
                }
                if(ShotKeys.HasFlag(ExitKey) || SelectKeys.HasFlag(ExitKey)) {
                    throw new ArgumentException($"shot key: reserved {ExitKey}");
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

        Keys ShotKeys { get; set; } = Keys.None;
        Keys SelectKeys { get; set; } = Keys.None;

        CameramanForm Form { get; set; }

        IKeyboardMouseEvents HookEvents { get; set; }

        IntPtr TargetWindowHandle { get; set; }

        //TODO
        IReadOnlyList<string> IgnoreWindowClasses { get; } = new List<string>();
        #endregion

        #region function

        /// <summary>
        /// 撮影。
        /// </summary>
        /// <returns></returns>
        Image TakeShot()
        {
            if(CaptureMode == CaptureMode.Screen) {
                var camera = new ScreenCamera();
                return camera.TaskShot();
            } else {
                if(CaptureMode == CaptureMode.Scroll) {
                    var camera = new ScrollCamera(TargetWindowHandle);
                    return camera.TaskShot();
                } else {
                    var camera = new WindowHandleCamera(TargetWindowHandle, CaptureMode);
                    return camera.TaskShot();
                }
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
                var fileName = $"{DateTime.Now: yyyyMMdd_hhmmss}.png";
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

            Form.Opacity = 0;
            Form.Visible = true;
        }

        void EndViewSelect()
        {
            Logger.Debug("end selecting");
            Debug.Assert(NowSelecting);

            TargetWindowHandle = IntPtr.Zero;
            NowSelecting = false;

            Form.Visible = false;

            HookEvents.MouseMove -= HookEvents_MouseMove;
            HookEvents.MouseDown -= HookEvents_MouseDown;
        }

        void CaptureScreen()
        {
            var image = TakeShot();
            Develop(image);

            if(!IsContinuation) {
                Exit();
            }
        }

        void CaptureSelect()
        {
            Debug.Assert(TargetWindowHandle != IntPtr.Zero);

            var image = TakeShot();
            Develop(image);

            // 選択してたら選択終了
            if(NowSelecting) {
                EndViewSelect();
            }

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
            if(e.KeyCode == SelectKeys) {
                if(!NowSelecting) {
                    Logger.Information("select");

                    e.Handled = true;
                    StartViewSelect();
                }
            }
            if(e.KeyCode == ShotKeys) {
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
            if(e.KeyCode == ExitKey) {
                e.Handled = true;

                if(NowSelecting) {
                    Logger.Information("exit select");
                    EndViewSelect();
                } else {
                    Logger.Information("exit program");
                    Exit();
                }
            }
        }

        private void HookEvents_MouseMove(object sender, MouseEventArgs e)
        {
            TargetWindowHandle = IntPtr.Zero;

            var hWnd = WindowHandleUtility.GetView(e.Location, CaptureMode);
            if(Form.Handle == hWnd) {
                Form.Visible = false;
            }
            if(hWnd == IntPtr.Zero) {
                Form.Visible = false;
                return;
            }
            Form.Visible = true;
            Form.Opacity = 1;

            //TODO
            //IgnoreWindowClasses

            var rect = WindowHandleUtility.GetViewArea(hWnd, CaptureMode);
            // 枠用にサイズ補正
            rect.X -= Form.Padding.Left;
            rect.Y -= Form.Padding.Top;
            rect.Width += Form.Padding.Horizontal;
            rect.Height += Form.Padding.Vertical;

            Form.Location = rect.Location;
            Form.Size = rect.Size;

            TargetWindowHandle = hWnd;
        }

        private void HookEvents_MouseDown(object sender, MouseEventArgs e)
        {
            if(TargetWindowHandle != IntPtr.Zero) {
                CaptureSelect();
            }
        }

    }
}
