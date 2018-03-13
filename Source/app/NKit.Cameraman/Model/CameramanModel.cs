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

        Image TakeShot()
        {
            if(CaptureMode == CaptureMode.Screen && !IsContinuation) {
                var screenCamera = new ScreenCamera();
                return screenCamera.TaskShot();
            }

            throw new NotImplementedException();
        }

        void Exit()
        {

        }

        public void Execute(CameramanForm form)
        {
            if(CaptureMode == CaptureMode.Screen) {
                TakeShot();
            } else {
                form.ShowDialog();
            }
        }

        #endregion
    }
}
