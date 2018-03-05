using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.ViewModel.File;
using ContentTypeTextNet.NKit.Utility.ViewModell;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindItemViewModel : SingleModelViewModelBase<FindItemModel>
    {
        #region variable

        bool _isSelected;

        #endregion

        public FindItemViewModel(FindItemModel model)
            : base(model)
        {
            FileInfo = (FileInfo)model.FileSystemInfo;
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

        public bool ContentIsXmlHtml => false;//Model.FileContentSearchResult.Text != null;

        public FileTypeViewModel FileType => new FileTypeViewModel(Model.FileType);

        public FileHashViewModel FileHash => new FileHashViewModel(Model.FileHash);

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        #endregion

        #region command

        public ICommand CopyFileSize => new DelegateCommand(() => Model.CopyFileSize());

        public ICommand OpenFile => new DelegateCommand(() => Model.OpenFile());
        public ICommand OpenDirectory => new DelegateCommand(() => Model.OpenDirectory());
        public ICommand ShowProperty => new DelegateCommand(() => Model.ShowProperty());

        public ICommand CopyFile => new DelegateCommand(() => Model.CopyFile());
        public ICommand CopyNameWithExtension => new DelegateCommand(() => Model.CopyNameWithExtension());
        public ICommand CopyNameWithoutExtension => new DelegateCommand(() => Model.CopyNameWithoutExtension());
        public ICommand CopyDirectory => new DelegateCommand(() => Model.CopyDirectory());

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
            };

            foreach(var propertyName in propertyNames) {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion
    }
}
