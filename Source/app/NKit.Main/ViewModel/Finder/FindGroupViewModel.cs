using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModell;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindGroupViewModel : RunnableViewModelBase<FindGroupModel, None>
    {
        #region variable

        static readonly Regex FullMatchRegex = new Regex("");

        bool _expandedFileContent;

        bool _isEnabledHiddenFileFiler = true;
        bool _isEnabledFileNameFilter = true;
        bool _isEnabledFileContentFilter = true;

        string _easyFileNameFilterPattern;
        string _easyExtensionFilterPattern;

        FindItemViewModel _selectedItem;

        bool _showSelectedFileDetail;

        #endregion

        public FindGroupViewModel(FindGroupModel model)
            : base(model)
        {
            if(Dispatcher.CurrentDispatcher != Application.Current.Dispatcher) {
                Application.Current.Dispatcher.Invoke(() => {
                    Items = CollectionViewSource.GetDefaultView(ItemViewModels);
                });
            } else {
                Items = CollectionViewSource.GetDefaultView(ItemViewModels);
            }

            Items.Filter = FilterFileList;
        }

        #region property
        public string GroupName
        {
            get { return Model.FindGroupSetting.GroupName; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public string RootDirectoryPath
        {
            get { return Model.FindGroupSetting.RootDirectoryPath; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public SearchPatternKind FileNameSearchPatternKind
        {
            get { return Model.FindGroupSetting.FileNameSearchPatternKind; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public bool FileNameIgnoreCase
        {
            get { return Model.FindGroupSetting.FileNameIgnoreCase; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public string FileNameSearchPattern
        {
            get { return Model.FindGroupSetting.FileNameSearchPattern; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }
        public int DirectoryLimitLevel
        {
            get { return Model.FindGroupSetting.DirectoryLimitLevel; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }
        public bool FindFileContent
        {
            get { return Model.FindGroupSetting.FindFileContent; }
            set
            {
                if(SetPropertyValue(Model.FindGroupSetting, value)) {
                    if(FindFileContent && !ExpandedFileContent) {
                        ExpandedFileContent = true;
                    }
                }
            }
        }

        public SearchPatternKind FileContentSearchPatternKind
        {
            get { return Model.FindGroupSetting.FileContentSearchPatternKind; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public bool FileContentIgnoreCase
        {
            get { return Model.FindGroupSetting.FileContentIgnoreCase; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public string FileContentSearchPattern
        {
            get { return Model.FindGroupSetting.FileContentSearchPattern; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        #region office

        public bool MicrosoftOfficeContentIsEnabled
        {
            get { return Model.FindGroupSetting.MicrosoftOfficeContent.IsEnabled; }
            set { SetPropertyValue(Model.FindGroupSetting.MicrosoftOfficeContent, value, nameof(Model.FindGroupSetting.MicrosoftOfficeContent.IsEnabled)); }
        }

        public bool MicrosoftOfficeContentSheetName
        {
            get { return Model.FindGroupSetting.MicrosoftOfficeContent.SheetName; }
            set { SetPropertyValue(Model.FindGroupSetting.MicrosoftOfficeContent, value, nameof(Model.FindGroupSetting.MicrosoftOfficeContent.SheetName)); }
        }
        public bool MicrosoftOfficeContentPriorityFormula
        {
            get { return Model.FindGroupSetting.MicrosoftOfficeContent.PriorityFormula; }
            set { SetPropertyValue(Model.FindGroupSetting.MicrosoftOfficeContent, value, nameof(Model.FindGroupSetting.MicrosoftOfficeContent.PriorityFormula)); }
        }
        public bool MicrosoftOfficeContentCommentInCell
        {
            get { return Model.FindGroupSetting.MicrosoftOfficeContent.CommentInCell; }
            set { SetPropertyValue(Model.FindGroupSetting.MicrosoftOfficeContent, value, nameof(Model.FindGroupSetting.MicrosoftOfficeContent.CommentInCell)); }
        }
        public bool MicrosoftOfficeContentTextInShape
        {
            get { return Model.FindGroupSetting.MicrosoftOfficeContent.TextInShape; }
            set { SetPropertyValue(Model.FindGroupSetting.MicrosoftOfficeContent, value, nameof(Model.FindGroupSetting.MicrosoftOfficeContent.TextInShape)); }
        }

        #endregion

        #region xml/html

        public bool XmlHtmlContentIsEnabled
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IsEnabled; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IsEnabled)); }
        }

        public bool XmlContentIgnoreElement
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IgnoreElement; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IgnoreElement)); }
        }

        public bool XmlContentIgnoreAttribute
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IgnoreAttribute; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IgnoreAttribute)); }
        }

        public bool XmlContentIgnoreComment
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IgnoreComment; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IgnoreComment)); }
        }

        public bool XmlContentIgnoreHtmlLinkValue
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IgnoreHtmlLinkValue; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IgnoreHtmlLinkValue)); }
        }

        #endregion

        public bool ExpandedFileContent
        {
            get { return this._expandedFileContent; }
            set { SetProperty(ref this._expandedFileContent, value); }
        }

        ObservableCollection<FindItemViewModel> ItemViewModels { get; } = new ObservableCollection<FindItemViewModel>();
        public long EnabledItemsCount => ItemViewModels.Count(i => i.MatchedName && (!Model.CurrentFindGroupSetting.FindFileContent || (Model.CurrentFindGroupSetting.FindFileContent && i.MatchedContent)));
        public long TotalItemsCount => ItemViewModels.Count;

        public SortedSet<string> ExtensionItems { get; } = new SortedSet<string>();
        public ICollectionView Items { get; set; }

        public FindItemViewModel SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                if(SetProperty(ref this._selectedItem, value)) {
                    ShowSelectedFileDetail = SelectedItem != null;
                    SelectedItem?.Flash();
                }
            }
        }

        public FindMultiItemsViewModel MultiSelectedItem { get; } = new FindMultiItemsViewModel();

        public bool ShowSelectedFileDetail
        {
            get { return this._showSelectedFileDetail; }
            set
            {
                if(SetProperty(ref this._showSelectedFileDetail, value)) {
                }
            }
        }

        public bool IsEnabledHiddenFileFiler
        {
            get { return this._isEnabledHiddenFileFiler; }
            set
            {
                if(SetProperty(ref this._isEnabledHiddenFileFiler, value)) {
                    Items.Refresh();
                }
            }
        }

        public bool IsEnabledFileNameFilter
        {
            get { return this._isEnabledFileNameFilter; }
            set
            {
                if(SetProperty(ref this._isEnabledFileNameFilter, value)) {
                    Items.Refresh();
                }
            }
        }

        public bool IsEnabledFileContentFilter
        {
            get { return this._isEnabledFileContentFilter; }
            set
            {
                if(SetProperty(ref this._isEnabledFileContentFilter, value)) {
                    Items.Refresh();
                }
            }
        }

        Regex CachedEasyFileNameFilterPattern { get; set; } = FullMatchRegex;
        Regex CachedEasyExtensionFilterPattern { get; set; } = FullMatchRegex;
        public string EasyFileNameFilterPattern
        {
            get { return this._easyFileNameFilterPattern; }
            set
            {
                if(SetProperty(ref this._easyFileNameFilterPattern, value)) {
                    if(string.IsNullOrWhiteSpace(EasyFileNameFilterPattern)) {
                        CachedEasyFileNameFilterPattern = FullMatchRegex;
                    } else {
                        var pc = new SearchPatternCreator();
                        CachedEasyFileNameFilterPattern = pc.CreateRegex(SearchPatternKind.PartialMatch, EasyFileNameFilterPattern, true);
                    }
                    Items.Refresh();
                }
            }
        }

        public string EasyExtensionFilterPattern
        {
            get { return this._easyExtensionFilterPattern; }
            set
            {
                if(SetProperty(ref this._easyExtensionFilterPattern, value)) {
                    if(string.IsNullOrWhiteSpace(EasyExtensionFilterPattern)) {
                        CachedEasyExtensionFilterPattern = FullMatchRegex;
                    } else {
                        var pc = new SearchPatternCreator();
                        CachedEasyExtensionFilterPattern = pc.CreateRegex(SearchPatternKind.PartialMatch, EasyExtensionFilterPattern, true);
                    }
                    Items.Refresh();
                }
            }
        }



        #endregion

        #region command

        public ICommand UpRootDirectoryPathCommand => new DelegateCommand(() => {
            if(Model.UpRootDirectoryPath()) {
                RaisePropertyChanged(nameof(RootDirectoryPath));
            }
        });

        public ICommand SelectRootDirectoryPathCommand => new DelegateCommand(() => {
            if(Model.SelectRootDirectoryPathFromDialog()) {
                RaisePropertyChanged(nameof(RootDirectoryPath));
            }
        });

        #endregion

        #region function

        void RaiseCountPropertyChanged()
        {
            RaisePropertyChanged(nameof(EnabledItemsCount));
            RaisePropertyChanged(nameof(TotalItemsCount));
        }
        private bool FilterFileList(object obj)
        {
            var item = (FindItemViewModel)obj;

            if(IsEnabledHiddenFileFiler) {
                if(item.IsHiddenFile) {
                    return false;
                }
            }

            if(IsEnabledFileNameFilter) {
                if(!item.MatchedName) {
                    if(item.IsSelected) {
                        item.IsSelected = false;
                    }
                    return false;
                }
            }

            if(Model.CurrentFindGroupSetting.FindFileContent && IsEnabledFileContentFilter) {
                if(!item.MatchedContent) {
                    if(item.IsSelected) {
                        item.IsSelected = false;
                    }
                    return false;
                }
            }

            var filterEazyFileName = CachedEasyFileNameFilterPattern.IsMatch(item.FileName);
            if(!filterEazyFileName) {
                if(item.IsSelected) {
                    item.IsSelected = false;
                }
                return false;
            }

            var filterEazyExtension = CachedEasyExtensionFilterPattern.IsMatch(item.Extension);
            if(!filterEazyExtension) {
                if(item.IsSelected) {
                    item.IsSelected = false;
                }
                return false;
            }

            return true;
        }

        void RefreshSelectedItems()
        {
            var removeItems = MultiSelectedItem.Items
                .Where(i => !i.IsSelected)
                .ToList()
            ;

            foreach(var item in removeItems) {
                MultiSelectedItem.Items.Remove(item);
            }

            var addItems = ItemViewModels
                .Where(i => i.IsSelected)
                .Except(MultiSelectedItem.Items)
                .ToList()
            ;
            foreach(var item in addItems) {
                MultiSelectedItem.Items.Add(item);
            }
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

            foreach(var vm in ItemViewModels) {
                vm.PropertyChanged -= FindItemModel_PropertyChanged;
            }

            Model.Items.CollectionChanged -= Items_CollectionChanged;
        }

        #endregion

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                switch(e.Action) {
                    case NotifyCollectionChangedAction.Reset:
                        ItemViewModels.Clear();
                        MultiSelectedItem.Items.Clear();
                        RaiseCountPropertyChanged();
                        break;

                    case NotifyCollectionChangedAction.Move:
                        ItemViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        ItemViewModels.RemoveAt(e.OldStartingIndex);
                        foreach(var oldItem in e.OldItems.Cast<FindItemModel>()) {
                            oldItem.PropertyChanged -= FindItemModel_PropertyChanged;
                        }
                        RaiseCountPropertyChanged();
                        break;

                    case NotifyCollectionChangedAction.Add:
                        var vms = e.NewItems
                            .Cast<FindItemModel>()
                            .Select(i => new FindItemViewModel(i))
                            .ToList()
                        ;
                        ItemViewModels.AddRange(vms);

                        foreach(var vm in vms) {
                            vm.PropertyChanged += FindItemModel_PropertyChanged;
                            ExtensionItems.Add(vm.Extension);
                            RaisePropertyChanged(nameof(ExtensionItems));
                        }

                        RaiseCountPropertyChanged();
                        break;
                }
            });

        }

        private void FindItemModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FindItemViewModel.IsSelected)) {
                RefreshSelectedItems();
            }
        }
    }
}
