using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Main
{
    internal static partial class Constants
    {
        #region property

        #region capture

        public static string CaptureRawImageSuffix { get; } = "raw";
        public static string CaptureThumbnailImageSuffix { get; } = "thumb";

        public static ImageKind CaptureImageKind { get; } = ImageKind.Png;
        public static ImageKind CaptureThumbnailKind { get; } = ImageKind.Jpeg;
        public static Size CaptureThumbnailSize { get; } = new Size(120, 80);

        #endregion

        #endregion
    }
}
