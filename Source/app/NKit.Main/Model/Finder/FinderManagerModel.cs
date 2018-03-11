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
        { }

        #region property

        public ObservableCollection<FindGroupModel> Groups { get; } = new ObservableCollection<FindGroupModel>();

        #endregion

        #region property

        FindGroupModel CreateGroupModel()
        {
            var setting = new FindGroupSetting();
            var model = new FindGroupModel(this, setting, Setting.Finder, Setting.File, Setting.NKit);

            return model;
        }

        public FindGroupModel AddNewGroup()
        {
            var model = CreateGroupModel();
            Groups.Add(model);

            return model;
        }

        public void RemoveAtInGroups(int index)
        {
            var model = Groups[index];
            Groups.RemoveAt(index);
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
