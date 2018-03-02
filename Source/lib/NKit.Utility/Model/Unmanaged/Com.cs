using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model.Unmanaged
{
    /// <summary>
    /// 生のCOMを管理。
    /// </summary>
    public class ComModel<T> : UnmanagedModelBase<T>
    {
        public ComModel(T comObject)
            : base(comObject)
        { }

        #region UnmanagedModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                Marshal.ReleaseComObject(Raw);
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
