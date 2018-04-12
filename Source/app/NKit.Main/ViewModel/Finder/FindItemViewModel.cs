using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ContentTypeTextNet.NKit.Main.ViewModel.File;
using ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindItemViewModel : SingleModelViewModelBase<FindItemModel>, ISelectable
    {
        #region variable

        bool _isSelected;

        #endregion

        public FindItemViewModel(FindItemModel model)
            : base(model)
        {
            GeneralDetail = new FindItemGeneralDetailViewModel(Model);
            BrowserDetail = new FindItemBrowserDetailViewModel(Model);
            TextDetail = new FindItemTextDetailViewModel(Model);
            MsOfficeDetail = new FindItemMicrosoftOfficeDetailViewModel(Model);
            PdfDetail = new FindItemPdfDetailViewModel(Model);
            XmlHtmlDetail = new FindItemXmlHtmlDetailViewModel(Model);

            DetailItems = new FindItemDetailViewModelBase[] {
                GeneralDetail,
                BrowserDetail,
                TextDetail,
                MsOfficeDetail,
                PdfDetail,
                XmlHtmlDetail,
            };
        }

        #region property

        public bool MatchedName => Model.FileNameSearchResult.IsMatched;
        public bool MatchedSize => Model.MatchedFileSize;
        public bool MatchedProperty => Model.FilePropertySearchResult.IsMatched;
        public bool MatchedContent => Model.FileContentSearchResult.IsMatched;

        public IReadOnlyList<TextSearchMatch> FileNameMatches => Model.FileNameSearchResult.Matches;

        public string RelativeDirectoryPath => Model.RelativeDirectoryPath;

        public int RelativeDirectoryDepth => RelativeDirectoryPath.Split(Path.DirectorySeparatorChar).Length;

        public string FilePath => GeneralDetail.FilePath;
        public string FileName => GeneralDetail.FileName;
        public string FileNameWithoutExtension => GeneralDetail.FileNameWithoutExtension;
        public string Extension => GeneralDetail.Extension;
        public string DirectoryPath => GeneralDetail.DirectoryPath;
        public long FileSize => GeneralDetail.FileSize;
        public bool IsHiddenFile => GeneralDetail.IsHiddenFile;

        public FindItemDetailViewModelBase[] DetailItems { get; }

        public FindItemGeneralDetailViewModel GeneralDetail { get; }
        public FindItemTextDetailViewModel TextDetail { get; }
        public FindItemMicrosoftOfficeDetailViewModel MsOfficeDetail { get; }
        public FindItemPdfDetailViewModel PdfDetail { get; }
        public FindItemXmlHtmlDetailViewModel XmlHtmlDetail { get; }
        public FindItemBrowserDetailViewModel BrowserDetail { get; }

        #endregion

        #region command

        public ICommand CopyFileSizeCommand => new DelegateCommand(() => Model.CopyFileSize());

        public ICommand OpenFileCommand => new DelegateCommand(() => Model.OpenFile());
        public ICommand BrowseFileCommand => new DelegateCommand(() => Model.BrowseFile());
        public ICommand OpenDirectoryCommand => new DelegateCommand(() => Model.OpenDirectory());
        public ICommand ShowPropertyCommand => new DelegateCommand(() => Model.ShowProperty());

        public ICommand CopyFileCommand => new DelegateCommand(() => Model.CopyFile());
        public ICommand CopyNameWithExtensionCommand => new DelegateCommand(() => Model.CopyNameWithExtension());
        public ICommand CopyNameWithoutExtensionCommand => new DelegateCommand(() => Model.CopyNameWithoutExtension());
        public ICommand CopyDirectoryCommand => new DelegateCommand(() => Model.CopyDirectory());

        public ICommand OpenTextFileCommand
        {
            get
            {
                return new DelegateCommand<TextSearchMatch>(match => {
                    var parameter = CreateCommonAssociationOpenParameter(match);
                    Model.OpenAssociationFile(AssociationFileKind.Text, parameter);
                });
            }
            set { /* TwoWay ダミー */}
        }
        public ICommand OpenMsOfficeExcelFileCommand
        {
            get
            {
                return new DelegateCommand<TextSearchMatch>(match => {
                    var parameter = CreateCommonAssociationOpenParameter(match);
                    parameter.SpreadSeet = (AssociationSpreadSeetParameter)match.Tag;
                    Model.OpenAssociationFile(AssociationFileKind.MicrosoftOfficeExcel, parameter);
                });
            }
            //set { /* TwoWay ダミー */}
        }
        public ICommand OpenMsOfficeWordFileCommand
        {
            get
            {
                return new DelegateCommand<TextSearchMatch>(match => {
                    var parameter = CreateCommonAssociationOpenParameter(match);
                    parameter.Document = (AssociationDocumentParameter)match.Tag;
                    Model.OpenAssociationFile(AssociationFileKind.MicrosoftOfficeWord, parameter);
                });
            }
            set { /* TwoWay ダミー */}
        }

        public ICommand OpenPdfFileCommand
        {
            get
            {
                return new DelegateCommand<TextSearchMatch>(match => {
                    var parameter = CreateCommonAssociationOpenParameter(match);
                    parameter.Document = (AssociationDocumentParameter)match.Tag;
                    Model.OpenAssociationFile(AssociationFileKind.Pdf, parameter);
                });
            }
            set { /* TwoWay ダミー */}
        }
        public ICommand OpenXmlHtmlFileCommand
        {
            get
            {
                return new DelegateCommand<TextSearchMatch>(match => {
                    var parameter = CreateCommonAssociationOpenParameter(match);
                    Model.OpenAssociationFile(AssociationFileKind.XmlHtml, parameter);
                });
            }
            set { /* TwoWay ダミー */}
        }

        #endregion

        #region function

        AssociationOpenParameter CreateCommonAssociationOpenParameter(TextSearchMatch match)
        {
            var parameter = new AssociationOpenParameter() {
                LineNumber = match.DisplayLineNumber,
                CharacterPostion = match.DisplayCharacterPosition,
                CharacterLength = match.Length,
            };

            return parameter;
        }

        public void Flash()
        {
            var propertyNames = new[]
            {
                //nameof(ContentIsText),
                //nameof(ContentText),
                //nameof(ContentMatches),

                //nameof(ContentIsMsOffice),
                //nameof(ContentMsOffice),
                //nameof(ContentMsOfficeWordElements),

                //nameof(ContentIsXmlHtml),
                //nameof(ContentXmlHtml),
                //nameof(ContentXmlHtmlMatches),

                nameof(OpenTextFileCommand),
                nameof(OpenXmlHtmlFileCommand),
                nameof(OpenMsOfficeExcelFileCommand),
                nameof(OpenMsOfficeWordFileCommand),
            };

            foreach(var propertyName in propertyNames) {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion

        #region ISelectable

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        #endregion

    }
}
