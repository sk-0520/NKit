using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    /// <summary>
    /// 渡されたストリームを閉じないストリーム。
    /// <para>他のストリーム使用処理へ渡した後 閉じられると困る場合にこいつをかませて閉じないようにすることが目的。</para>
    /// <para>用途が用途なので <see cref="KeepOpenStream.Dispose"/> しても <see cref="KeepOpenStream.BaseStream"/> は何もケアされない、つまりはひらきっっぱなことに注意。</para>
    /// </summary>
    public class KeepOpenStream : Stream
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">閉じたくないストリーム。</param>
        public KeepOpenStream(Stream stream)
        {
            BaseStream = stream;
        }

        #region proeprty

        Stream BaseStream { get; set; }

        #endregion

        #region Stream

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override void Flush() => BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count)=> BaseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => BaseStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            BaseStream = null;

            base.Dispose(disposing);
        }

        #endregion
    }
}
