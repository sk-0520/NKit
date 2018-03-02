using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Define
{
    /// <summary>
    /// 検索パターン種別。
    /// <para>ファイル名のフィルタ処理。</para>
    /// </summary>
    public enum FindPatternKind
    {
        /// <summary>
        /// 部分一致。
        /// </summary>
        PartialMatch,
        /// <summary>
        /// ワイルドカード。
        /// </summary>
        WildcardCharacter,
        /// <summary>
        /// 正規表現。
        /// </summary>
        Regex,
    }

}
