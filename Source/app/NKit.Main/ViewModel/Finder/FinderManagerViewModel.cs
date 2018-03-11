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
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FinderManagerViewModel : ManagerViewModelBase<FinderManagerModel>
    {
        #region variable

        FindGroupViewModel _selectedGroupItem;

        #endregion

        public FinderManagerViewModel(FinderManagerModel model)
            : base(model)
        {
            GroupViewModels = new ObservableCollection<FindGroupViewModel>(model.Groups.Select(g => new FindGroupViewModel(g)));
            Groups = CollectionViewSource.GetDefaultView(GroupViewModels);
        }

        #region property

        ObservableCollection<FindGroupViewModel> GroupViewModels { get; }
        public ICollectionView Groups { get; }
        public FindGroupViewModel SelectedGroupItem
        {
            get { return this._selectedGroupItem; }
            set { SetProperty(ref this._selectedGroupItem, value); }
        }

        #endregion

        #region command

        public ICommand AddNewGroupCommand => new DelegateCommand(() => {
            var model = Model.AddNewGroup();
            var viewModel = new FindGroupViewModel(model);
            GroupViewModels.Add(viewModel);
            SelectedGroupItem = viewModel;
        });

        public ICommand RemoveGroupCommand => new DelegateCommand<FindGroupViewModel>(vm => {
            if(SelectedGroupItem == vm) {
                // くるしい, 近しい子を選んであげるべき
                SelectedGroupItem = GroupViewModels.Where(g => g != vm).FirstOrDefault();
            }

            var index = GroupViewModels.IndexOf(vm);
            GroupViewModels.RemoveAt(index);
            vm.Dispose();
            Model.RemoveAtInGroups(index);
        });


        #endregion
    }
}
