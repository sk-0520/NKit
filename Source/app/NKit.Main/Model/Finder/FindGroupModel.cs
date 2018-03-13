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
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Setting.File;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindGroupModel : RunnableAsyncModel<None>
    {
        public FindGroupModel(FinderManagerModel manager, FindGroupSetting findGroupSetting, IReadOnlyFinderSetting finderSetting, IReadOnlyFileSetting fileSetting, IReadOnlyNKitSetting nkitSetting)
        {
            Manager = manager;
            FindGroupSetting = findGroupSetting;
            FinderSetting = finderSetting;
            FileSetting = fileSetting;
            NKitSetting = nkitSetting;
        }

        #region proeprty

        FinderManagerModel Manager { get; }
        public FindGroupSetting FindGroupSetting { get; }
        public IReadOnlyFinderSetting FinderSetting { get; }
        public IReadOnlyFileSetting FileSetting { get; }
        public IReadOnlyNKitSetting NKitSetting { get; }
        /// <summary>
        /// 検索時に使用した各種設定。
        /// </summary>
        FindGroupCache Cache { get; } = new FindGroupCache();
        public IReadOnlyFindGroupCache CurrentCache => Cache;

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

        public void SetDefaultSetting()
        {
            Manager.SetDefaultSetting(FindGroupSetting);
        }

        TextSearchResult SearchNamePattern(FileSystemInfo fileSystemInfo)
        {
            if(string.IsNullOrEmpty(FindGroupSetting.FileNameSearchPattern)) {
                return TextSearchResult.NotFound;
            }

            var ts = new TextSearcher();
            return ts.Search(fileSystemInfo.Name, Cache.FileName);
        }

        bool SearchFileSize(FileInfo fileInfo)
        {
            var disabledMaxSize = Cache.Setting.FileSizeLimit.Tail == 0;

            if(Cache.Setting.FileSizeLimit.Head == 0 && disabledMaxSize) {
                return true;
            }

            var sizeRange = Range.Create(
                Cache.Setting.FileSizeLimit.Head,
                disabledMaxSize ? fileInfo.Length : Cache.Setting.FileSizeLimit.Tail
            );

            return sizeRange.IsIn(fileInfo.Length);
        }

        bool IsMatchFileAttributes(FileAttributes fileAttributes)
        {
            var maskFlags = FileAttributes.Normal | FileAttributes.Directory;
            var maskedValue = fileAttributes & ~maskFlags;
            var maskedSetting = Cache.Setting.FilePropertyFileAttributes & ~maskFlags;

            switch(Cache.Setting.FilePropertyFileAttributeFlagMatchKind) {
                case FlagMatchKind.Has:
                    return 0 != (maskedValue & maskedSetting);

                case FlagMatchKind.Approximate:
                    return (maskedValue & maskedSetting) == maskedSetting;

                case FlagMatchKind.Full:
                    return maskedValue == maskedSetting;

                default:
                    throw new NotImplementedException();
            }
        }

        FilePropertySearchResult SearchProperty(FileSystemInfo fileSystemInfo)
        {
            Debug.Assert(Cache.Setting.FindFileProperty);

            var result = new FilePropertySearchResult();

            result.IsEnabledAttributes = IsMatchFileAttributes(fileSystemInfo.Attributes);


            result.IsMatched = result.IsEnabledAttributes;

            return result;
        }

        FileContentSearchResult SearchFlieContentPattern(FileInfo fileInfo)
        {
            Debug.Assert(Cache.Setting.FindFileContent);
            Debug.Assert(Cache.FileNameKinds.Any());

            FileContentSearcher searcher;
            try {
                searcher = new FileContentSearcher(fileInfo);
            } catch(IOException ex) {
                Logger.Warning(ex);
                // ファイルアクセスできないとかそんなん。諦めよう
                return FileContentSearchResult.NotFound;
            }


            var result = new FileContentSearchResult();

            // ファイルアクセス読み込み系は往々にしてよく落ちる
            using(searcher) {
                if(Cache.FileNameKinds[FileNameKind.Text].IsMatch(fileInfo.Name)) {
                    try {
                        result.Text = searcher.SearchText(Cache.FileContent);
                    } catch(Exception ex) {
                        Logger.Warning(ex);
                    }
                }
                if(Cache.Setting.MicrosoftOfficeContent.IsEnabled && Cache.FileNameKinds[FileNameKind.MicrosoftOffice].IsMatch(fileInfo.Name)) {
                    try {
                        var parameter = new MicrosoftOfficeSearchParameter();
                        parameter.Excel = Cache.Setting.MicrosoftOfficeContent;
                        parameter.Word = Cache.Setting.MicrosoftOfficeContent;

                        result.MicrosoftOffice = searcher.SearchMicrosoftOffice(Cache.FileContent, parameter);
                    } catch(Exception ex) {
                        Logger.Warning(ex);
                    }
                }
                if(Cache.Setting.XmlHtmlContent.IsEnabled && Cache.FileNameKinds[FileNameKind.XmlHtml].IsMatch(fileInfo.Name)) {
                    try {
                        // XMLとか検索した記憶あんまねぇなぁ
                        result.XmlHtml = searcher.SearchXmlHtml(Cache.FileContent, Cache.Setting.XmlHtmlContent);
                    } catch(Exception ex) {
                        Logger.Warning(ex);
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

        IEnumerable<FileInfo> GetFiles(DirectoryInfo targetDirectory, string searchPattern, int limitLevel, CancellationToken cancelToken)
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
                        Logger.Warning(ex);
                        continue;
                    }

                    // アクセスできるようなので検索条件に合致すれば中を検索していく

                    // 隠しディレクトリのファイル列挙
                    if(!Cache.Setting.FindHiddenDirectory) {
                        if(dir.Attributes.HasFlag(FileAttributes.Hidden)) {
                            continue;
                        }
                    }

                    // 先頭が . のディレクトリのファイル列挙
                    if(!Cache.Setting.FindDotDirectory) {
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


            Cache.RootDirectoryPath = Environment.ExpandEnvironmentVariables(FindGroupSetting.RootDirectoryPath);

            if(!Directory.Exists(Cache.RootDirectoryPath)) {
                return GetDefaultPreparaValueAsync(false);
            }

            var patternCreator = new SearchPatternCreator();

            try {
                Cache.FileName = patternCreator.CreateRegex(FindGroupSetting.FileNameSearchPatternKind, FindGroupSetting.FileNameSearchPattern, !FindGroupSetting.FileNameCase);
            } catch(ArgumentException ex) {
                Logger.Error(ex);
                return GetDefaultPreparaValueAsync(false);
            }


            if(FindGroupSetting.FindFileContent) {
                try {
                    Cache.FileContent = patternCreator.CreateRegex(FindGroupSetting.FileContentSearchPatternKind, FindGroupSetting.FileContentSearchPattern, !FindGroupSetting.FileContentCase);

                    Cache.FileNameKinds = new[] {
                        new { Kind = FileNameKind.Text,  Pattern = FinderSetting.TextFileNamePattern, },
                        new { Kind = FileNameKind.MicrosoftOffice,  Pattern = FinderSetting.MicrosoftOfficeFileNamePattern, },
                        new { Kind = FileNameKind.XmlHtml,  Pattern = FinderSetting.XmlHtmlFileNamePattern, },
                    }.Select(i => new {
                        Kind = i.Kind,
                        Regex = patternCreator.CreateFileNameFilterRegex(i.Pattern)
                    }).ToDictionary(
                        i => i.Kind,
                        i => i.Regex
                    )
                    ;

                } catch(ArgumentException ex) {
                    Logger.Error(ex);
                    return GetDefaultPreparaValueAsync(false);
                }
            }

            Cache.Setting = SerializeUtility.Clone(FindGroupSetting);

            var oldItems = Items;
            Items.Clear();
            foreach(var oldItem in oldItems) {
                oldItem.Dispose();
            }

            return base.PreparationCoreAsync(cancelToken);
        }

        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            var dirInfo = new DirectoryInfo(Cache.RootDirectoryPath);
            var limitLevel = Cache.Setting.DirectoryLimitLevel;

            return Task.Run(() => {
                var files = GetFiles(dirInfo, "*", limitLevel, cancelToken);

                foreach(var fileInfo in files) {
                    cancelToken.ThrowIfCancellationRequested();

                    var fileNameSearchResult = SearchNamePattern(fileInfo);

                    var matchedFileSize = SearchFileSize(fileInfo);

                    var filePropertySearchResult = FilePropertySearchResult.NotFound;
                    if(Cache.Setting.FindFileProperty) {
                        filePropertySearchResult = SearchProperty(fileInfo);
                    }

                    var fileContentSearchResult = FileContentSearchResult.NotFound;
                    if(Cache.Setting.FindFileContent && !string.IsNullOrEmpty(Cache.Setting.FileContentSearchPattern)) {
                        if(Cache.Setting.IsEnabledFileContentSizeLimit && matchedFileSize) {
                            // ファイルサイズ制限による読み込み抑制が有効であればサイズもチェックしたうえで検索
                            fileContentSearchResult = SearchFlieContentPattern(fileInfo);
                        } else if(!Cache.Setting.IsEnabledFileContentSizeLimit) {
                            // ファイルサイズ制限による読み込み抑制が無効なら問答無用で検索
                            fileContentSearchResult = SearchFlieContentPattern(fileInfo);
                        }
                    }

                    var findItem = new FindItemModel(dirInfo, fileInfo, fileNameSearchResult, matchedFileSize, filePropertySearchResult, fileContentSearchResult, FileSetting.AssociationFile, NKitSetting);
                    Items.Add(findItem);
                }

                return None.Void;
            });
        }

        #endregion
    }
}
