using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Setting.Capture;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Capture
{
    public class CaptureManagerViewModel : ManagerViewModelBase<CaptureManagerModel>
    {
        #region variable

        CaptureGroupViewModel _selectedGroupItem;

        #endregion

        public CaptureManagerViewModel(CaptureManagerModel model)
            : base(model)
        {
            GroupViewModels = new ObservableCollection<CaptureGroupViewModel>(Model.Groups.Select(g => new CaptureGroupViewModel(g)));
            if(GroupViewModels.Any()) {
                SelectedGroupItem = GroupViewModels[0];
            }
            Groups = CollectionViewSource.GetDefaultView(GroupViewModels);
        }

        #region property

        ObservableCollection<CaptureGroupViewModel> GroupViewModels { get; }
        public ICollectionView Groups { get; }

        public CaptureGroupViewModel SelectedGroupItem
        {
            get { return this._selectedGroupItem; }
            set { SetProperty(ref this._selectedGroupItem, value); }
        }

        public Key SelectKey
        {
            get { return Model.SelectKeySetting.Key; }
            set { SetPropertyValue(Model.SelectKeySetting, value, nameof(Model.SelectKeySetting.Key)); }
        }
        public ModifierKeys SelectModifierKeys
        {
            get { return Model.SelectKeySetting.ModifierKeys; }
            set { SetPropertyValue(Model.SelectKeySetting, value, nameof(Model.SelectKeySetting.ModifierKeys)); }
        }
        public Key TakeShotKey
        {
            get { return Model.TakeShotKeySetting.Key; }
            set { SetPropertyValue(Model.TakeShotKeySetting, value, nameof(Model.TakeShotKeySetting.Key)); }
        }
        public ModifierKeys TakeShotModifierKeys
        {
            get { return Model.TakeShotKeySetting.ModifierKeys; }
            set { SetPropertyValue(Model.TakeShotKeySetting, value, nameof(Model.TakeShotKeySetting.ModifierKeys)); }
        }

        public bool IsEnabledHideHeader
        {
            get { return Model.ScrollInternetExplorerIsEnabledHideFixedHeader; }
            set { SetPropertyValue(Model, value, nameof(Model.ScrollInternetExplorerIsEnabledHideFixedHeader)); }
        }
        public string HideHeaderElement
        {
            get { return Model.ScrollInternetExplorerHideFixedHeaderElements; }
            set { SetPropertyValue(Model, value, nameof(Model.ScrollInternetExplorerHideFixedHeaderElements)); }
        }

        public bool IsEnabledHideFooter
        {
            get { return Model.ScrollInternetExplorerIsEnabledHideFixedFooter; }
            set { SetPropertyValue(Model, value, nameof(Model.ScrollInternetExplorerIsEnabledHideFixedFooter)); }
        }

        public string HideFooterElement
        {
            get { return Model.ScrollInternetExplorerHideFixedFooterElements; }
            set { SetPropertyValue(Model, value, nameof(Model.ScrollInternetExplorerHideFixedFooterElements)); }
        }

        #endregion

        #region command

        public ICommand AddNewGroupCommand => new DelegateCommand(() => {
            var model = Model.AddNewGroup();
            var viewModel = new CaptureGroupViewModel(model);

            GroupViewModels.Add(viewModel);
            SelectedGroupItem = viewModel;
        });

        public ICommand RemoveGroupCommand => new DelegateCommand<CaptureGroupViewModel>(vm => {
            if(SelectedGroupItem == vm) {
                // くるしい, 近しい子を選んであげるべき
                SelectedGroupItem = GroupViewModels.Where(g => g != vm).FirstOrDefault();
            }

            var index = GroupViewModels.IndexOf(vm);
            GroupViewModels.RemoveAt(index);
            vm.Dispose();
            Model.RemoveGroupAt(index);
        });

        public ICommand CaptureControlCommand => new DelegateCommand(
            () => { Model.CaptureControl(); }
        );
        public ICommand CaptureWindowCommand => new DelegateCommand(
            () => { Model.CaptureWindow(); }
        );
        public ICommand CaptureScrollCommand => new DelegateCommand(
            () => { Model.CaptureScroll(); }
        );

        #endregion

        #region function
        #endregion
    }
}
