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
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;
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
        DateTime _selectedFilterStartTimestamp = UnSelectedFilterTimestamp;

        #endregion

        public CaptureGroupViewModel(CaptureGroupModel model)
            : base(model)
        {
            Items = GetInvokeUI(() => CollectionViewSource.GetDefaultView(ItemViewModels));
            Items.Filter = CaptureItemFilter;
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

        ObservableCollection<CaptureImageViewModel> ItemViewModels { get; } = new ObservableCollection<CaptureImageViewModel>();
        public ICollectionView Items { get; set; }

        public ObservableCollection<DateTime> FilterStartTimestampItems { get; } = new ObservableCollection<DateTime>();
        public DateTime SelectedFilterStartTimestamp
        {
            get { return this._selectedFilterStartTimestamp; }
            set
            {
                if(SetProperty(ref this._selectedFilterStartTimestamp, value)) {
                    Items.Refresh();
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

                    var startTimestamps = ItemViewModels
                        .Select(vm => vm.CaptureStartTimestamp)
                        .GroupBy(time => time)
                        .Select(g => g.First())
                        .OrderBy(time => time)
                    ;
                    FilterStartTimestampItems.Clear();
                    FilterStartTimestampItems.Add(UnSelectedFilterTimestamp);
                    FilterStartTimestampItems.AddRange(startTimestamps);
                    SelectedFilterStartTimestamp = UnSelectedFilterTimestamp;
                    Items.Refresh();

                    IsEnabledLastItemScroll = true;
                    IsEnabledAddCaptureStartTimestamp = true;
                });
            }

            return Task.CompletedTask;
        }
        private bool CaptureItemFilter(object obj)
        {
            if(SelectedFilterStartTimestamp == UnSelectedFilterTimestamp) {
                return true;
            }

            if(obj is CaptureImageViewModel image) {
                return image.CaptureStartTimestamp == SelectedFilterStartTimestamp;
            }

            return false;
        }


        #endregion

        #region SingleModelViewModelBase

        override protected void AttachModelEventsCore()
        {
            base.AttachModelEventsCore();

            Model.Items.CollectionChanged += Items_CollectionChanged;
        }

        override protected void DetachModelEventsCore()
        {
            base.DetachModelEventsCore();

            Model.Items.CollectionChanged -= Items_CollectionChanged;
        }

        #endregion

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                switch(e.Action) {
                    case NotifyCollectionChangedAction.Reset:
                        var oldItems = ItemViewModels;
                        ItemViewModels.Clear();
                        foreach(var oldItem in oldItems) {
                            oldItem.Dispose();
                        }
                        FilterStartTimestampItems.Clear();
                        SelectedFilterStartTimestamp = UnSelectedFilterTimestamp;
                        break;

                    case NotifyCollectionChangedAction.Move:
                        ItemViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        var removeStartTimestamp = ItemViewModels[e.OldStartingIndex].CaptureStartTimestamp;
                        var oldViewModels = ItemViewModels.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                        foreach(var counter in new Counter(oldViewModels.Count)) {
                            ItemViewModels.RemoveAt(e.OldStartingIndex);
                        }
                        foreach(var oldViewModel in oldViewModels) {
                            oldViewModel.Dispose();
                        }
                        if(!ItemViewModels.Any(vm => vm.CaptureStartTimestamp == removeStartTimestamp)) {
                            FilterStartTimestampItems.Remove(removeStartTimestamp);
                        }

                        //TODO: SelectedFilterStartTimestamp にへの補正処理
                        break;

                    case NotifyCollectionChangedAction.Add:
                        var vms = e.NewItems
                            .Cast<CaptureImageModel>()
                            .Select(i => new CaptureImageViewModel(i))
                            .ToList()
                        ;
                        ItemViewModels.AddRange(vms);
                        if(IsEnabledLastItemScroll) {
                            ScrollRequest.Raise(new ScrollNotification<CaptureImageViewModel>(vms.Last()));
                        }
                        if(IsEnabledAddCaptureStartTimestamp) {
                            if(FilterStartTimestampItems.Count == 0) {
                                FilterStartTimestampItems.Add(UnSelectedFilterTimestamp);
                            }
                            foreach(var addTimestamp in vms.GroupBy(vm => vm.CaptureStartTimestamp).Select(g => g.Key).OrderBy(t => t)) {
                                if(FilterStartTimestampItems.IndexOf(addTimestamp) == -1) {
                                    FilterStartTimestampItems.Add(addTimestamp);
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
