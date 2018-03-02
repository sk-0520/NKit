using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    /// <summary>
    /// App.config データのキャッシュ。
    /// </summary>
    public class ConfigurationCacher : Cacher<string, object>
    {
        /// <summary>
        /// 指定キーからデータ変換して値を取得。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public TResult Get<TResult>(string key, Func<string, TResult> parser)
        {
            return (TResult)Get(key, () => parser(ConfigurationManager.AppSettings[key]));
        }
        /// <summary>
        /// 指定キーから文字列を取得。
        /// <para><see cref="Get{TResult}(string, Func{string, TResult})"/>と文字列取得処理を合わせるためだけI/Fとして定義。</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return (string)Get(key, () => ConfigurationManager.AppSettings[key]);
        }
    }
}
