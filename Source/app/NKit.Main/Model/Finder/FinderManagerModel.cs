using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Setting;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FinderManagerModel : ManagerModelBase
    {
        public FinderManagerModel(MainSetting setting)
            : base(setting)
        {
            Groups = new ObservableCollection<FindGroupModel>(
                Setting.Finder.Groups.Select(g => new FindGroupModel(this, g, Setting.Finder, Setting.File, Setting.NKit))
            );
        }

        #region property

        public ObservableCollection<FindGroupModel> Groups { get; }

        #endregion

        #region function

        FindGroupModel CreateGroupModel()
        {
            var setting = SerializeUtility.Clone(Setting.Finder.DefaultGroupSetting);

            setting.Id = Guid.NewGuid();
            setting.GroupName = TextUtility.ToUniqueDefault(
                Properties.Resources.String_Finder_FindGroup_NewGroupName,
                Setting.Finder.Groups.Select(g => g.GroupName),
                StringComparison.InvariantCultureIgnoreCase
            );

            var model = new FindGroupModel(this, setting, Setting.Finder, Setting.File, Setting.NKit);

            return model;
        }

        public FindGroupModel AddNewGroup()
        {
            var model = CreateGroupModel();

            Setting.Finder.Groups.Add(model.FindGroupSetting);
            Groups.Add(model);

            return model;
        }

        public void RemoveGroupAt(int index)
        {
            var model = Groups[index];
            Groups.RemoveAt(index);

            var groupSetting = Setting.Finder.Groups.FirstOrDefault(g => g.Id == model.FindGroupSetting.Id);
            if(groupSetting != null) {
                Setting.Finder.Groups.Remove(groupSetting);
            }

            model.Dispose();
        }

        public void SetDefaultSetting(IReadOnlyFindGroupSetting setting)
        {
            // 変更処理は行わないけど複製はしたい、そんな思い
            var defaultSetting = SerializeUtility.Clone<FindGroupSetting>(setting);

            defaultSetting.Id = Guid.Empty;
            defaultSetting.GroupName = string.Empty;

            Setting.Finder.DefaultGroupSetting = defaultSetting;
        }

        #endregion
    }
}
