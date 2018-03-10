using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FilePropertySearchResult: SearchResultBase
    {
        #region define

        public static FilePropertySearchResult NotFound { get; } = new FilePropertySearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        /// <summary>
        /// ファイル属性一致。
        /// <para>一致内容自体は指定された<see cref="ContentTypeTextNet.NKit.Setting.Define.FlagMatchKind"/>に依存。</para>
        /// </summary>
        public bool IsEnabledAttributes { get; set; }

        #endregion
    }
}
