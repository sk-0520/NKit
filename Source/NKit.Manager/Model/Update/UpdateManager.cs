using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model.Log;

namespace ContentTypeTextNet.NKit.Manager.Model.Update
{
    public class UpdateManager : ManagerBase
    {
        #region define

        struct UriResult
        {
            public UriResult(string value, Uri uri)
            {
                Value = value;
                Uri= uri;
            }

            #region property

            public string Value { get; }
            public Uri Uri { get; }

            #endregion
        }

        struct MatchValue
        {
            public MatchValue(bool isMatch, string value)
            {
                IsMatch = isMatch;
                Value = value;
            }

            #region property

            public bool IsMatch { get; }
            public string Value { get; }

            #endregion
        }

        #endregion

        public UpdateManager(ILogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("UP");
        }

        #region property

        ILogFactory LogFactory { get; }
        ILogger Logger { get; }

        public bool HasUpdate { get; private set; }

        public Uri DownloadUri { get; private set; }
        public Uri ReleaseNoteUri { get; private set; }

        public ReleaseNoteItem ReleaseNote { get; private set; } = new ReleaseNoteItem();

        #endregion


        Uri CombineUri(string baseUri, params string[] hierarchies)
        {
            var list = new List<string>() {
                baseUri,
            };
            list.AddRange(hierarchies);
            //hierarchy
            var originUri = list.Aggregate((left, right) => left.TrimEnd('/') + "/" + right.TrimStart('/'));

            return new Uri(originUri);
        }

        Task<UriResult> GetStringAsync(HttpClient client, string baseUri, params string[] hierarchies)
        {
            var combinedUri = CombineUri(baseUri, hierarchies);
            var uri = Constants.UpdateCheckUriAppendRandom
                ? new Uri(combinedUri.ToString().TrimEnd('/') + $"?{DateTime.UtcNow.ToFileTime()}")
                : combinedUri
            ;


            Logger.Debug($"target uri: {uri}");

            var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            return response.ContinueWith(t => {
                var result = t.Result;
                Logger.Debug($"http status code: {result.StatusCode}");

                if(result.IsSuccessStatusCode) {
                    return result.Content.ReadAsStringAsync().ContinueWith(t2 => {
                        return new UriResult(t2.Result, uri);
                    });
                }

                Logger.Warning("http content is empty");
                return Task.FromResult(new UriResult(string.Empty, uri));
            }).Unwrap();
        }

        MatchValue GetTagetValue(string input, string pattern, string rawRegexOptions, string matchKey)
        {
            var options = (RegexOptions)Enum.Parse(typeof(RegexOptions), rawRegexOptions);
            var regex = new Regex(pattern, options);
            var match = regex.Match(input);

            if(!match.Success) {
                return new MatchValue(false, string.Empty);
            }

            var result = match.Groups[matchKey].Value;
            if(string.IsNullOrEmpty(result)) {
                return new MatchValue(false, string.Empty);
            }

            return new MatchValue(true, result);
        }

        string GetFormattedArchiveFileName(Version version)
        {
            var archiveFileName = Constants.UpdateCheckDownloadNameFormat
                .Replace("${MAJOR}", version.Major.ToString())
                .Replace("${MINOR}", version.Minor.ToString())
                .Replace("${BUILD}", version.Build.ToString())
#if BUILD_PLATFORM_ANYCPU
                    .Replace("${PLATFORM}", "AnyCPU")
#elif BUILD_PLATFORM_x86
                    .Replace("${PLATFORM}", "x86")
#elif BUILD_PLATFORM_x64
                    .Replace("${PLATFORM}", "x64")
#else
#error not defined: BUILD_PLATFORM_`xxxx'
#endif
            ;

            return archiveFileName;
        }

        // TODO: くっそながい
        public async Task<bool> CheckUpdateAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var productVersion = versionInfo.ProductVersion;

            Logger.Debug($"{nameof(assemblyVersion)}: {assemblyVersion}");
            Logger.Debug($"{nameof(productVersion)}: {productVersion}");

            using(var client = new HttpClient()) {
                // 対象ブランチのハッシュを取得
                var rawJson = await GetStringAsync(client, Constants.UpdateCheckBranchBaseUri, Constants.UpdateCheckBranchTargetName);
                // Json をデシリアライズする余力無し(nugetっていうかライブラリ参照すると解放できないのよね)
                // DataContractJsonSerializer? 格納クラスつくんのだりぃ
                var hashResult = GetTagetValue(rawJson.Value, Constants.UpdateCheckBranchHashPattern, Constants.UpdateCheckBranchHashPatternOptions, Constants.UpdateCheckBranchHashPatternKey);
                if(!hashResult.IsMatch) {
                    Logger.Warning("not found hash");
                    return false;
                }
                var hash = hashResult.Value;
                Logger.Debug($"hash: {hash}");

                if(string.Equals(hash, productVersion, StringComparison.InvariantCultureIgnoreCase)) {
                    // ハッシュは一緒
                    Logger.Information("latest version");
                    return false;
                }

                // リリース日を取得
                var timestampResult = GetTagetValue(rawJson.Value, Constants.UpdateCheckBranchTimestampPattern, Constants.UpdateCheckBranchTimestampPatternOptions, Constants.UpdateCheckBranchTimestampPatternKey);
                var timestamp = DateTime.MinValue;
                if(timestampResult.IsMatch) {
                    Logger.Debug($"timestamp: {timestampResult.Value}");
                    timestamp = DateTime.Parse(timestampResult.Value);
                } else {
                    Logger.Debug($"timestamp not found");
                }

                // ハッシュが違うのでバージョンが変わってるかもしんない
                var rawVersionFile = await GetStringAsync(client, Constants.UpdateCheckVersionBaseUri, hash, Constants.UpdateCheckVersionFilePath);
                var versionResult = GetTagetValue(rawVersionFile.Value, Constants.UpdateCheckVersionFilePattern, Constants.UpdateCheckVersionFilePatternOptions, Constants.UpdateCheckVersionFilePatternKey);
                if(!versionResult.IsMatch) {
                    Logger.Warning("not found version");
                    return false;
                }

                var version = versionResult.Value;
                Logger.Debug($"version: {version}");

                if(!Version.TryParse(version, out var parsedVersion)) {
                    Logger.Warning("version text can not parse");
                }

                if(parsedVersion <= assemblyVersion) {
                    // 一緒かβ版とかデバッグ時の新しいやつ
                    Logger.Information("latest version");
                    return false;
                }

                // バージョン違うけど各種サービス連携にも時間かかるしダウンロードファイルは存在しないかもしんない
                var rawDownload = await GetStringAsync(client, Constants.UpdateCheckDownloadBaseUri, string.Empty);
                // 単純検索
                var downloadFileName = GetFormattedArchiveFileName(parsedVersion);
                var donwloadPattern = Constants.UpdateCheckDownloadUriPattern.Replace("${FILENAME}", Regex.Escape(downloadFileName));
                var downloadResult = GetTagetValue(rawDownload.Value, donwloadPattern, Constants.UpdateCheckDownloadUriPatternOptions, Constants.UpdateCheckDownloadUriPatternKey);
                if(!downloadResult.IsMatch) {
                    Logger.Information("not found module");
                    return false;
                }

                var downloadUri = downloadResult.Value;
                Logger.Debug($"download uri: {downloadUri}");

                // リリースノート本文を取ってくる(ここまで来たら404だろうが何だろうが関係なく進める)
                var releaseNoteFilePath = Constants.UpdateReleaseNoteFilePathFormat
                    .Replace("${MAJOR}", parsedVersion.Major.ToString())
                    .Replace("${MINOR}", parsedVersion.Minor.ToString())
                    .Replace("${BUILD}", parsedVersion.Build.ToString())
                ;
                var rawReleaseNoteFile = await GetStringAsync(client, Constants.UpdateReleaseNoteBaseUri, hash, releaseNoteFilePath);


                HasUpdate = true;
                ReleaseNote.Hash = hash;
                ReleaseNote.Timestamp = timestamp;
                ReleaseNote.Version = parsedVersion;
                ReleaseNote.Content = rawReleaseNoteFile.Value;
                DownloadUri = new Uri(downloadUri);
                ReleaseNoteUri = rawReleaseNoteFile.Uri;
            }

            return true;
        }

        /// <summary>
        /// 古いアップデートファイルの破棄
        /// </summary>
        void GarbageCollectionArchive()
        {
            var archiveDirectoryPath = Path.Combine(CommonUtility.GetDataDirectory().FullName, Constants.ArchiveDirectoryName);
            if(!Directory.Exists(archiveDirectoryPath)) {
                Logger.Debug($"not exists archive dir: {archiveDirectoryPath}");
                return;
            }

            var archiveRegex = new Regex(@"NKit_(\d+\-\d+-\d)_\w\.zip", RegexOptions.IgnoreCase);

            var deleteFilePaths = Directory
                .EnumerateFiles(archiveDirectoryPath, "*.zip", SearchOption.TopDirectoryOnly)
                .Select(f => new { Path = f, Match = archiveRegex.Match(f) })
                .Where(i => i.Match.Success)
                .Select(i => new { Path = i.Path, Version = Version.Parse(i.Match.Value.Replace('-', '.')) })
                .OrderByDescending(i => i.Version)
                .Skip(Constants.UpdateLeaveArchiveCount)
                .Select(i => i.Path)
                .ToList()
            ;
            foreach(var filePath in deleteFilePaths) {
                try {
                    Logger.Debug($"delete target: {filePath}");
                    File.Delete(filePath);
                } catch(IOException ex) {
                    Logger.Warning(ex);
                }
            }
        }

        bool GarbageCollectionExtract(string extractDirectoryPath)
        {
            if(!Directory.Exists(extractDirectoryPath)) {
                Logger.Debug($"not exists extract dir: {extractDirectoryPath}");
                return true;
            }

            try {
                //TODO: もしかしたら消えないかもね。昔はまった記憶がある
                Directory.Delete(extractDirectoryPath, true);
                return true;
            } catch(IOException ex) {
                Logger.Warning(ex);
                return false;
            }
        }

        async Task<FileInfo> DownloadArchiveFileAsync(Uri downloadFileUri)
        {
            using(var client = new HttpClient()) {
                Logger.Debug($"download: {downloadFileUri}");
                var response = await client.GetAsync(downloadFileUri, HttpCompletionOption.ResponseHeadersRead);

                Logger.Debug($"http status code: {response.StatusCode}");
                if(!response.IsSuccessStatusCode) {
                    Logger.Error($"download error");
                    return null;
                }

                var archiveDirectoryPath = Path.Combine(CommonUtility.GetDataDirectory().FullName, Constants.ArchiveDirectoryName);
                Directory.CreateDirectory(archiveDirectoryPath);

                var archiveFilePath = Path.Combine(archiveDirectoryPath, GetFormattedArchiveFileName(ReleaseNote.Version));
                using(var fileStream = File.Create(archiveFilePath)) {
                    using(var httpContentStream = await response.Content.ReadAsStreamAsync()) {
                        await httpContentStream.CopyToAsync(fileStream);
                    }
                }

                return new FileInfo(archiveFilePath);
            }
        }

        void ExtractArchiveFile(string extractDirectoryPath, FileInfo archiveFile)
        {

            GarbageCollectionExtract(extractDirectoryPath);
            Logger.Debug($"create extract dir: {extractDirectoryPath}");
            Directory.CreateDirectory(extractDirectoryPath);

            Logger.Debug($"extract start");
            ZipFile.ExtractToDirectory(archiveFile.FullName, extractDirectoryPath);
        }

        void BackupCurrentDirectory()
        {
            var backupFilePath = Path.Combine(CommonUtility.GetDataDirectory().FullName, Constants.BackupFileName);
            Logger.Debug($"backup path: {backupFilePath}");

            if(File.Exists(backupFilePath)) {
                Logger.Debug($"remove old backup");
                File.Delete(backupFilePath);
            }

            var currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.Debug($"backup start");
            ZipFile.CreateFromDirectory(currentDirPath, backupFilePath);
        }

        long UpdateFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
        {
            //TODO: 移動可能なら移動したい
            long fileCount = 0;
            var files = sourceDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            foreach(var srcFile in files) {
                var dstFilePath = Path.Combine(destinationDirectory.FullName, srcFile.Name);
                Logger.Debug($"update file: {dstFilePath}");
                File.Copy(srcFile.FullName, dstFilePath, true);
                fileCount += 1;
            }

            var dirs = sourceDirectory.EnumerateDirectories();
            foreach(var srcDir in dirs) {
                var dstDirPath = Path.Combine(destinationDirectory.FullName, srcDir.Name);
                Logger.Debug($"update dir: {dstDirPath}");
                var dstDir = Directory.CreateDirectory(dstDirPath);
                fileCount += UpdateFiles(srcDir, dstDir);
            }

            return fileCount;
        }

        void UpdateModule(DirectoryInfo sourceDirectory)
        {
            // 展開ディレクトリの中身と現状のプログラムを置き換え
            var myFilePath = Assembly.GetExecutingAssembly().Location;
            var oldPath = myFilePath + ".old";

            var myDirPath = Path.GetDirectoryName(myFilePath);
            var myDir = new DirectoryInfo(myDirPath);

            // 老兵はただ去る
            if(File.Exists(oldPath)) {
                Logger.Debug($"delete: {oldPath}");
                File.Delete(oldPath);
            }
            File.Move(myFilePath, oldPath);

            try {
                var updatedFileCount = UpdateFiles(sourceDirectory, myDir);
                Logger.Information($"update file count: {updatedFileCount}");
            } catch(Exception ex) {
                Logger.Error(ex);
                // えらったのでファイル名戻して最悪アップデート処理だけでもできるようにしておく
                File.Move(oldPath, myFilePath);
                throw;
            }
        }

        public async Task<bool> UpdateAsync()
        {
            GarbageCollectionArchive();

            var downloadFile = await DownloadArchiveFileAsync(DownloadUri);
            if(downloadFile == null) {
                return false;
            }

            // ダウンロードしたら一旦展開用ディレクトリで展開しておく
            var extractDirectoryPath = Path.Combine(CommonUtility.GetDataDirectory().FullName, Constants.ExtractDirectoryName);
            ExtractArchiveFile(extractDirectoryPath, downloadFile);

            // 今の状態を保存しとく
            BackupCurrentDirectory();

            UpdateModule(new DirectoryInfo(extractDirectoryPath));

            Logger.Information("update ok!");

            return true;
        }
    }
}
