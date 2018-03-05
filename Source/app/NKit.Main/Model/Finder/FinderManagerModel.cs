using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FinderManagerModel : ManagerModelBase
    {
        public FinderManagerModel(Setting setting)
            : base(setting)
        { }

        #region property

        public ObservableCollection<FindGroupModel> Groups { get; } = new ObservableCollection<FindGroupModel>();

        #endregion

        #region property

        FindGroupModel CreateGroupModel()
        {
            var setting = new FindGroupSetting();
            var model = new FindGroupModel(setting, Setting.Finder, Setting.Application);

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
            Groups.RemoveAt(index);
        }

        #endregion
    }
}
