using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting;
using ContentTypeTextNet.NKit.Setting.Capture;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureManagerModel : ManagerModelBase
    {
        #region variable

        bool _nowCapturing;

        #endregion

        public CaptureManagerModel(MainSetting setting)
            : base(setting)
        {
            Groups = new ObservableCollection<CaptureGroupModel>(
                Setting.Capture.Groups.Select(g => new CaptureGroupModel(this, g, Setting.Capture, Setting.NKit))
            );
        }

        #region property

        public ObservableCollection<CaptureGroupModel> Groups { get; }

        public KeySetting SelectKeySetting => Setting.Capture.SelectKey;
        public KeySetting TakeShotKeySetting => Setting.Capture.TakeShotKey;

        public bool ScrollInternetExplorerIsEnabledHideFixedHeader
        {
            get { return Setting.Capture.ScrollInternetExplorerIsEnabledHideFixedHeader; }
            set { Setting.Capture.ScrollInternetExplorerIsEnabledHideFixedHeader = value; }
        }
        public string ScrollInternetExplorerHideFixedHeaderElements
        {
            get { return Setting.Capture.ScrollInternetExplorerHideFixedHeaderElements; }
            set { Setting.Capture.ScrollInternetExplorerHideFixedHeaderElements = value; }
        }

        public bool ScrollInternetExplorerIsEnabledHideFixedFooter
        {
            get { return Setting.Capture.ScrollInternetExplorerIsEnabledHideFixedFooter; }
            set { Setting.Capture.ScrollInternetExplorerIsEnabledHideFixedFooter = value; }
        }
        public string ScrollInternetExplorerHideFixedFooterElements
        {
            get { return Setting.Capture.ScrollInternetExplorerHideFixedFooterElements; }
            set { Setting.Capture.ScrollInternetExplorerHideFixedFooterElements = value; }
        }

        public bool NowCapturing
        {
            get { return this._nowCapturing; }
            set { SetProperty(ref this._nowCapturing, value); }
        }

        CancellationTokenSource CaptureCancel { get; set; }
        TimeSpan CaptureExitPollingTime { get; set; } = TimeSpan.FromSeconds(30);

        #endregion

        #region function

        CaptureGroupModel CreateGroupModel()
        {
            //var setting = SerializeUtility.Clone(Setting.Finder.DefaultGroupSetting);

            //setting.Id = Guid.NewGuid();
            var setting = new CaptureGroupSetting();
            setting.GroupName = TextUtility.ToUniqueDefault(
                Properties.Resources.String_Capture_CaptureGroup_NewGroupName,
                Setting.Capture.Groups.Select(g => g.GroupName),
                StringComparison.InvariantCultureIgnoreCase
            );

            var model = new CaptureGroupModel(this, setting, Setting.Capture, Setting.NKit);

            return model;
        }

        public CaptureGroupModel AddNewGroup()
        {
            var model = CreateGroupModel();

            Setting.Capture.Groups.Add(model.GroupSetting);
            Groups.Add(model);

            return model;
        }

        public void RemoveGroupAt(int index)
        {
            var model = Groups[index];
            Groups.RemoveAt(index);

            var groupSetting = Setting.Capture.Groups.FirstOrDefault(g => g.Id == model.GroupSetting.Id);
            if(groupSetting != null) {
                Setting.Capture.Groups.Remove(groupSetting);
            }

            model.Dispose();
        }

        void Capture(string arguments, string workingDirectoryPath)
        {
            Debug.Assert(!NowCapturing);

            using(var client = new ApplicationSwitcher(StartupOptions.ServiceUri)) {
                client.Initialize();
                var manageId = client.PreparateApplication(NKitApplicationKind.Cameraman, arguments, workingDirectoryPath);
                var status = client.GetStatusApplication(manageId);
                //if(status.IsEnabled) {
                if(!string.IsNullOrEmpty(status.ExitedEventName)) {
                    var cameramanExitEvent = EventWaitHandle.OpenExisting(status.ExitedEventName);
                    CaptureCancel = new CancellationTokenSource();
                    var token = CaptureCancel.Token;
                    Task.Run(() => {
                        while(true) {
                            var result = cameramanExitEvent.WaitOne(CaptureExitPollingTime);
                            if(result) {
                                Logger.Debug("capture exit!");
                                break;
                            }
                            token.ThrowIfCancellationRequested();
                            Logger.Debug("capture continue!");
                        }
                    }, CaptureCancel.Token).ContinueWith(_ => {
                        NowCapturing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }

                if(client.WakeupApplication(manageId)) {
                    NowCapturing = true;
                }

                //}
            }
        }

        void SimpleCaptureCore(CaptureMode captureMode)
        {
            var arguments = new List<string>() {
                "--mode",
                ProgramRelationUtility.EscapesequenceToArgument(captureMode.ToString()),

                "--clipboard",

                "--immediately_select",
            };

            if(ScrollInternetExplorerIsEnabledHideFixedHeader) {
                arguments.Add("--scroll_ie_hide_header");

                if(string.IsNullOrWhiteSpace(ScrollInternetExplorerHideFixedHeaderElements)) {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument("*"));
                } else {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(ScrollInternetExplorerHideFixedHeaderElements));
                }
            }
            if(ScrollInternetExplorerIsEnabledHideFixedFooter) {
                arguments.Add("--scroll_ie_hide_footer");

                if(string.IsNullOrWhiteSpace(ScrollInternetExplorerHideFixedFooterElements)) {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument("*"));
                } else {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(ScrollInternetExplorerHideFixedFooterElements));
                }
            }

            if(CaptureKeyUtility.CanSendKeySetting(Setting.Capture.TakeShotKey)) {
                arguments.Add("--photo_opportunity_key");
                arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(CaptureKeyUtility.ToCameramanArgumentKey(Setting.Capture.TakeShotKey)));
            }
            if(CaptureKeyUtility.CanSendKeySetting(Setting.Capture.SelectKey)) {
                arguments.Add("--wait_opportunity_key");
                arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(CaptureKeyUtility.ToCameramanArgumentKey(Setting.Capture.SelectKey)));
            }

            Capture(string.Join(" ", arguments), string.Empty);
        }

        public void SimpleCaptureControl()
        {
            SimpleCaptureCore(CaptureMode.Control);
        }
        public void SimpleCaptureWindow()
        {
            SimpleCaptureCore(CaptureMode.Window);
        }
        public void SimpleCaptureScroll()
        {
            SimpleCaptureCore(CaptureMode.Scroll);
        }

        #endregion
    }
}
