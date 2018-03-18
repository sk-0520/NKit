using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.Extensions.CommandLineUtils;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class CameramanBag
    {
        public CameramanBag(string[] arguments)
        {
            var command = new CommandLineApplication(false);

            var modeOption = command.Option("--mode", "mode", CommandOptionType.SingleValue);
            var clipboardOption = command.Option("--clipboard", "use clipboard", CommandOptionType.NoValue);
            var saveDirOption = command.Option("--save_directory", "save directory", CommandOptionType.SingleValue);
            var saveFileNameFormatOption = command.Option("--save_file_name_format", "save file name format, extension is ${EXT}", CommandOptionType.SingleValue);
            var saveImageKindOption = command.Option("--save_image_kind", "png", CommandOptionType.SingleValue);
            var saveEventOption = command.Option("--save_event_name", "save event", CommandOptionType.SingleValue);
            var continuationOption = command.Option("--continuation", "one/continuation", CommandOptionType.NoValue);
            var immediatelySelectOption = command.Option("--immediately_select", "start select", CommandOptionType.NoValue);
            var shotKeyOption = command.Option("--photo_opportunity_key", $"shot normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var selectKeyOption = command.Option("--wait_opportunity_key", $"select normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var shotDelayTimeOption = command.Option("--photo_opportunity_delay_time", "shot deilay time", CommandOptionType.SingleValue);
            var cameraBorderColorOption = command.Option("--camera_border_color", "color", CommandOptionType.SingleValue);
            var cameraBorderWidthOption = command.Option("--camera_border_width", "color", CommandOptionType.SingleValue);
            var scrollDelayTimeOption = command.Option("--scroll_delay_time", "color", CommandOptionType.SingleValue);
            var scrollIeInitializeTimeOption = command.Option("--scroll_ie_init_time", "color", CommandOptionType.SingleValue);
            var scrollIeHideFixedHeader = command.Option("--scroll_ie_hide_header", "value * is default", CommandOptionType.SingleValue);
            var scrollIeHideFixedFooter = command.Option("--scroll_ie_hide_footer", "value * is default", CommandOptionType.SingleValue);

            command.Execute(arguments);

            CaptureMode = EnumUtility.Parse<Setting.Define.CaptureMode>(modeOption.Value());
            IsEnabledClipboard = clipboardOption.HasValue();
            if(saveDirOption.HasValue()) {
                SaveDirectory = new DirectoryInfo(saveDirOption.Value());

                if(!saveFileNameFormatOption.HasValue()) {
                    throw new ArgumentException("--save_directory, --save_file_name_format");
                }
                SaveFileNameFormat = saveFileNameFormatOption.Value();

                if(saveImageKindOption.HasValue()) {
                    SaveImageKind = EnumUtility.Parse<ImageKind>(saveImageKindOption.Value());
                }
            }
            if(saveEventOption.HasValue()) {
                SaveNoticeEvent = EventWaitHandle.OpenExisting(saveEventOption.Value());
            }
            IsContinuation = continuationOption.HasValue();
            ImmediatelySelect = immediatelySelectOption.HasValue();

            var needKey = true;
            if(CaptureMode == Setting.Define.CaptureMode.Screen && !IsContinuation) {
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

            if(CaptureMode == Setting.Define.CaptureMode.Scroll) {
                if(scrollDelayTimeOption.HasValue()) {
                    ScrollDelayTime = TimeSpan.Parse(scrollDelayTimeOption.Value());
                }
                if(scrollIeInitializeTimeOption.HasValue()) {
                    ScrollInternetExplorerInitializeTime = TimeSpan.Parse(scrollIeInitializeTimeOption.Value());
                }
            }

            if(scrollIeHideFixedHeader.HasValue()) {
                ScrollInternetExplorerHideFixedHeader = true;
                ScrollInternetExplorerHideFixedHeaderElements = scrollIeHideFixedHeader.Value().Replace("*", Constants.HideHeaderTagClassItems);
            }
            if(scrollIeHideFixedFooter.HasValue()) {
                ScrollInternetExplorerHideFixedFooter = true;
                ScrollInternetExplorerHideFixedFooterElements = scrollIeHideFixedFooter.Value().Replace("*", Constants.HideFooterTagClassItems);
            }
        }

        #region property
        public Keys ExitKey { get; } = Keys.Escape;

        public bool NowSelecting { get; set; }

        public Setting.Define.CaptureMode CaptureMode { get; }

        public bool IsEnabledClipboard { get; }

        public DirectoryInfo SaveDirectory { get; }
        public string SaveFileNameFormat { get; }
        public ImageKind SaveImageKind { get; } = ImageKind.Png;

        public EventWaitHandle SaveNoticeEvent { get; }

        public bool IsContinuation { get; }
        public bool ImmediatelySelect { get; }

        public Keys ShotKeys { get; } = Keys.None;
        public Keys SelectKeys { get; } = Keys.None;

        public TimeSpan ShotDelayTime { get; } = Constants.ShotDelayTime;

        public Color BorderColor { get; } = Constants.CameraBorderColor;
        public int BorderWidth { get; } = Constants.CameraBorderWidth;

        public TimeSpan ScrollDelayTime { get; } = Constants.ScrollDelayTime;
        public TimeSpan ScrollInternetExplorerInitializeTime { get; } = Constants.ScrollInternetExplorerInitializeTime;

        public bool ScrollInternetExplorerHideFixedHeader { get; } = false;
        public string ScrollInternetExplorerHideFixedHeaderElements { get; }

        public bool ScrollInternetExplorerHideFixedFooter { get; } = false;
        public string ScrollInternetExplorerHideFixedFooterElements { get; }

        #endregion
    }
}
