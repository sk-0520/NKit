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
        public CaptureImageModel(CaptureGroupModel group, FileInfo imageFile)
        {
            Group = group;
            ImageFile = imageFile;
        }

        #region property

        CaptureGroupModel Group { get; }

        public FileInfo ImageFile { get; }

        #endregion

        #region function
        #endregion
    }
}
