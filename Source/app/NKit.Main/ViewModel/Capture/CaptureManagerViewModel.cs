using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

            Model.PropertyChanged += Model_PropertyChanged;
        }

        #region property

        ObservableCollection<CaptureGroupViewModel> GroupViewModels { get; }
        public ICollectionView Groups { get; }

        public CaptureGroupViewModel SelectedGroupItem
        {
            get { return this._selectedGroupItem; }
            set
            {
                if(SetProperty(ref this._selectedGroupItem, value)) {
                    if(SelectedGroupItem != null) {
                        SelectedGroupItem.InitializeCaptureFilesAsync();
                    }
                }
            }
        }

        public bool NowCapturing => Model.NowCapturing;

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
            get { return Model.InternetExplorerScrollCaptureSetting.Header.IsEnabled; }
            set { SetPropertyValue(Model.InternetExplorerScrollCaptureSetting.Header, value, nameof(Model.InternetExplorerScrollCaptureSetting.Header.IsEnabled)); }
        }
        public string HideHeaderElement
        {
            get { return Model.InternetExplorerScrollCaptureSetting.Header.HideElements; }
            set { SetPropertyValue(Model.InternetExplorerScrollCaptureSetting.Header, value, nameof(Model.InternetExplorerScrollCaptureSetting.Header.HideElements)); }
        }

        public bool IsEnabledHideFooter
        {
            get { return Model.InternetExplorerScrollCaptureSetting.Footer.IsEnabled; }
            set { SetPropertyValue(Model.InternetExplorerScrollCaptureSetting.Footer, value, nameof(Model.InternetExplorerScrollCaptureSetting.Footer.IsEnabled)); }
        }
        public string HideFooterElement
        {
            get { return Model.InternetExplorerScrollCaptureSetting.Footer.HideElements; }
            set { SetPropertyValue(Model.InternetExplorerScrollCaptureSetting.Footer, value, nameof(Model.InternetExplorerScrollCaptureSetting.Footer.HideElements)); }
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

        public ICommand SimpleCaptureControlCommand => new DelegateCommand(
            () => { Model.SimpleCapture(Setting.Define.CaptureTarget.Control); },
            () => !NowCapturing
        );
        public ICommand SimpleCaptureWindowCommand => new DelegateCommand(
            () => { Model.SimpleCapture(Setting.Define.CaptureTarget.Window); },
            () => !NowCapturing
        );
        public ICommand SimpleCaptureScrollCommand => new DelegateCommand(
            () => { Model.SimpleCapture(Setting.Define.CaptureTarget.Scroll); },
            () => !NowCapturing
        );

        #endregion

        #region function


        #endregion

        #region ManagerViewModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    Model.PropertyChanged -= Model_PropertyChanged;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Model.NowCapturing)) {
                //Application.Current.Dispatcher.Invoke(() => {
                RaisePropertyChanged(nameof(NowCapturing));
                foreach(var g  in GroupViewModels) {
                    g.RaiseNowCapturingPropertyChanged();
                }
                //CommandManager.InvalidateRequerySuggested();
                //});
            }
        }
    }
}
