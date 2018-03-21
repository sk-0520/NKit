using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.WindowsAPICodePack.Shell;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureImageModel : ModelBase
    {
        public CaptureImageModel(CaptureGroupModel group, FileInfo rawImageFile)
        {
            Group = group;
            ImageFile = rawImageFile;

            ImageShellFile = ShellFile.FromFilePath(ImageFile.FullName);
        }

        #region property

        CaptureGroupModel Group { get; }

        public FileInfo ImageFile { get; }
        ShellFile ImageShellFile { get; }

        public uint Width => ImageShellFile.Properties.System.Image.HorizontalSize?.Value ?? 0;
        public uint Height => ImageShellFile.Properties.System.Image.VerticalSize?.Value ?? 0;

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

        //public string GetComment()
        //{
        //}

        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    ImageShellFile.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
