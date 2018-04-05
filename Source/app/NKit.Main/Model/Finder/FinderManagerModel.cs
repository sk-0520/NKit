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

            HistoryItems = new ObservableCollection<IReadOnlyFindGroupSetting>(Setting.Finder.Histories);
        }

        #region property

        public ObservableCollection<FindGroupModel> Groups { get; }

        public ObservableCollection<IReadOnlyFindGroupSetting> HistoryItems { get; }

        #endregion

        #region function

        FindGroupModel CreateGroupModel(IReadOnlyFindGroupSetting baseSetting)
        {
            var setting = SerializeUtility.Clone<FindGroupSetting>(baseSetting);

            setting.Id = Guid.NewGuid();
            setting.GroupName = TextUtility.ToUniqueDefault(
                Properties.Resources.String_Finder_FindGroup_NewGroupName,
                Setting.Finder.Groups.Select(g => g.GroupName),
                StringComparison.InvariantCultureIgnoreCase
            );
            setting.CreatedUtcTimestamp = DateTime.UtcNow;
            setting.UpdatedUtcTimestamp = DateTime.UtcNow;

            var model = new FindGroupModel(this, setting, Setting.Finder, Setting.File, Setting.NKit);

            return model;
        }

        public FindGroupModel AddNewGroup()
        {
            var model = CreateGroupModel(Setting.Finder.DefaultGroupSetting);

            Setting.Finder.Groups.Add(model.FindGroupSetting);
            Groups.Add(model);

            return model;
        }

        public void AddHistory(FindGroupModel model)
        {
            while(Constants.FinderHistoryLimit <= Setting.Finder.Histories.Count) {
                Setting.Finder.Histories.RemoveAt(0);
                HistoryItems.RemoveAt(0);
            }

            Setting.Finder.Histories.Add(model.FindGroupSetting);
            HistoryItems.Add(model.FindGroupSetting);
        }

        public FindGroupModel RecallHistory(IReadOnlyFindGroupSetting setting)
        {
            var model = CreateGroupModel(setting);

            Setting.Finder.Groups.Add(model.FindGroupSetting);
            Groups.Add(model);

            return model;
        }

        public void ClearHistory()
        {
            Setting.Finder.Histories.Clear();
            HistoryItems.Clear();
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
