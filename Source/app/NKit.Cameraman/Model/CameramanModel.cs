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
            var shotKeyOption = command.Option("--photo_opportunity_key", "shot", CommandOptionType.SingleValue);
            var selectKeyOption = command.Option("--wait_opportunity_key", "select", CommandOptionType.SingleValue);

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
            }

        }

        #region property

        CaptureMode CaptureMode { get; }

        bool IsEnabledScroll { get; }
        bool IsEnabledClipboard { get; }

        DirectoryInfo SaveDirectory { get; }

        EventWaitHandle SaveNoticeEvent { get; }
        EventWaitHandle ExitNoticeEvent { get; }

        bool IsContinuation { get; }

        Keys ShotKeys { get; set; }
        Keys SelectKeys { get; set; }

        CameramanForm Form { get; set; }
        #endregion

        #region function

        /// <summary>
        /// 撮影。
        /// </summary>
        /// <returns></returns>
        Image TakeShot()
        {
            if(CaptureMode == CaptureMode.Screen) {
                var screenCamera = new ScreenCamera();
                return screenCamera.TaskShot();
            } else {
                Debug.Assert(CaptureMode == CaptureMode.Target);
                Form.ShowDialog();
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
            if(CaptureMode == CaptureMode.Screen) {
                var image = TakeShot();
                Develop(image);
            } else {
                Form = form;
                var image = TakeShot();
                Develop(image);
            }
        }

        void Exit()
        {

        }

        #endregion
    }
}
