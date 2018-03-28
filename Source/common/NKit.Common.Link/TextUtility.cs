using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Common
{
    public static class TextUtility
    {
        #region function

        /// <summary>
        /// 指定データを集合の中から単一である値に変換する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="list"></param>
        /// <param name="comparisonType">比較</param>
        /// <param name="dg">nullの場合はデフォルト動作</param>
        /// <returns></returns>
        public static string ToUnique(string target, IEnumerable<string> list, StringComparison comparisonType, Func<string, int, string> dg)
        {
            Debug.Assert(dg != null);

            var changeName = target;

            int n = 1;
            RETRY:
            foreach(var value in list) {
                if(string.Equals(value, changeName, comparisonType)) {
                    changeName = dg(target, ++n);
                    goto RETRY;
                }
            }

            return changeName;
        }

        /// <summary>
        /// 指定データを集合の中から単一である値に変換する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="list"></param>
        /// <param name="comparisonType"></param>
        /// <returns>集合の中に同じものがなければtarget, 存在すればtarget(n)。</returns>
        public static string ToUniqueDefault(string target, IEnumerable<string> list, StringComparison comparisonType)
        {
            return ToUnique(target, list, comparisonType, (string source, int index) => string.Format("{0}({1})", source, index));
        }

        /// <summary>
        /// 指定文字列集合を<see cref="StringCollection"/>に変換する。
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static StringCollection ToStringCollection(IEnumerable<string> seq)
        {
            var sc = new StringCollection();
            sc.AddRange(seq.ToArray());

            return sc;
        }

        /// <summary>
        /// 指定範囲の値を指定処理で置き換える。
        /// </summary>
        /// <param name="src">対象。</param>
        /// <param name="head">置き換え開始文字列。</param>
        /// <param name="tail">置き換え終了文字列。</param>
        /// <param name="dg">処理。</param>
        /// <returns></returns>
        public static string ReplaceRange(string src, string head, string tail, Func<string, string> dg)
        {
            var escHead = Regex.Escape(head);
            var escTail = Regex.Escape(tail);
            var pattern = escHead + "(.+?)" + escTail;
            var replacedText = Regex.Replace(src, pattern, (Match m) => dg(m.Groups[1].Value));
            return replacedText;
        }

        /// <summary>
        /// 指定範囲の値を指定のコレクションで置き換える。
        /// </summary>
        /// <param name="src">対象。</param>
        /// <param name="head">置き換え開始文字列。</param>
        /// <param name="tail">置き換え終了文字列。</param>
        /// <param name="map">置き換え対象文字列と置き換え後文字列のペアであるコレクション。</param>
        /// <returns></returns>
        public static string ReplaceRangeFromDictionary(string src, string head, string tail, IDictionary<string, string> map)
        {
            return ReplaceRange(src, head, tail, s => map.ContainsKey(s) ? map[s] : head + s + tail);
        }
        /// <summary>
        /// ${key}をvalueに置き変える。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string ReplaceFromDictionary(string src, IDictionary<string, string> map)
        {
            return ReplaceRangeFromDictionary(src, "${", "}", map);
        }


        public static IEnumerable<string> ReadLines(string text)
        {
            using(var reader = new StringReader(text)) {
                return ReadLines(reader);
            }
        }

        public static IEnumerable<string> ReadLines(TextReader reader)
        {
            if(reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            string line;
            while((line = reader.ReadLine()) != null) {
                yield return line;
            }
        }

        #endregion
    }
}
