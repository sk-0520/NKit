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
        #region variable

        string _thumbnailImagePath = null;

        #endregion

        public CaptureImageViewModel(CaptureImageModel model)
            : base(model)
        { }

        #region property

        public string FileName => Model.RawImageFile.Name;
        public string FilePath => Model.RawImageFile.FullName;

        public string ThumbnailImagePath
        {
            get
            {
                if(this._thumbnailImagePath == null) {
                    this._thumbnailImagePath = Model.GetEnabledThumbnailImagePath();
                }

                return this._thumbnailImagePath;
            }
        }

        public DateTime CaptureStartTimestamp => Model.RawImageFile.Directory.CreationTime;
        public DateTime CaptureTimestamp => Model.RawImageFile.CreationTime;

        #endregion

        #region command
        #endregion

        #region function
        #endregion
    }
}
