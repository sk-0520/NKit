using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class TextOperator
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
        public string ToUnique(string target, IEnumerable<string> list, StringComparison comparisonType, Func<string, int, string> dg)
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
        public string ToUniqueDefault(string target, IEnumerable<string> list, StringComparison comparisonType)
        {
            return ToUnique(target, list, comparisonType, (string source, int index) => string.Format("{0}({1})", source, index));
        }

        /// <summary>
        /// 指定文字列集合を<see cref="StringCollection"/>に変換する。
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public StringCollection ToStringCollection(IEnumerable<string> seq)
        {
            var sc = new StringCollection();
            sc.AddRange(seq.ToArray());

            return sc;
        }

        #endregion
    }
}
