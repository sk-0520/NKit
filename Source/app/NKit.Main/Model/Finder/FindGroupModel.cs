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
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindGroupModel : RunnableModelBase<None>
    {
        public FindGroupModel(FindGroupSetting findGroupSetting, IReadOnlyFinderSetting finderSetting, IReadOnlyNKitSetting nkitSetting)
        {
            FindGroupSetting = findGroupSetting;
            FinderSetting = finderSetting;
            NKitSetting = nkitSetting;
        }

        #region proeprty

        public FindGroupSetting FindGroupSetting { get; }
        public IReadOnlyFinderSetting FinderSetting { get; }
        public IReadOnlyNKitSetting NKitSetting { get; }
        /// <summary>
        /// 検索時に使用した設定。
        /// </summary>
        public FindGroupSetting CurrentFindGroupSetting { get; private set; }

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
                    } catch(Exception ex) {
                        Debug.WriteLine(ex);
                    }
                }
            }

            var isMatches = new SearchResultBase[] {
                result.Text,
                result.MicrosoftOffice,
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

                    // アクセスできるようなので中を検索していく
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
                return GetDefaultPreparaValueTask(false);
            }

            CachedUsingRootDirectoryPath = Environment.ExpandEnvironmentVariables(FindGroupSetting.RootDirectoryPath);

            if(!Directory.Exists(CachedUsingRootDirectoryPath)) {
                return GetDefaultPreparaValueTask(false);
            }

            var patternCreator = new SearchPatternCreator();

            try {
                CachedFileNamePattern = patternCreator.CreateRegex(FindGroupSetting.FileNameSearchPatternKind, FindGroupSetting.FileNameSearchPattern, FindGroupSetting.FileNameIgnoreCase);
            } catch(ArgumentException ex) {
                Debug.WriteLine(ex);
                return GetDefaultPreparaValueTask(false);
            }


            if(FindGroupSetting.FindFileContent) {
                try {
                    CachedFileContentPattern = patternCreator.CreateRegex(FindGroupSetting.FileContentSearchPatternKind, FindGroupSetting.FileContentSearchPattern, FindGroupSetting.FileContentIgnoreCase);

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
                    return GetDefaultPreparaValueTask(false);
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

                    var fileContentSearchResult = FileContentSearchResult.NotFound;
                    if(CurrentFindGroupSetting.FindFileContent && !string.IsNullOrEmpty(CurrentFindGroupSetting.FileContentSearchPattern)) {
                        fileContentSearchResult = SearchFlieContentPattern(file);
                    }

                    var findItem = new FindItemModel(dirInfo, file, fileNameSearchResult, fileContentSearchResult, NKitSetting);
                    Items.Add(findItem);
                }

                return None.Void;
            });
        }

        #endregion
    }
}
