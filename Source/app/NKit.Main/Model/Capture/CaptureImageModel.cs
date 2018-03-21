using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ContentTypeTextNet.NKit.Setting.Capture;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.WindowsAPICodePack.Shell;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureImageModel : ModelBase
    {
        public CaptureImageModel(CaptureImageSetting imageSetting, CaptureGroupModel group, FileInfo rawImageFile)
        {
            ImageSetting = imageSetting;
            Group = group;
            ImageFile = rawImageFile;
        }

        #region property

        CaptureImageSetting ImageSetting { get; }
        CaptureGroupModel Group { get; }

        public FileInfo ImageFile { get; }

        public uint Width => ImageSetting.Width;
        public uint Height => ImageSetting.Height;

        public string Comment
        {
            get { return ImageSetting.Comment; }
            set { ImageSetting.Comment = value; }
        }

        #endregion

        #region function

        FileInfo GetThumbnailImage()
        {
            var baseName = Path.GetFileNameWithoutExtension(ImageFile.Name);
            var thumbnailFileName = baseName.Replace(Constants.CaptureRawImageSuffix, Constants.CaptureThumbnailImageSuffix) + ".jpeg";
            var thumbnailFilePath = Path.Combine(ImageFile.DirectoryName, thumbnailFileName);

            var file = new FileInfo(thumbnailFilePath);
            file.Refresh();

            return file;
        }

        public string GetEnabledThumbnailImagePath()
        {
            var file = GetThumbnailImage();
            if(file.Exists) {
                return file.FullName;
            }
            Logger.Warning($"not found thumbnail: {file.FullName}");

            return ImageFile.FullName;
        }

        public void RefreshSetting()
        {
            using(var shellFile = ShellFile.FromFilePath(ImageFile.FullName)) {
                var image = shellFile.Properties.System.Image;
                ImageSetting.Width = image.HorizontalSize.Value ?? 0;
                ImageSetting.Height = image.VerticalSize.Value ?? 0;
            }
        }

        public bool CopyImage()
        {
            // UI スレッドからしか来ないっしょ
            using(var stream = ImageFile.OpenRead()) {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
                var bitmap = new WriteableBitmap(decoder.Frames[0]);

                var co = new ClipboardOperator();
                return co.CopyImage(bitmap);
            }
        }

        #endregion

        #region ModelBase
        #endregion
    }
}
