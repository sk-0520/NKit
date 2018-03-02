using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ContentTypeTextNet.Library.PInvoke.Windows;

namespace ContentTypeTextNet.NKit.Utility.Model.Unmanaged
{
    public abstract class GdiObjectModelBase : UnmanagedHandleModelBase
    {
        public GdiObjectModelBase(IntPtr hHandle)
            : base(hHandle)
        { }

        #region property

        public virtual bool CanMakeImageSource => false;

        #endregion

        #region function

        protected abstract BitmapSource MakeBitmapSourceCore();

        /// <summary>
        /// GDIオブジェクトから<see cref="BitmapSource"/>作成。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"><see cref="CanMakeImageSource"/></exception>
        public BitmapSource MakeBitmapSource()
        {
            if(!CanMakeImageSource) {
                throw new InvalidOperationException();
            }
            return MakeBitmapSourceCore();
        }

        #endregion

        #region UnmanagedHandleModelBase

        protected override void ReleaseHandle()
        {
            NativeMethods.DeleteObject(Raw);
        }

        #endregion
    }

    public class IconHandleModel : GdiObjectModelBase
    {
        public IconHandleModel(IntPtr hIcon)
            : base(hIcon)
        { }

        #region UnmanagedHandle

        protected override void ReleaseHandle()
        {
            NativeMethods.DestroyIcon(Raw);
            /* なんだったかなぁこれ
            NativeMethods.SendMessage(Raw, WM.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            */
        }

        #endregion

        #region GdiObjectModelBase

        public override bool CanMakeImageSource => true;

        protected override BitmapSource MakeBitmapSourceCore()
        {
            var result = Imaging.CreateBitmapSourceFromHIcon(
                Raw,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            return result;
        }

        #endregion
    }

    /// <summary>
    /// ビットマップハンドルを管理。
    /// </summary>
    public class BitmapHandleModel : GdiObjectModelBase
    {
        public BitmapHandleModel(IntPtr hBitmap)
            : base(hBitmap)
        { }

        #region GdiObjectModelBase

        public override bool CanMakeImageSource => true;

        protected override BitmapSource MakeBitmapSourceCore()
        {
            var result = Imaging.CreateBitmapSourceFromHBitmap(
                Raw,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            return result;
        }

        #endregion
    }

}
