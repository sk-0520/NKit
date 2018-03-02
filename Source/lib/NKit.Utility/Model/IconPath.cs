using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public struct IconPath
    {
        #region property
        /// <summary>
        /// パス。
        /// </summary>
        [DataMember]
        public string Path { get; set; }
        /// <summary>
        /// アイコンインデックス。
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        public string DisplayPath
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Path)) {
                    if(Index > 0) {
                        return $":{nameof(Index)} = {Index}";
                    } else {
                        return string.Empty;
                    }
                } else {
                    return $"{Path},{Index}";
                }
            }

        }
        #endregion
    }
}
