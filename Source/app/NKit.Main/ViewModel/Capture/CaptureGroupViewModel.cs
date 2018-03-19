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
using ContentTypeTextNet.NKit.Utility.ViewModell;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Capture
{
    public class CaptureGroupViewModel : RunnableViewModelBase<CaptureGroupModel, None>
    {
        #region variable

        RunState _initialRunState;

        #endregion

        public CaptureGroupViewModel(CaptureGroupModel model)
            : base(model)
        {
            Items = GetInvokeUI(() => CollectionViewSource.GetDefaultView(ItemViewModels));
        }

        #region property

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
                InitialRunState = RunState.Running;
                return Task.Run(() => {
                    Model.InitializeCaptureFiles();
                }).ContinueWith(t => {
                    if(t.IsFaulted) {
                        InitialRunState = RunState.Error;
                    } else {
                        InitialRunState = RunState.Finished;
                    }
                });
            }

            return Task.CompletedTask;
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
                        break;

                    case NotifyCollectionChangedAction.Move:
                        ItemViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        ItemViewModels.RemoveAt(e.OldStartingIndex);
                        foreach(var oldItem in e.OldItems.Cast<CaptureImageModel>()) {
                            oldItem.Dispose();
                        }
                        break;

                    case NotifyCollectionChangedAction.Add:
                        var vms = e.NewItems
                            .Cast<CaptureImageModel>()
                            .Select(i => new CaptureImageViewModel(i))
                            .ToList()
                        ;
                        ItemViewModels.AddRange(vms);
                        Items.MoveCurrentToLast();
                        break;
                }
            });
        }

    }
}
