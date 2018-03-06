using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.ViewModel.File;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.ViewModell;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindItemViewModel : SingleModelViewModelBase<FindItemModel>
    {
        #region variable

        bool _isSelected;

        bool _isSelectedContentGeneral;
        bool _isSelectedContentText;
        bool _isSelectedContentMsOffice;
        bool _isSelectedContentXmlHtml;

        #endregion

        public FindItemViewModel(FindItemModel model)
            : base(model)
        {
            FileInfo = (FileInfo)model.FileSystemInfo;

            // だっせぇ
            if(Model.FileContentSearchResult.IsMatched) {
                if(ContentIsText) {
                    IsSelectedContentText = true;
                }
                if(ContentIsXmlHtml) {
                    IsSelectedContentXmlHtml = true;
                }
                if(ContentIsMsOffice) {
                    IsSelectedContentMsOffice = true;
                }
            }
            if(!(ContentIsText || ContentIsXmlHtml | ContentIsMsOffice)) {
                IsSelectedContentGeneral = true;
            }
        }

        #region property

        FileInfo FileInfo { get; }

        public bool MatchedName => Model.FileNameSearchResult.IsMatched;
        public bool MatchedContent => Model.FileContentSearchResult.IsMatched;

        public string RelativeDirectoryPath => Model.RelativeDirectoryPath;
        public string FilePath => Model.FileSystemInfo.FullName;
        public string FileName => Model.FileSystemInfo.Name;
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);
        public string Extension => Path.GetExtension(FilePath).Replace(".", string.Empty);
        public string DirectoryPath => Path.GetDirectoryName(FilePath);
        public long FileSize => FileInfo.Length;
        public bool IsHiddenFile => FileInfo.Attributes.HasFlag(FileAttributes.Hidden);

        public IReadOnlyList<TextSearchMatch> FileNameMatches => Model.FileNameSearchResult.Matches;

        public bool ContentIsText => Model.FileContentSearchResult.Text != null && Model.FileContentSearchResult.IsMatched;
        public TextSearchResult ContentText => Model.FileContentSearchResult.Text;

        public IReadOnlyList<TextSearchMatch> ContentMatches
        {
            get
            {
                if(!ContentIsText) {
                    return null;
                }
                return ContentText.Matches;
            }
            set { /* TwoWay ダミー */}
        }

        public bool ContentIsMsOffice => Model.FileContentSearchResult.MicrosoftOffice != null && Model.FileContentSearchResult.MicrosoftOffice.IsMatched;
        public MicrosoftOfficeSearchResultBase ContentMsOffice => Model.FileContentSearchResult.MicrosoftOffice;

        public IReadOnlyList<TextSearchMatch> ContentMsOfficeWordElements
        {
            get
            {
                if(!ContentIsMsOffice) {
                    return null;
                }

                var wordResult = ContentMsOffice as MicrosoftOfficeWordSearchResult;
                if(wordResult == null) {
                    return null;
                }

                var list = new List<TextSearchMatch>();

                foreach(var result in wordResult.ElementResults) {
                    switch(result) {
                        case MicrosoftOfficeWordParagraphSearchResult paragraph:
                            list.AddRange(paragraph.TextResult.Matches);
                            break;

                        case MicrosoftOfficeWordTableSearchResult table:
                            list.AddRange(table.CellResults.SelectMany(c => c.TextResult.Matches));
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                return list;
            }
            set { /* TwoWay ダミー */}
        }

        public bool ContentIsXmlHtml => Model.FileContentSearchResult.XmlHtml != null;
        public XmlHtmlSearchResult ContentXmlHtml => Model.FileContentSearchResult.XmlHtml;
        public IReadOnlyList<TextSearchMatch> ContentXmlHtmlMatches
        {
            get
            {
                if(!ContentIsXmlHtml) {
                    return null;
                }

                var list = new List<TextSearchMatch>(ContentXmlHtml.Results.Count);
                foreach(var result in ContentXmlHtml.Results) {
                    if(result.NodeType == HtmlAgilityPack.HtmlNodeType.Comment) {
                        var comment = (XmlHtmlCommentSearchResult)result;
                        list.AddRange(comment.Matches);
                    } else if(result.NodeType == HtmlAgilityPack.HtmlNodeType.Text) {
                        var text = (XmlHtmlTextSearchResult)result;
                        list.AddRange(text.Matches);
                    } else {
                        var element = (XmlHtmlElementSearchResult)result;
                        list.AddRange(element.ElementResult.Matches);
                        foreach(var attribute in element.AttributeKeyResults) {
                            list.AddRange(attribute.KeyResult.Matches);
                            list.AddRange(attribute.ValueResult.Matches);
                        }
                    }
                }

                return list;
            }
            set { /* TwoWay ダミー */}
        }

        public FileTypeViewModel FileType => new FileTypeViewModel(Model.FileType);

        public FileHashViewModel FileHash => new FileHashViewModel(Model.FileHash);

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        public bool IsSelectedContentGeneral
        {
            get { return this._isSelectedContentGeneral; }
            set { SetProperty(ref this._isSelectedContentGeneral, value); }
        }
        public bool IsSelectedContentText
        {
            get { return this._isSelectedContentText; }
            set { SetProperty(ref this._isSelectedContentText, value); }
        }
        public bool IsSelectedContentMsOffice
        {
            get { return this._isSelectedContentMsOffice; }
            set { SetProperty(ref this._isSelectedContentMsOffice, value); }
        }
        public bool IsSelectedContentXmlHtml
        {
            get { return this._isSelectedContentXmlHtml; }
            set { SetProperty(ref this._isSelectedContentXmlHtml, value); }
        }


        #endregion

        #region function

        AssociationOpenParameter CreateCommonAssociationOpenParameter(TextSearchMatch match)
        {
            var parameter = new AssociationOpenParameter() {
                LineNumber = match.DisplayLineNumber,
                CharacterPostion = match.DisplayCharacterPostion,
                CharacterLength = match.Length,
            };

            return parameter;
        }

        #endregion

        #region command

        public ICommand CopyFileSizeCommand => new DelegateCommand(() => Model.CopyFileSize());

        public ICommand OpenFileCommand => new DelegateCommand(() => Model.OpenFile());
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
        public void Flash()
        {
            var propertyNames = new[]
            {
                nameof(ContentIsText),
                nameof(ContentText),
                nameof(ContentMatches),

                nameof(ContentIsMsOffice),
                nameof(ContentMsOffice),
                nameof(ContentMsOfficeWordElements),

                nameof(ContentIsXmlHtml),
                nameof(ContentXmlHtml),
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
    }
}
