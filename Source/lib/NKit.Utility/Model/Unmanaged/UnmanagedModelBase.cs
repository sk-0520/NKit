using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model.Unmanaged
{
    public abstract class UnmanagedModelBase<T> : RawModel<T>
    {
        public UnmanagedModelBase(T rawObject)
            : base(rawObject)
        { }
    }

    /// <summary>
    /// アンマネージドなOS提供ハンドルを管理。
    /// </summary>
    public abstract class UnmanagedHandleModelBase : UnmanagedModelBase<IntPtr>
    {
        public UnmanagedHandleModelBase(IntPtr hHandle)
            : base(hHandle)
        {
            if(hHandle == IntPtr.Zero) {
                throw new ArgumentNullException(nameof(hHandle));
            }
        }

        /// <summary>
        /// 解放処理。
        /// <para>ハンドルにより処理色々なんでオーバーライドしてごちゃごちゃする。</para>
        /// </summary>
        protected virtual void ReleaseHandle()
        {
            throw new NotImplementedException();
        }

        #region UnmanagedBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                ReleaseHandle();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

}
