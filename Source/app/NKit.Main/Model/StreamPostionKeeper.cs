using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Main.Model
{
    /// <summary>
    /// 渡された時点のストリーム位置を戻す人。
    /// <para>ストリームを閉じたりは担当じゃない点に注意。</para>
    /// </summary>
    public sealed class StreamPostionKeeper : DisposerBase
    {
        public StreamPostionKeeper(Stream stream)
        {
            if(!stream.CanSeek) {
                throw new ArgumentException($"{nameof(stream)} can not seek");
            }

            Stream = stream;
            KeepPostion = Stream.Position;

        }

        #region property

        Stream Stream { get; set; }

        public long KeepPostion { get; }

        #endregion

        #region function

        public void Reset()
        {
            Stream.Position = KeepPostion;
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    Reset();
                }
                Stream = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
