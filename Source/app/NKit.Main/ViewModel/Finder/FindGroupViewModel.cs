using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindGroupViewModel : RunnableViewModelBase<FindGroupModel, None>
    {
        #region variable

        static readonly Regex FullMatchRegex = new Regex("");

        //bool _expandedFileContent;

        bool _isEnabledHiddenFileFiler = true;
        bool _isEnabledFileNameFilter = true;
        bool _isEnabledFileSizeFilter = true;
        bool _isEnabledFilePropertyFilter = true;
        bool _isEnabledFileContentFilter = true;

        string _easyFileNameFilterPattern;
        string _easyExtensionFilterPattern;

        FindItemViewModel _selectedItem;

        bool _showSelectedFileDetail;

        bool _outputDisplayItemOnly = true;
        bool _outputAbsolutePath = true;
        bool _outputIsDetail = false;

        #endregion

        public FindGroupViewModel(FindGroupModel model)
            : base(model)
        {
            FindItemCollectionManager = new ActionViewViewModelObservableCollectionManager<FindItemModel, FindItemViewModel>(Model.Items);
            Items = GetInvokeUI(() => CollectionViewSource.GetDefaultView(FindItemCollectionManager.ViewModels));

            Items.Filter = FilterFileList;

            AttachItemsCollectionChanged();
        }

        #region property

        public IReadOnlyFindGroupSetting CurrentFindGroupSetting => Model.CurrentCache.Setting;

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

        public bool FileNameCase
        {
            get { return Model.FindGroupSetting.FileNameCase; }
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
        public bool FindHiddenDirectory
        {
            get { return Model.FindGroupSetting.FindHiddenDirectory; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }
        public bool FindDotDirectory
        {
            get { return Model.FindGroupSetting.FindDotDirectory; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public bool FindDotFile
        {
            get { return Model.FindGroupSetting.FindDotFile; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }
        public bool FindFileNameOnly
        {
            get { return Model.FindGroupSetting.FindFileNameOnly; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public long FileSizeLimitMaximum
        {
            get { return Model.FindGroupSetting.FileSizeLimit.Tail; }
            set { SetPropertyValue(Model.FindGroupSetting, Range.Create(Model.FindGroupSetting.FileSizeLimit.Head, value), nameof(Model.FindGroupSetting.FileSizeLimit)); }
        }

        public bool FindFileProperty
        {
            get { return Model.FindGroupSetting.FindFileProperty; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public FlagMatchKind FilePropertyFileAttributeFlagMatchKind
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributeFlagMatchKind; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        #region attribute

        public bool FilePropertyAttributeIsArchive
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Archive); }
            set { SetPropertyAttribute(value, FileAttributes.Archive); }
        }
        public bool FilePropertyAttributeIsCompressed
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Compressed); }
            set { SetPropertyAttribute(value, FileAttributes.Compressed); }
        }
        public bool FilePropertyAttributeIsEncrypted
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Encrypted); }
            set { SetPropertyAttribute(value, FileAttributes.Encrypted); }
        }
        public bool FilePropertyAttributeIsHidden
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Hidden); }
            set { SetPropertyAttribute(value, FileAttributes.Hidden); }
        }
        public bool FilePropertyAttributeIsIntegrityStream
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.IntegrityStream); }
            set { SetPropertyAttribute(value, FileAttributes.IntegrityStream); }
        }
        public bool FilePropertyAttributeIsNoScrubData
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.NoScrubData); }
            set { SetPropertyAttribute(value, FileAttributes.NoScrubData); }
        }
        public bool FilePropertyAttributeIsNotContentIndexed
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.NotContentIndexed); }
            set { SetPropertyAttribute(value, FileAttributes.NotContentIndexed); }
        }
        public bool FilePropertyAttributeIsOffline
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Offline); }
            set { SetPropertyAttribute(value, FileAttributes.Offline); }
        }
        public bool FilePropertyAttributeIsReadOnly
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.ReadOnly); }
            set { SetPropertyAttribute(value, FileAttributes.ReadOnly); }
        }
        public bool FilePropertyAttributeIsReparsePoint
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.ReparsePoint); }
            set { SetPropertyAttribute(value, FileAttributes.ReparsePoint); }
        }
        public bool FilePropertyAttributeIsSparseFile
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.SparseFile); }
            set { SetPropertyAttribute(value, FileAttributes.SparseFile); }
        }
        public bool FilePropertyAttributeIsSystem
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.System); }
            set { SetPropertyAttribute(value, FileAttributes.System); }
        }
        public bool FilePropertyAttributeIsTemporary
        {
            get { return Model.FindGroupSetting.FilePropertyFileAttributes.HasFlag(FileAttributes.Temporary); }
            set { SetPropertyAttribute(value, FileAttributes.Temporary); }
        }

        #endregion

        public bool FindFileContent
        {
            get { return Model.FindGroupSetting.FindFileContent; }
            set
            {
                if(SetPropertyValue(Model.FindGroupSetting, value)) {
                    //if(FindFileContent && !ExpandedFileContent) {
                    //    ExpandedFileContent = true;
                    //}
                }
            }
        }

        public SearchPatternKind FileContentSearchPatternKind
        {
            get { return Model.FindGroupSetting.FileContentSearchPatternKind; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public bool FileContentCase
        {
            get { return Model.FindGroupSetting.FileContentCase; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public string FileContentSearchPattern
        {
            get { return Model.FindGroupSetting.FileContentSearchPattern; }
            set { SetPropertyValue(Model.FindGroupSetting, value); }
        }

        public bool IsEnabledFileContentSizeLimit
        {
            get { return Model.FindGroupSetting.IsEnabledFileContentSizeLimit; }
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

        #region pdf
        public bool PdfContentIsEnabled
        {
            get { return Model.FindGroupSetting.PdfContent.IsEnabled; }
            set { SetPropertyValue(Model.FindGroupSetting.PdfContent, value, nameof(Model.FindGroupSetting.PdfContent.IsEnabled)); }
        }

        #endregion

        #region xml/html

        public bool XmlHtmlContentIsEnabled
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IsEnabled; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IsEnabled)); }
        }

        public bool XmlContentElement
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.Element; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.Element)); }
        }

        public bool XmlContentText
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.Text; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.Text)); }
        }


        public bool XmlContentAttributeKey
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.AttributeKey; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.AttributeKey)); }
        }

        public bool XmlContentAttributeValue
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.AttributeValue; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.AttributeValue)); }
        }

        public bool XmlContentComment
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.Comment; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.Comment)); }
        }

        public bool XmlContentIgnoreHtmlLinkValue
        {
            get { return Model.FindGroupSetting.XmlHtmlContent.IgnoreHtmlLinkValue; }
            set { SetPropertyValue(Model.FindGroupSetting.XmlHtmlContent, value, nameof(Model.FindGroupSetting.XmlHtmlContent.IgnoreHtmlLinkValue)); }
        }

        #endregion

        //public bool ExpandedFileContent
        //{
        //    get { return this._expandedFileContent; }
        //    set { SetProperty(ref this._expandedFileContent, value); }
        //}

        ActionViewViewModelObservableCollectionManager<FindItemModel, FindItemViewModel> FindItemCollectionManager { get; }
        public long EnabledItemsCount => FindItemCollectionManager.ViewModels.Count(i => i.MatchedName && (!Model.CurrentCache.Setting.FindFileContent || (Model.CurrentCache.Setting.FindFileContent && i.MatchedContent)));
        public long TotalItemsCount => FindItemCollectionManager.ViewModels.Count;

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

        public bool OutputDisplayItemOnly
        {
            get { return this._outputDisplayItemOnly; }
            set { SetProperty(ref this._outputDisplayItemOnly, value); }
        }
        public bool OutputAbsolutePath
        {
            get { return this._outputAbsolutePath; }
            set { SetProperty(ref this._outputAbsolutePath, value); }
        }

        public bool OutputIsDetail
        {
            get { return this._outputIsDetail; }
            set { SetProperty(ref this._outputIsDetail, value); }
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

        public bool IsEnabledFileSizeFilter
        {
            get { return this._isEnabledFileSizeFilter; }
            set
            {
                if(SetProperty(ref this._isEnabledFileSizeFilter, value)) {
                    Items.Refresh();
                }
            }
        }

        public bool IsEnabledFilePropertyFilter
        {
            get { return this._isEnabledFilePropertyFilter; }
            set
            {
                if(SetProperty(ref this._isEnabledFilePropertyFilter, value)) {
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

        public InteractionRequest<Confirmation> SaveFileDialogRequest { get; } = new InteractionRequest<Confirmation>();


        #endregion

        #region command

        public ICommand UpRootDirectoryPathCommand => new DelegateCommand(() => {
            if(Model.UpRootDirectoryPath()) {
                RaisePropertyChanged(nameof(RootDirectoryPath));
            }
        });

        public ICommand SelectRootDirectoryPathCommand => new DelegateCommand(() => {
            // TODO: メッセージにした方がいいかも
            if(Model.SelectRootDirectoryPathFromDialog()) {
                RaisePropertyChanged(nameof(RootDirectoryPath));
            }
        });

        public ICommand SetDefaultSettingCommand => new DelegateCommand(() => {
            Model.SetDefaultSetting();
        });

        public ICommand OpenSelectedFileCommand => new DelegateCommand<MouseButtonEventArgs>(
            e => {
                if(e.LeftButton == MouseButtonState.Pressed) {
                    if(UIUtility.IsEnabledEventArea((DependencyObject)e.OriginalSource, new[] { typeof(ListViewItem) })) {
                        SelectedItem.OpenFileCommand.Execute(null);
                    }
                }
            },
            e => true//SelectedItem != null
        );

        //TODO: シェルメニュー表示
        //public ICommand OpenSelectedFilesContextMenuCommand => new DelegateCommand(
        //    () => {
        //        if(MultiSelectedItem.IsEnabled) {
        //        } else {
        //        }
        //    },
        //    () => SelectedItem != null || MultiSelectedItem.IsEnabled
        //);

        public ICommand FindItemsSelectionChangedCommand => new DelegateCommand<SelectionChangedEventArgs>(
            e => {
                foreach(var item in e.RemovedItems.OfType<ISelectable>()) {
                    item.IsSelected = false;
                }
                foreach(var item in e.AddedItems.OfType<ISelectable>()) {
                    item.IsSelected = true;
                }
            }
        );

        public DelegateCommand OutputListFileCommand => new DelegateCommand(
            () => {
                if(!CanRun || !GetOutputItemsIndex().Any()) {
                    return;
                }
                var outputItemsIndex = GetOutputItemsIndex().ToList();

                var confirmation = new Confirmation();
                var dialog = new SaveFileDialog();
                var list = new DialogFilterList() {
                    new DialogFilterItem("text", "*.txt"),
                    new DialogFilterItem("*", "*.*"),
                };
                dialog.Filter = list.FilterText;
                confirmation.Content = dialog;

                SaveFileDialogRequest.Raise(confirmation);
                if(confirmation.Confirmed) {
                    var outputPath = dialog.FileName;

                    Model.OutputListFile(outputPath, OutputAbsolutePath, OutputIsDetail, outputItemsIndex);
                }
            }//,
            //NOTE うごかねぇぇぇ, () => CanRun //&& GetOutputItemsIndex().Any()
        );

        #endregion

        #region function

        IEnumerable<int> GetOutputItemsIndex()
        {
            if(OutputDisplayItemOnly) {
                var displayItems = Items
                    .Cast<FindItemViewModel>()
                ;
                foreach(var displayItem in displayItems) {
                    var index = FindItemCollectionManager.ViewModels.IndexOf(displayItem);
                    // ログも出してないし判定いらない。。。
                    if(0 <= index) {
                        yield return index;
                    }
                }
            } else {
                foreach(var index in  Enumerable.Range(0, FindItemCollectionManager.ViewModels.Count)) {
                    yield return index;
                }
            }
        }

        void RaiseCountPropertyChanged()
        {
            RaisePropertyChanged(nameof(EnabledItemsCount));
            RaisePropertyChanged(nameof(TotalItemsCount));
            RaisePropertyChanged(nameof(CurrentFindGroupSetting));
        }
        private bool FilterFileList(object obj)
        {
            var item = (FindItemViewModel)obj;

            if(IsEnabledHiddenFileFiler) {
                if(item.IsHiddenFile) {
                    return false;
                }
            }

            if(IsEnabledFileNameFilter && !string.IsNullOrEmpty(Model.CurrentCache.Setting.FileNameSearchPattern)) {
                if(!item.MatchedName) {
                    if(item.IsSelected) {
                        item.IsSelected = false;
                    }
                    return false;
                }
            }

            if(IsEnabledFileSizeFilter && 0 < Model.CurrentCache.Setting.FileSizeLimit.Tail) { // Head はまぁいいっしょ...
                if(!item.MatchedSize) {
                    if(item.IsSelected) {
                        item.IsSelected = false;
                    }
                    return false;
                }
            }

            if(IsEnabledFilePropertyFilter && Model.CurrentCache.Setting.FindFileProperty) {
                if(!item.MatchedProperty) {
                    if(item.IsSelected) {
                        item.IsSelected = false;
                    }
                    return false;
                }
            }

            if(IsEnabledFileContentFilter && Model.CurrentCache.Setting.FindFileContent) {
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

            var addItems = FindItemCollectionManager.ViewModels
                .Where(i => i.IsSelected)
                .Except(MultiSelectedItem.Items)
                .ToList()
            ;
            foreach(var item in addItems) {
                MultiSelectedItem.Items.Add(item);
            }
        }

        void SetPropertyAttribute(bool flag, FileAttributes target, [CallerMemberName] string _callerMemberName = "")
        {
            var setValue = Model.FindGroupSetting.FilePropertyFileAttributes;
            if(flag) {
                setValue |= target;
            } else {
                setValue ^= target;
            }
            SetPropertyValue(Model.FindGroupSetting, setValue, nameof(Model.FindGroupSetting.FilePropertyFileAttributes), _callerMemberName);
        }

        void AttachItemsCollectionChanged()
        {
            FindItemCollectionManager.ToViewModel = (FindItemModel model) => new FindItemViewModel(model);

            FindItemCollectionManager.AddItems = (ObservableCollectionKind kind, IReadOnlyList<FindItemModel> newModels, IReadOnlyList<FindItemViewModel> newViewModels) => {
                if(kind == ObservableCollectionKind.After) {
                    foreach(var vm in newViewModels) {
                        vm.PropertyChanged += FindItemModel_PropertyChanged;
                        ExtensionItems.Add(vm.Extension);
                        RaisePropertyChanged(nameof(ExtensionItems));
                    }
                    RaiseCountPropertyChanged();
                }
            };

            FindItemCollectionManager.RemoveItems = (ObservableCollectionKind kind, IReadOnlyList<FindItemModel> oldItems, int oldStartingIndex, IReadOnlyList<FindItemViewModel> oldViewModels) => {
                if(kind == ObservableCollectionKind.After) {
                    foreach(var oldViewModel in oldViewModels) {
                        oldViewModel.PropertyChanged -= FindItemModel_PropertyChanged;
                    }
                    RaiseCountPropertyChanged();
                }
            };

            FindItemCollectionManager.ResetItems = (ObservableCollectionKind kind, IReadOnlyList<FindItemViewModel> oldViewModels) => {
                if(kind == ObservableCollectionKind.After) {
                    foreach(var oldItem in oldViewModels) {
                        oldItem.PropertyChanged -= FindItemModel_PropertyChanged;
                    }
                }
            };
        }

        #endregion

        #region SingleModelViewModelBase

        override protected void AttachModelEventsCore()
        {
            base.AttachModelEventsCore();

            //Model.Items.CollectionChanged += Items_CollectionChanged;
        }

        override protected void DetachModelEventsCore()
        {
            base.DetachModelEventsCore();

            foreach(var vm in FindItemCollectionManager.ViewModels) {
                vm.PropertyChanged -= FindItemModel_PropertyChanged;
            }

            //Model.Items.CollectionChanged -= Items_CollectionChanged;
        }
        /*
        protected override void OnChangedModelProperty(PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(RunState)) {
                var status = new[] {
                    Utility.Define.RunState.Cancel,
                    Utility.Define.RunState.Error,
                    Utility.Define.RunState.Finished,
                };
                if(status.Any(s => s == RunState)) {
                    //CommandManager.InvalidateRequerySuggested();
                    InvokeUI(() => CommandManager.InvalidateRequerySuggested());
                }
            }
            base.OnChangedModelProperty(e);
        }
        */
        #endregion

        private void FindItemModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FindItemViewModel.IsSelected)) {
                RefreshSelectedItems();
            }
        }
    }
}
