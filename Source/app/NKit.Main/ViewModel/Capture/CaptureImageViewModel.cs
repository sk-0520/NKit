using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Capture
{
    public class CaptureImageViewModel : SingleModelViewModelBase<CaptureImageModel>
    {
        public CaptureImageViewModel(CaptureImageModel model)
            : base(model)
        { }

        #region property

        public string FileName => Model.ImageFile.Name;
        public string FilePath => Model.ImageFile.FullName;

        public DateTime CaptureStartTimestamp => Model.ImageFile.Directory.CreationTime;
        public DateTime CaptureTimestamp => Model.ImageFile.CreationTime;

        #endregion

        #region command
        #endregion

        #region function
        #endregion
    }
}
