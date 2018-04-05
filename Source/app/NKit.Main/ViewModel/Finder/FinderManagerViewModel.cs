using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FinderManagerViewModel : ManagerViewModelBase<FinderManagerModel>
    {
        #region variable

        bool _isOpenHistory;
        FindGroupViewModel _selectedGroupItem;

        #endregion

        public FinderManagerViewModel(FinderManagerModel model)
            : base(model)
        {
            GroupViewModels = new ObservableCollection<FindGroupViewModel>(Model.Groups.Select(g => new FindGroupViewModel(g)));
            if(GroupViewModels.Any()) {
                SelectedGroupItem = GroupViewModels[0];
            }
            Groups = CollectionViewSource.GetDefaultView(GroupViewModels);

            HistoryItems = CollectionViewSource.GetDefaultView(Model.HistoryItems);
            HistoryItems.SortDescriptions.Add(new SortDescription(nameof(IReadOnlyFindGroupSetting.UpdatedUtcTimestamp), ListSortDirection.Descending));
            HistoryItems.SortDescriptions.Add(new SortDescription(nameof(IReadOnlyFindGroupSetting.CreatedUtcTimestamp), ListSortDirection.Descending));
        }

        #region property

        ObservableCollection<FindGroupViewModel> GroupViewModels { get; }
        public ICollectionView Groups { get; }
        public ICollectionView HistoryItems { get; }

        public FindGroupViewModel SelectedGroupItem
        {
            get { return this._selectedGroupItem; }
            set { SetProperty(ref this._selectedGroupItem, value); }
        }

        public bool IsOpenHistory
        {
            get { return this._isOpenHistory; }
            set { SetProperty(ref this._isOpenHistory, value); }
        }

        #endregion

        #region command

        public ICommand AddNewGroupCommand => new DelegateCommand(() => {
            // TODO: ObservableManager による管理
            var model = Model.AddNewGroup();
            var viewModel = new FindGroupViewModel(model);

            GroupViewModels.Add(viewModel);
            SelectedGroupItem = viewModel;
        });

        public ICommand RecallHistoryCommand => new DelegateCommand<IReadOnlyFindGroupSetting>(setting => {
            var model = Model.RecallHistory(setting);
            var viewModel = new FindGroupViewModel(model);

            GroupViewModels.Add(viewModel);
            SelectedGroupItem = viewModel;
            IsOpenHistory = false;
        });

        public ICommand ClearHistoryCommand => new DelegateCommand(() => {
            Model.ClearHistory();
            IsOpenHistory = false;
        });

        public ICommand RemoveGroupCommand => new DelegateCommand<FindGroupViewModel>(vm => {
            if(SelectedGroupItem == vm) {
                // くるしい, 近しい子を選んであげるべき
                SelectedGroupItem = GroupViewModels.Where(g => g != vm).FirstOrDefault();
            }

            var index = GroupViewModels.IndexOf(vm);
            GroupViewModels.RemoveAt(index);
            vm.Dispose();
            Model.RemoveGroupAt(index);
        });


        #endregion
    }
}
