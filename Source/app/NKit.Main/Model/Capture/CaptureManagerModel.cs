using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        public InternetExplorerScrollCaptureSetting InternetExplorerScrollCaptureSetting => Setting.Capture.Scroll.InternetExplorer;

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

        void CaptureCore(string arguments, string workingDirectoryPath)
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

        // 引数がなぁ、多いのなぁ
        public void Capture(CaptureMode captureMode, bool isEnabledClipboard, bool isImmediateSelect, bool isContinuation, string savedEventName, DirectoryInfo saveDirectory, IReadOnlyScrollCaptureSetting scrollSetting)
        {
            var arguments = new List<string>() {
                "--mode",
                ProgramRelationUtility.EscapesequenceToArgument(captureMode.ToString()),
            };

            if(isEnabledClipboard) {
                arguments.Add("--clipboard");
            }

            if(isImmediateSelect) {
                arguments.Add("--immediately_select");
            }

            if(isContinuation) {
                arguments.Add("--continuation");

            }

            if(scrollSetting.InternetExplorer.Header.IsEnabled) {
                arguments.Add("--scroll_ie_hide_header");

                if(string.IsNullOrWhiteSpace(scrollSetting.InternetExplorer.Header.HideElements)) {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument("*"));
                } else {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(scrollSetting.InternetExplorer.Header.HideElements));
                }
            }
            if(scrollSetting.InternetExplorer.Footer.IsEnabled) {
                arguments.Add("--scroll_ie_hide_footer");

                if(string.IsNullOrWhiteSpace(scrollSetting.InternetExplorer.Footer.HideElements)) {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument("*"));
                } else {
                    arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(scrollSetting.InternetExplorer.Footer.HideElements));
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

            CaptureCore(string.Join(" ", arguments), string.Empty);
        }

        public void SimpleCapture(CaptureMode captureMode)
        {
            Capture(captureMode, true, true, false, default(string), default(DirectoryInfo), Setting.Capture.Scroll);
        }

        #endregion
    }
}
