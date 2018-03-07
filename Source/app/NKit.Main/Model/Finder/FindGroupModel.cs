using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.NKit.Setting.File;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindGroupModel : RunnableAsyncModel<None>
    {
        public FindGroupModel(FindGroupSetting findGroupSetting, IReadOnlyFinderSetting finderSetting, IReadOnlyFileSetting fileSetting, IReadOnlyNKitSetting nkitSetting)
        {
            FindGroupSetting = findGroupSetting;
            FinderSetting = finderSetting;
            FileSetting = fileSetting;
            NKitSetting = nkitSetting;
        }

        #region proeprty

        public FindGroupSetting FindGroupSetting { get; }
        public IReadOnlyFinderSetting FinderSetting { get; }
        public IReadOnlyFileSetting FileSetting { get; }
        public IReadOnlyNKitSetting NKitSetting { get; }
        /// <summary>
        /// 検索時に使用した設定。
        /// </summary>
        public IReadOnlyFindGroupSetting CurrentFindGroupSetting { get; private set; }

        string CachedUsingRootDirectoryPath { get; set; }

        Regex CachedFileNamePattern { get; set; }
        Regex CachedFileContentPattern { get; set; }
        Dictionary<FileNameKind, Regex> CachedFileNameKindPatterns { get; set; } = new Dictionary<FileNameKind, Regex>();

        public ObservableCollection<FindItemModel> Items { get; } = new ObservableCollection<FindItemModel>();

        #endregion

        #region function

        public bool UpRootDirectoryPath()
        {
            var path = FindGroupSetting.RootDirectoryPath;
            if(string.IsNullOrWhiteSpace(path)) {
                return false;
            }

            FindGroupSetting.RootDirectoryPath = PathUtility.GetUpDirectoryPath(path);

            return FindGroupSetting.RootDirectoryPath != path;
        }

        public bool SelectRootDirectoryPathFromDialog()
        {
            var dialogs = new Dialogs();
            using(var dialog = dialogs.CreateFolderDialog(FindGroupSetting.RootDirectoryPath)) {
                var dialogResult = dialog.ShowDialog();
                if(dialogResult.GetValueOrDefault()) {
                    FindGroupSetting.RootDirectoryPath = dialog.SelectedPath;
                    return true;
                }
            }

            return false;
        }

        TextSearchResult SearchNamePattern(FileSystemInfo fileSystemInfo)
        {
            if(string.IsNullOrEmpty(FindGroupSetting.FileNameSearchPattern)) {
                return TextSearchResult.NotFound;
            }

            var ts = new TextSearcher();
            return ts.Search(fileSystemInfo.Name, CachedFileNamePattern);
        }

        bool SearchFileSize(FileSystemInfo fileSystemInfo)
        {
            // ここにきてれば絶対ファイルっしょ
            var file = (FileInfo)fileSystemInfo;

            var disabledMaxSize = CurrentFindGroupSetting.FileSizeLimit.Tail == 0;

            if(CurrentFindGroupSetting.FileSizeLimit.Head == 0 && disabledMaxSize) {
                return true;
            }

            var sizeRange = Range.Create(
                CurrentFindGroupSetting.FileSizeLimit.Head,
                disabledMaxSize ? file.Length : CurrentFindGroupSetting.FileSizeLimit.Tail
            );

            return sizeRange.IsIn(file.Length);
        }

        bool IsMatchFileAttributes(FileAttributes fileAttributes)
        {
            var maskFlags = FileAttributes.Normal | FileAttributes.Directory;
            var maskedValue = fileAttributes & ~maskFlags;
            var maskedSetting = CurrentFindGroupSetting.FilePropertyFileAttributes & ~maskFlags;

            switch(CurrentFindGroupSetting.FilePropertyFileAttributeFlagMatch) {
                case FlagMatch.Has:
                    return 0 != (maskedValue & maskedSetting);

                case FlagMatch.Approximate:
                    return (maskedValue & maskedSetting) == maskedSetting;

                case FlagMatch.Full:
                    return maskedValue == maskedSetting;

                default:
                    throw new NotImplementedException();
            }
        }

        FilePropertySearchResult SearchProperty(FileSystemInfo fileSystemInfo)
        {
            Debug.Assert(CurrentFindGroupSetting.FindFileProperty);

            var result = new FilePropertySearchResult();

            // ここにきてれば絶対ファイルっしょ
            var file = (FileInfo)fileSystemInfo;

            result.IsEnabledAttributes = IsMatchFileAttributes(file.Attributes);


            result.IsMatched = result.IsEnabledAttributes;

            return result;
        }

        FileContentSearchResult SearchFlieContentPattern(FileSystemInfo fileSystemInfo)
        {
            Debug.Assert(CurrentFindGroupSetting.FindFileContent);
            Debug.Assert(CachedFileNameKindPatterns.Any());

            // ここにきてれば絶対ファイルっしょ
            var file = (FileInfo)fileSystemInfo;
            FileContentSearcher searcher;
            try {
                searcher = new FileContentSearcher(file);
            } catch(IOException ex) {
                Debug.WriteLine(ex);
                // ファイルアクセスできないとかそんなん。諦めよう
                return FileContentSearchResult.NotFound;
            }


            var result = new FileContentSearchResult();

            // ファイルアクセス読み込み系は往々にしてよく落ちる
            using(searcher) {
                if(CachedFileNameKindPatterns[FileNameKind.Text].IsMatch(file.Name)) {
                    try {
                        result.Text = searcher.SearchText(CachedFileContentPattern);
                    } catch(Exception ex) {
                        Debug.WriteLine(ex);
                    }
                }
                if(CurrentFindGroupSetting.MicrosoftOfficeContent.IsEnabled && CachedFileNameKindPatterns[FileNameKind.MicrosoftOffice].IsMatch(file.Name)) {
                    try {
                        var parameter = new MicrosoftOfficeSearchParameter();
                        parameter.Excel = CurrentFindGroupSetting.MicrosoftOfficeContent;
                        parameter.Word = CurrentFindGroupSetting.MicrosoftOfficeContent;

                        result.MicrosoftOffice = searcher.SearchMicrosoftOffice(CachedFileContentPattern, parameter);
                    } catch(Exception ex) {
                        Debug.WriteLine(ex);
                    }
                }
                if(CurrentFindGroupSetting.XmlHtmlContent.IsEnabled && CachedFileNameKindPatterns[FileNameKind.XmlHtml].IsMatch(file.Name)) {
                    try {
                        // XMLとか検索した記憶あんまねぇなぁ
                        result.XmlHtml = searcher.SearchXmlHtml(CachedFileContentPattern, CurrentFindGroupSetting.XmlHtmlContent);
                    } catch(Exception ex) {
                        Debug.WriteLine(ex);
                    }
                }
            }

            var isMatches = new SearchResultBase[] {
                result.Text,
                result.MicrosoftOffice,
                result.XmlHtml
            };

            result.IsMatched = isMatches.Any(i => i?.IsMatched ?? false);

            return result;
        }

        IEnumerable<FileSystemInfo> GetFiles(DirectoryInfo targetDirectory, string searchPattern, int limitLevel, CancellationToken cancelToken)
        {
            var files = targetDirectory.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
            foreach(var file in files) {
                yield return file;
            }

            if(limitLevel != 1) {
                var dirs = targetDirectory.EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
                foreach(var dir in dirs) {
                    // アクセスできないディレクトリを前もって列挙しておく
                    try {
                        dir.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
                    } catch(Exception ex) {
                        Debug.WriteLine(ex);
                        continue;
                    }

                    // アクセスできるようなので検索条件に合致すれば中を検索していく

                    // 隠しディレクトリのファイル列挙
                    if(!CurrentFindGroupSetting.FindHiddenDirectory) {
                        if(dir.Attributes.HasFlag(FileAttributes.Hidden)) {
                            continue;
                        }
                    }

                    // 先頭が . のディレクトリのファイル列挙
                    if(!CurrentFindGroupSetting.FindDotDirectory) {
                        if(dir.Name.StartsWith(".")) {
                            continue;
                        }
                    }

                    var filesInDir = GetFiles(dir, searchPattern, limitLevel - 1, cancelToken);
                    foreach(var file in filesInDir) {
                        yield return file;
                    }
                }
            }
        }

        #endregion

        #region RunnableModelBase

        protected override Task<PreparaResult<None>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            if(string.IsNullOrEmpty(FindGroupSetting.RootDirectoryPath)) {
                return GetDefaultPreparaValueAsync(false);
            }

            CachedUsingRootDirectoryPath = Environment.ExpandEnvironmentVariables(FindGroupSetting.RootDirectoryPath);

            if(!Directory.Exists(CachedUsingRootDirectoryPath)) {
                return GetDefaultPreparaValueAsync(false);
            }

            var patternCreator = new SearchPatternCreator();

            try {
                CachedFileNamePattern = patternCreator.CreateRegex(FindGroupSetting.FileNameSearchPatternKind, FindGroupSetting.FileNameSearchPattern, !FindGroupSetting.FileNameCase);
            } catch(ArgumentException ex) {
                Debug.WriteLine(ex);
                return GetDefaultPreparaValueAsync(false);
            }


            if(FindGroupSetting.FindFileContent) {
                try {
                    CachedFileContentPattern = patternCreator.CreateRegex(FindGroupSetting.FileContentSearchPatternKind, FindGroupSetting.FileContentSearchPattern, !FindGroupSetting.FileContentCase);

                    CachedFileNameKindPatterns = new[] {
                        new { Kind = FileNameKind.Text,  Pattern = FinderSetting.TextNamePattern, },
                        new { Kind = FileNameKind.MicrosoftOffice,  Pattern = FinderSetting.MicrosoftOfficeNamePattern, },
                        new { Kind = FileNameKind.XmlHtml,  Pattern = FinderSetting.XmlHtmlNamePattern, },
                    }.Select(i => new {
                        Kind = i.Kind,
                        Regex = patternCreator.CreateFileNameFilterRegex(i.Pattern)
                    }).ToDictionary(
                        i => i.Kind,
                        i => i.Regex
                    )
                    ;

                } catch(ArgumentException ex) {
                    Debug.WriteLine(ex);
                    return GetDefaultPreparaValueAsync(false);
                }
            }

            CurrentFindGroupSetting = (FindGroupSetting)FindGroupSetting.Clone();


            return base.PreparationCoreAsync(cancelToken);
        }

        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            Items.Clear();

            var dirInfo = new DirectoryInfo(CachedUsingRootDirectoryPath);
            var limitLevel = CurrentFindGroupSetting.DirectoryLimitLevel;

            return Task.Run(() => {
                var files = GetFiles(dirInfo, "*", limitLevel, cancelToken);

                foreach(var file in files) {
                    cancelToken.ThrowIfCancellationRequested();

                    var fileNameSearchResult = SearchNamePattern(file);

                    var matchedFileSize = SearchFileSize(file);

                    var filePropertySearchResult = FilePropertySearchResult.NotFound;
                    if(CurrentFindGroupSetting.FindFileProperty) {
                        filePropertySearchResult = SearchProperty(file);
                    }

                    var fileContentSearchResult = FileContentSearchResult.NotFound;
                    if(CurrentFindGroupSetting.FindFileContent && !string.IsNullOrEmpty(CurrentFindGroupSetting.FileContentSearchPattern)) {
                        // あんまりお行儀良くない気がしたのでファイルサイズチェックに該当したものだけファイル内検索するようにした
                        if(matchedFileSize) {
                            fileContentSearchResult = SearchFlieContentPattern(file);
                        }
                    }

                    var findItem = new FindItemModel(dirInfo, file, fileNameSearchResult, matchedFileSize, filePropertySearchResult, fileContentSearchResult, FileSetting.AssociationFile, NKitSetting);
                    Items.Add(findItem);
                }

                return None.Void;
            });
        }

        #endregion
    }
}
