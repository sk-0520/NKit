using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureImageModel : ModelBase
    {
        public CaptureImageModel(CaptureGroupModel group, FileInfo rawImageFile)
        {
            Group = group;
            RawImageFile = rawImageFile;
        }

        #region property

        CaptureGroupModel Group { get; }

        public FileInfo RawImageFile { get; }

        #endregion

        #region function

        public string GetEnabledThumbnailImagePath()
        {
            var baseName = Path.GetFileNameWithoutExtension(RawImageFile.Name);
            var thumbnailFileName = baseName.Replace(Constants.CaptureRawImageSuffix, Constants.CaptureThumbnailImageSuffix) + ".jpeg";
            var thumbnailFilePath = Path.Combine(RawImageFile.DirectoryName, thumbnailFileName);

            if(global::System.IO.File.Exists(thumbnailFilePath)) {
                return thumbnailFilePath;
            }
            Logger.Warning($"not found thumbnail: {thumbnailFilePath}");

            return RawImageFile.FullName;
        }

        #endregion
    }
}
