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
            ImageItems = GetInvokeUI(() => CollectionViewSource.GetDefaultView(ImageItemViewModels));
            ImageItems.Filter = CaptureItemFilter;
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


        ObservableCollection<CaptureImageViewModel> ImageItemViewModels { get; } = new ObservableCollection<CaptureImageViewModel>();
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

        public ICommand RemoveImageCommand => new DelegateCommand<CaptureImageViewModel>(
            vm => {
                var index = ImageItemViewModels.IndexOf(vm);
                Model.RemoveImageAt(index);
            }
        );

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

                    var startTimestamps = ImageItemViewModels
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


        #endregion

        #region SingleModelViewModelBase

        override protected void AttachModelEventsCore()
        {
            base.AttachModelEventsCore();

            Model.Images.CollectionChanged += Items_CollectionChanged;
        }

        override protected void DetachModelEventsCore()
        {
            base.DetachModelEventsCore();

            Model.Images.CollectionChanged -= Items_CollectionChanged;
        }

        protected override void CancelCore()
        {
            Model.CancelCapture();
            base.CancelCore();
        }

        #endregion

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                switch(e.Action) {
                    case NotifyCollectionChangedAction.Reset:
                        var oldItems = ImageItemViewModels;
                        ImageItemViewModels.Clear();
                        foreach(var oldItem in oldItems) {
                            oldItem.Dispose();
                        }
                        FilterStartUtcTimestampItems.Clear();
                        SelectedFilterStartUtcTimestamp = UnSelectedFilterTimestamp;
                        SelectedImageItem = null;
                        break;

                    case NotifyCollectionChangedAction.Move:
                        ImageItemViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        var removeStartTimestamp = ImageItemViewModels[e.OldStartingIndex].CaptureStartUtcTimestamp;
                        var oldViewModels = ImageItemViewModels.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                        foreach(var counter in new Counter(oldViewModels.Count)) {
                            ImageItemViewModels.RemoveAt(e.OldStartingIndex);
                        }
                        foreach(var oldViewModel in oldViewModels) {
                            oldViewModel.Dispose();
                        }
                        if(oldViewModels.Any(i => i == SelectedImageItem)) {
                            SelectedImageItem = null;
                        }
                        if(!ImageItemViewModels.Any(vm => vm.CaptureStartUtcTimestamp == removeStartTimestamp)) {
                            FilterStartUtcTimestampItems.Remove(removeStartTimestamp);
                        }

                        //TODO: SelectedFilterStartTimestamp にへの補正処理
                        break;

                    case NotifyCollectionChangedAction.Add:
                        var vms = e.NewItems
                            .Cast<CaptureImageModel>()
                            .Select(i => new CaptureImageViewModel(i))
                            .ToList()
                        ;
                        ImageItemViewModels.AddRange(vms);
                        if(IsEnabledLastItemScroll) {
                            ScrollRequest.Raise(new ScrollNotification<CaptureImageViewModel>(vms.Last()));
                        }
                        if(IsEnabledAddCaptureStartTimestamp) {
                            if(FilterStartUtcTimestampItems.Count == 0) {
                                FilterStartUtcTimestampItems.Add(UnSelectedFilterTimestamp);
                            }
                            foreach(var addTimestamp in vms.GroupBy(vm => vm.CaptureStartUtcTimestamp).Select(g => g.Key).OrderBy(t => t)) {
                                if(FilterStartUtcTimestampItems.IndexOf(addTimestamp) == -1) {
                                    FilterStartUtcTimestampItems.Add(addTimestamp);
                                    // 現実問題 後にしか入らないだろ。システム日時変更なんか知らんし
                                    //var sorted = FilterStartTimestampItems.OrderBy(t => t).ToList();
                                    //FilterStartTimestampItems.Clear();
                                    //FilterStartTimestampItems.AddRange(sorted);
                                }
                            }
                        }
                        break;
                }

            });
        }

    }
}
