using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting;
using ContentTypeTextNet.NKit.Setting.Capture;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureManagerModel : ManagerModelBase
    {
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

        public void CaptureControl()
        {
            var arguments = new List<string>() {
                "--mode",
                ProgramRelationUtility.EscapesequenceToArgument(CaptureMode.TargetControl.ToString()),

                "--clipboard",

                "--immediately_start",
            };

            if(CaptureKeyUtility.CanSendKeySetting(Setting.Capture.TakeShotKey)) {
                arguments.Add("--photo_opportunity_key");
                arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(CaptureKeyUtility.ToCameramanArgumentKey(Setting.Capture.TakeShotKey)));
            }
            if(CaptureKeyUtility.CanSendKeySetting(Setting.Capture.SelectKey)) {
                arguments.Add("--wait_opportunity_key");
                arguments.Add(ProgramRelationUtility.EscapesequenceToArgument(CaptureKeyUtility.ToCameramanArgumentKey(Setting.Capture.SelectKey)));
            }

            using(var client = new NKitApplicationTalkerClient(NKitApplicationKind.Main, StartupOptions.ServiceUri)) {
                client.Open();
                client.WakeupApplication(NKitApplicationKind.Cameraman, string.Join(" ", arguments), string.Empty);
            }
        }

        #endregion
    }
}
