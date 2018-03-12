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

        #endregion

        string CombineUriCore(string left, string right)
        {
            return left.TrimEnd('/') + "/" + right.TrimStart('/');
        }

        Uri CombineUri(string baseUri, params string[] hierarchies)
        {
            var list = new List<string>() {
                baseUri,
            };
            list.AddRange(hierarchies);
            //hierarchy
            var originUri = list.Aggregate((left, right) => CombineUriCore(left, right));

            return new Uri(originUri);
        }

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
                var branchUri = CombineUri(Constants.UpdateCheckBranchBaseUri, Constants.UpdateCheckBranchTargetName);
                Logger.Debug($"hash uri: {branchUri}");
                var rawJson = await client.GetStringAsync(branchUri);
                Logger.Debug(rawJson);
                // Json をデシリアライズする余力無し(nugetっていうかライブラリ参照すると解放できないのよね)
                // DataContractJsonSerializer? 格納クラスつくんのだりぃ
                var hashRegex = new Regex(Constants.UpdateCheckBranchHashPattern, (RegexOptions)Enum.Parse(typeof(RegexOptions), Constants.UpdateCheckBranchHashPatternOptions));
                var hashMatch = hashRegex.Match(rawJson);
                if(!hashMatch.Success) {
                    Logger.Warning("not found hash");
                    return false;
                }
                var hash = hashMatch.Groups[Constants.UpdateCheckBranchHashPatternKey].Value;
                Logger.Debug($"hash: {hash}");

                if(string.Equals(hash, productVersion, StringComparison.InvariantCultureIgnoreCase)) {
                    // ハッシュは一緒
                    Logger.Information("latest version");
                    return false;
                }

                // ハッシュが違うのでバージョンが変わってるかもしんない
                var versionFileUri = CombineUri(Constants.UpdateCheckVersionBaseUri, hash, Constants.UpdateCheckVersionFilePath);
                Logger.Debug($"file uri: {versionFileUri}");
                var rawVersionFile = await client.GetStringAsync(versionFileUri);
                var versionRegex = new Regex(Constants.UpdateCheckVersionFilePattern, (RegexOptions)Enum.Parse(typeof(RegexOptions), Constants.UpdateCheckVersionFilePatternOptions));
                var versionMatch = versionRegex.Match(rawVersionFile);
                if(!versionMatch.Success) {
                    Logger.Warning("not found version");
                    return false;
                }

                var version = versionMatch.Groups[Constants.UpdateCheckVersionFilePatternKey].Value;
                Logger.Debug($"version: {version}");

                if(!Version.TryParse(version, out var versionResult)) {
                    Logger.Warning("version text can not parse");
                }

                if(versionResult <= assemblyVersion) {
                    // 一緒かβ版とかデバッグ時の新しいやつ
                    Logger.Information("latest version");
                    return false;
                }

                // バージョン違うけど各種サービス連携にも時間かかるしダウンロードファイルは存在しないかもしんない


            }

            return false;
        }
    }
}
