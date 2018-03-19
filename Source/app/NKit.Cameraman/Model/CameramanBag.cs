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
    public class SaveImageParameter
    {
        #region define

        public static SaveImageParameter Disabled { get; } = new SaveImageParameter(false);

        #endregion

        private SaveImageParameter(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// サイズ指定。
        /// </summary>
        /// <param name="size"></param>
        /// <param name="fileNameFormat"></param>
        /// <param name="imageKind"></param>
        public SaveImageParameter(ImageKind imageKind, string fileNameFormat, Size size)
            : this(true)
        {
            ImageKind = imageKind;
            FileNameFormat = fileNameFormat;
            Size = size;
        }

        /// <summary>
        /// 通常サイズ。
        /// </summary>
        /// <param name="imageKind"></param>
        /// <param name="fileNameFormat"></param>
        public SaveImageParameter(ImageKind imageKind, string fileNameFormat)
            : this(imageKind, fileNameFormat, Size.Empty)
        { }


        #region property

        public bool IsEnabled { get; }
        public ImageKind ImageKind { get; }
        public string FileNameFormat { get; }
        /// <summary>
        /// <see cref="Size.Width"/>, <see cref="Size.Height"/>のどちらかが 0 ならそのまんまのサイズで保存する。
        /// </summary>
        public Size Size { get; }

        #endregion

        #region function

        public static SaveImageParameter Parse(string s)
        {
            var splitValues = s.Split('/');
            switch(splitValues.Length) {
                case 2:
                    return new SaveImageParameter(
                        EnumUtility.Parse<ImageKind>(splitValues[0]),
                        splitValues[1]
                    );

                case 4:
                    return new SaveImageParameter(
                        EnumUtility.Parse<ImageKind>(splitValues[0]),
                        splitValues[1],
                        new Size(
                            int.Parse(splitValues[2]),
                            int.Parse(splitValues[3])
                        )
                    );

                default:
                    throw new ArgumentException(s);
            }
        }

        #endregion
    }

    public class CameramanBag
    {
        public CameramanBag(string[] arguments)
        {
            var command = new CommandLineApplication(false);

            var targetOption = command.Option("--target", "target", CommandOptionType.SingleValue);
            var clipboardOption = command.Option("--clipboard", "use clipboard", CommandOptionType.NoValue);
            var saveDirOption = command.Option("--save_directory", "save directory", CommandOptionType.SingleValue);
            var saveImageOption = command.Option("--save_image", "[image kind]/[thumbnail file name format], extension is ${EXT}", CommandOptionType.SingleValue);
            var saveThumbnailOption = command.Option("--save_thumbnail", "[image kind]/[thumbnail file name format]/[width]/[height], extension is ${EXT}", CommandOptionType.SingleValue);
            var saveEventOption = command.Option("--save_event_name", "save event", CommandOptionType.SingleValue);
            var continuationOption = command.Option("--continuation", "single/continuation", CommandOptionType.NoValue);
            var isImmediateSelectOption = command.Option("--immediate_select", "start select", CommandOptionType.NoValue);
            var shotKeyOption = command.Option("--take_shot_key", $"shot normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var selectKeyOption = command.Option("--select_photo_key", $"select normal key + {Keys.Control}, {Keys.Shift}, {Keys.Alt}", CommandOptionType.SingleValue);
            var shotDelayTimeOption = command.Option("--photo_opportunity_delay_time", "shot deilay time", CommandOptionType.SingleValue);
            var cameraBorderColorOption = command.Option("--camera_border_color", "color", CommandOptionType.SingleValue);
            var cameraBorderWidthOption = command.Option("--camera_border_width", "color", CommandOptionType.SingleValue);
            var scrollDelayTimeOption = command.Option("--scroll_delay_time", "color", CommandOptionType.SingleValue);
            var scrollIeInitializeTimeOption = command.Option("--scroll_ie_init_time", "color", CommandOptionType.SingleValue);
            var scrollIeHideFixedHeader = command.Option("--scroll_ie_hide_header", "value * is default", CommandOptionType.SingleValue);
            var scrollIeHideFixedFooter = command.Option("--scroll_ie_hide_footer", "value * is default", CommandOptionType.SingleValue);

            command.Execute(arguments);

            CaptureTarget = EnumUtility.Parse<Setting.Define.CaptureTarget>(targetOption.Value());
            IsEnabledClipboard = clipboardOption.HasValue();
            if(saveDirOption.HasValue()) {
                SaveDirectory = new DirectoryInfo(saveDirOption.Value());

                if(!saveImageOption.HasValue()) {
                    throw new ArgumentException("--save_directory need --save_image");
                }
                Image = SaveImageParameter.Parse(saveImageOption.Value());

                if(saveThumbnailOption.HasValue()) {
                    Thumbnail = SaveImageParameter.Parse(saveThumbnailOption.Value());
                    if(Thumbnail.Size.Width == 0 || Thumbnail.Size.Height == 0) {
                        throw new ArgumentException("thumbnail size error");
                    }
                }
            }
            if(saveEventOption.HasValue()) {
                SaveNoticeEvent = EventWaitHandle.OpenExisting(saveEventOption.Value());
            }
            IsContinuation = continuationOption.HasValue();
            IsImmediateSelect = isImmediateSelectOption.HasValue();

            var needKey = true;
            if(CaptureTarget == Setting.Define.CaptureTarget.Screen && !IsContinuation) {
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

            if(CaptureTarget == Setting.Define.CaptureTarget.Scroll) {
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

        public Setting.Define.CaptureTarget CaptureTarget { get; }

        public bool IsEnabledClipboard { get; }

        public DirectoryInfo SaveDirectory { get; }
        public SaveImageParameter Image { get; } = SaveImageParameter.Disabled;

        public SaveImageParameter Thumbnail { get; } = SaveImageParameter.Disabled;

        public EventWaitHandle SaveNoticeEvent { get; }

        public bool IsContinuation { get; }
        public bool IsImmediateSelect { get; }

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
