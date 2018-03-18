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
        public CaptureImageModel(FileInfo imageFile)
        {
            ImageFile = imageFile;
        }

        #region property

        FileInfo ImageFile { get; }

        #endregion

        #region function
        #endregion
    }
}
