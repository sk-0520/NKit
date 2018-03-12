using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public UpdateManager(ILogFactory logFactory)
        {
            LogFactory = logFactory;
            Logger = LogFactory.CreateLogger("UP");
        }

        #region property

        ILogFactory LogFactory { get; }
        ILogger Logger { get; }

        public Version NewVersion { get; private set; }
        public Uri DownloadUri { get; private set; }

        public string ReleaseNote { get; private set; }

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

        Task<string> GetStringAsync(HttpClient client, string baseUri, params string[] hierarchies)
        {
            var uri = CombineUri(baseUri, hierarchies);

            Logger.Debug($"target uri: {uri}");

            var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            return response.ContinueWith(t => {
                var result = t.Result;
                Logger.Debug($"http status code: {result.StatusCode}");

                if(result.IsSuccessStatusCode) {
                    return result.Content.ReadAsStringAsync();
                }

                Logger.Warning("http content is empty");
                return Task.FromResult(string.Empty);
            }).Unwrap();
        }

        (bool isMatch, string value) GetTagetValue(string input, string pattern, string rawRegexOptions, string matchKey)
        {
            var options = (RegexOptions)Enum.Parse(typeof(RegexOptions), rawRegexOptions);
            var regex = new Regex(pattern, options);
            var match = regex.Match(input);

            if(!match.Success) {
                return (false, string.Empty);
            }

            var result = match.Groups[matchKey].Value;
            if(string.IsNullOrEmpty(result)) {
                return (false, string.Empty);
            }

            return (true, result);
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
                var hashResult = GetTagetValue(rawJson, Constants.UpdateCheckBranchHashPattern, Constants.UpdateCheckBranchHashPatternOptions, Constants.UpdateCheckBranchHashPatternKey);
                if(!hashResult.isMatch) {
                    Logger.Warning("not found hash");
                    return false;
                }
                var hash = hashResult.value;
                Logger.Debug($"hash: {hash}");

                if(string.Equals(hash, productVersion, StringComparison.InvariantCultureIgnoreCase)) {
                    // ハッシュは一緒
                    Logger.Information("latest version");
                    return false;
                }

                // ハッシュが違うのでバージョンが変わってるかもしんない
                var rawVersionFile = await GetStringAsync(client, Constants.UpdateCheckVersionBaseUri, hash, Constants.UpdateCheckVersionFilePath);
                var versionResult = GetTagetValue(rawVersionFile, Constants.UpdateCheckVersionFilePattern, Constants.UpdateCheckVersionFilePatternOptions, Constants.UpdateCheckVersionFilePatternKey);
                if(!versionResult.isMatch) {
                    Logger.Warning("not found version");
                    return false;
                }

                var version = versionResult.value;
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
                var downloadFileName = Constants.UpdateCheckDownloadNameFormat
                    .Replace("${MAJOR}", parsedVersion.Major.ToString())
                    .Replace("${MINOR}", parsedVersion.Minor.ToString())
                    .Replace("${BUILD}", parsedVersion.Build.ToString())
#if BUILD_PLATFORM_ANYCPU
                    .Replace("${PLATFORM}", "AnyCPU")
#elif BUILD_PLATFORM_x86
                    .Replace("${PLATFORM}", "x86")
#elif BUILD_PLATFORM_x64
                    .Replace("${PLATFORM}", "x64")
#else
#   error not defined: BUILD_PLATFORM_`xxxx'
#endif
                ;
                var donwloadPattern = Constants.UpdateCheckDownloadUriPattern.Replace("${FILENAME}", Regex.Escape(downloadFileName));
                var downloadResult = GetTagetValue(rawDownload, donwloadPattern, Constants.UpdateCheckDownloadUriPatternOptions, Constants.UpdateCheckDownloadUriPatternKey);
                if(!downloadResult.isMatch) {
                    Logger.Information("not found module");
                    return false;
                }

                var downloadUri = downloadResult.value;
                Logger.Debug($"download uri: {downloadUri}");

                // リリースノート本文を取ってくる(ここまで来たら404だろうが何だろうが関係なく進める)
                var releaseNoteFilePath = Constants.UpdateReleaseNoteFilePathFormat
                    .Replace("${MAJOR}", parsedVersion.Major.ToString())
                    .Replace("${MINOR}", parsedVersion.Minor.ToString())
                    .Replace("${BUILD}", parsedVersion.Build.ToString())
                ;
                var rawReleaseNoteFile = await GetStringAsync(client, Constants.UpdateReleaseNoteBaseUri, hash, releaseNoteFilePath);


                NewVersion = parsedVersion;
                DownloadUri = new Uri(downloadUri);
                ReleaseNote = rawReleaseNoteFile;
            }

            return true;
        }
    }
}
