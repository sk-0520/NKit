using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Capture
{
    public class CaptureGroupViewModel : RunnableViewModelBase<CaptureGroupModel, None>
    {
        #region define

        static readonly DateTime UnSelectedFilterTimestamp = DateTime.MinValue;

        #endregion

        #region variable

        RunState _initialRunState;
        DateTime _selectedFilterStartUtcTimestamp = UnSelectedFilterTimestamp;

        CaptureImageViewModel _selectedImageItem;

        #endregion

        public CaptureGroupViewModel(CaptureGroupModel model)
            : base(model)
        {
            ImageItemCollectionManager = new ActionModeliewModelObservableCollectionManager<CaptureImageModel, CaptureImageViewModel>(Model.Images);
            ImageItems = GetInvokeUI(() => CollectionViewSource.GetDefaultView(ImageItemCollectionManager.ViewModels));
            ImageItems.Filter = CaptureItemFilter;

            AttachItemsCollectionChanged();
        }

        #region property

        public InteractionRequest<ScrollNotification<CaptureImageViewModel>> ScrollRequest { get; } = new InteractionRequest<ScrollNotification<CaptureImageViewModel>>();

        public string GroupName
        {
            get { return Model.GroupSetting.GroupName; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public CaptureTarget CaptureTarget
        {
            get { return Model.GroupSetting.CaptureTarget; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool IsImmediateSelect
        {
            get { return Model.GroupSetting.IsImmediateSelect; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool OverwriteScrollSetting
        {
            get { return Model.GroupSetting.OverwriteScrollSetting; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool IsEnabledHideHeader
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Header.IsEnabled; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Header, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Header.IsEnabled)); }
        }
        public string HideHeaderElement
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Header.HideElements; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Header, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Header.HideElements)); }
        }

        public bool IsEnabledHideFooter
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Footer.IsEnabled; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Footer, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Footer.IsEnabled)); }
        }
        public string HideFooterElement
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Footer.HideElements; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Footer, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Footer.HideElements)); }
        }

        public bool IsEnabledClipboard
        {
            get { return Model.GroupSetting.IsEnabledClipboard; }
            set { SetPropertyValue(Model.GroupSetting, value, nameof(Model.GroupSetting.IsEnabledClipboard)); }
        }

        public CaptureImageViewModel SelectedImageItem
        {
            get { return this._selectedImageItem; }
            set { SetProperty(ref this._selectedImageItem, value); }
        }

        ActionModeliewModelObservableCollectionManager<CaptureImageModel, CaptureImageViewModel> ImageItemCollectionManager { get; }
        public ICollectionView ImageItems { get; set; }

        public ObservableCollection<DateTime> FilterStartUtcTimestampItems { get; } = new ObservableCollection<DateTime>();
        public DateTime SelectedFilterStartUtcTimestamp
        {
            get { return this._selectedFilterStartUtcTimestamp; }
            set
            {
                if(SetProperty(ref this._selectedFilterStartUtcTimestamp, value)) {
                    ImageItems.Refresh();
                }
            }
        }


        bool IsEnabledLastItemScroll { get; set; } = true;
        bool IsEnabledAddCaptureStartTimestamp { get; set; } = true;

        public RunState InitialRunState
        {
            get { return this._initialRunState; }
            set { SetProperty(ref this._initialRunState, value); }
        }

        #endregion

        #region command

        public ICommand RemoveImageCommand => GetOrCreateCommand(() => new DelegateCommand<CaptureImageViewModel>(
            vm => {
                var index = ImageItemCollectionManager.ViewModels.IndexOf(vm);
                Model.RemoveImageAt(index);
            }
        ));

        #endregion

        #region function

        public Task InitializeCaptureFilesAsync()
        {
            if(InitialRunState == RunState.None || InitialRunState == RunState.Error) {
                IsEnabledLastItemScroll = false;
                IsEnabledAddCaptureStartTimestamp = false;
                InitialRunState = RunState.Running;
                return Task.Run(() => {
                    Model.InitializeCaptureFiles();
                }).ContinueWith(t => {
                    if(t.IsFaulted) {
                        InitialRunState = RunState.Error;
                    } else {
                        InitialRunState = RunState.Finished;
                    }

                    var startTimestamps = ImageItemCollectionManager.ViewModels
                        .Select(vm => vm.CaptureStartUtcTimestamp)
                        .GroupBy(time => time)
                        .Select(g => g.First())
                        .OrderBy(time => time)
                    ;
                    FilterStartUtcTimestampItems.Clear();
                    FilterStartUtcTimestampItems.Add(UnSelectedFilterTimestamp);
                    FilterStartUtcTimestampItems.AddRange(startTimestamps);
                    SelectedFilterStartUtcTimestamp = UnSelectedFilterTimestamp;
                    ImageItems.Refresh();

                    IsEnabledLastItemScroll = true;
                    IsEnabledAddCaptureStartTimestamp = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            return Task.CompletedTask;
        }
        private bool CaptureItemFilter(object obj)
        {
            if(SelectedFilterStartUtcTimestamp == UnSelectedFilterTimestamp) {
                return true;
            }

            if(obj is CaptureImageViewModel image) {
                return image.CaptureStartUtcTimestamp == SelectedFilterStartUtcTimestamp;
            }

            return false;
        }

        public void RaiseNowCapturingPropertyChanged()
        {
            RaisePropertyChanged(nameof(CanRun));
        }

        void AttachItemsCollectionChanged()
        {
            ImageItemCollectionManager.ToViewModel = m => new CaptureImageViewModel(m);

            ImageItemCollectionManager.AddItems = (kind, newModels, newViewModels) => {
                if(kind == ObservableCollectionKind.After) {
                    if(IsEnabledLastItemScroll) {
                        ScrollRequest.Raise(new ScrollNotification<CaptureImageViewModel>(newViewModels.Last()));
                    }
                    if(IsEnabledAddCaptureStartTimestamp) {
                        if(FilterStartUtcTimestampItems.Count == 0) {
                            FilterStartUtcTimestampItems.Add(UnSelectedFilterTimestamp);
                        }
                        foreach(var addTimestamp in newViewModels.GroupBy(vm => vm.CaptureStartUtcTimestamp).Select(g => g.Key).OrderBy(t => t)) {
                            if(FilterStartUtcTimestampItems.IndexOf(addTimestamp) == -1) {
                                FilterStartUtcTimestampItems.Add(addTimestamp);
                                // 現実問題 後にしか入らないだろ。システム日時変更なんか知らんし
                                //var sorted = FilterStartTimestampItems.OrderBy(t => t).ToList();
                                //FilterStartTimestampItems.Clear();
                                //FilterStartTimestampItems.AddRange(sorted);
                            }
                        }
                    }
                }
            };

            ImageItemCollectionManager.RemoveItems = (kind, oldItems, oldStartingIndex, oldViewModels) => {
                if(kind == ObservableCollectionKind.After) {
                    var removeStartTimestamp = oldViewModels[0].CaptureStartUtcTimestamp;
                    if(oldViewModels.Any(i => i == SelectedImageItem)) {
                        SelectedImageItem = null;
                    }
                    if(!ImageItemCollectionManager.ViewModels.Any(vm => vm.CaptureStartUtcTimestamp == removeStartTimestamp)) {
                        FilterStartUtcTimestampItems.Remove(removeStartTimestamp);
                    }
                    //TODO: SelectedFilterStartTimestamp への補正処理
                }
            };

            ImageItemCollectionManager.ResetItems = (kind, oldViewModels) => {
                FilterStartUtcTimestampItems.Clear();
                SelectedFilterStartUtcTimestamp = UnSelectedFilterTimestamp;
                SelectedImageItem = null;
            };
        }


        #endregion

        #region SingleModelViewModelBase

        protected override void CancelCore()
        {
            Model.CancelCapture();
            base.CancelCore();
        }

        #endregion
    }
}
