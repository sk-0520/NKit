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

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var productVersion = versionInfo.ProductVersion;

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
                var match = hashRegex.Match(rawJson);
                if(!match.Success) {
                    Logger.Warning("not found hash");
                    return false;
                }
                var hash = match.Groups["HASH"].Value;
                Logger.Information($"hash: {hash}");

                // ハッシュ比較

                // ハッシュが違うのでバージョンが変わってるかもしんない
            }

            return false;
        }
    }
}
