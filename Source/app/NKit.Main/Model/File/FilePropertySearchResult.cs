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
        /// ファイルサイズチェック結果。
        /// </summary>
        public bool IsEnabledFileSize { get; set; }

        #endregion
    }
}
