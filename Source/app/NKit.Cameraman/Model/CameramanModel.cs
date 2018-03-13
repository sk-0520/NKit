using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Cameraman.View;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class CameramanModel : ModelBase
    {
        public CameramanModel(string[] arguments)
        {
            var command = new CommandLineApplication(false);

            var modeOption = command.Option("--mode", "mode", CommandOptionType.SingleValue);
            var scrollOption = command.Option("--scroll", "mode = target, scroll capture", CommandOptionType.NoValue);
            var clipboardOption = command.Option("--clipboard", "use clipboard", CommandOptionType.NoValue);
            var saveDirOption = command.Option("--save_directory", "save directory", CommandOptionType.SingleValue);
            var saveEventOption = command.Option("--save_event_name", "save event", CommandOptionType.SingleValue);
            var exitEventOption = command.Option("--exit_event_name", "exit event, pair --save_event_name", CommandOptionType.SingleValue);
            var continuationOption = command.Option("--continuation", "one/continuation", CommandOptionType.NoValue);

            command.Execute(arguments);

            CaptureMode = EnumUtility.Parse<CaptureMode>(modeOption.Value());
            IsEnabledScroll = scrollOption.HasValue();
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
        }

        #region property

        CaptureMode CaptureMode { get; }

        bool IsEnabledScroll { get; }
        bool IsEnabledClipboard { get; }

        DirectoryInfo SaveDirectory { get; }

        EventWaitHandle SaveNoticeEvent { get; }
        EventWaitHandle ExitNoticeEvent { get; }

        bool IsContinuation { get; }

        #endregion

        #region function

        void TakeScreen()
        {
            var allScreens = Screen.AllScreens;

            var minLeft = allScreens.Min(s => s.Bounds.Left);
            var minTop = allScreens.Min(s => s.Bounds.Top);
            var maxRight = allScreens.Max(s => s.Bounds.Right);
            var maxBottom = allScreens.Max(s => s.Bounds.Bottom);

            // 原点が(0,0)の左上座標へ補正してあげる
            var addX = minLeft < 0 ? -minLeft : 0;
            var addY = minTop < 0 ? -minTop : 0;

            var width = maxRight + addX;
            var height = maxBottom + addY;

            using(var bitmap = new Bitmap(width, height)) {
                using(var g = Graphics.FromImage(bitmap)) {
                    foreach(var screen in allScreens) {
                        var screenBounds = screen.Bounds;
                        var srcPoint = new Point(screenBounds.X, screenBounds.Y);
                        var dstPoint = new Point(screenBounds.X + addX, screenBounds.Y + addY);
                        var dstSize = new Size(screenBounds.Width, screenBounds.Height);
                        g.CopyFromScreen(srcPoint, dstPoint, dstSize);
                    }
                }
                bitmap.Save(@"Z:\cap.png");
            }

        }

        void Exit()
        {

        }

        public void Execute(CameramanForm form)
        {
            if(CaptureMode == CaptureMode.Screen && !IsContinuation) {
                // ただのスクリーンキャプチャならその場で死ぬべし
                TakeScreen();
                Exit();
                return;
            }

            form.ShowDialog();
        }

        #endregion
    }
}
