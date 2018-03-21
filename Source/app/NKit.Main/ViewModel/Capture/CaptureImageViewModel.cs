using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Prism.Commands;

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

        public string FileName => Model.ImageFile.Name;
        public string FilePath => Model.ImageFile.FullName;

        public uint Width => Model.Width;
        public uint Height => Model.Height;
        public string Comment
        {
            get { return Model.Comment; }
            set { SetModelValue(value); }
        }

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

        public DateTime CaptureStartUtcTimestamp => Model.ImageFile.Directory.CreationTime.ToUniversalTime();
        public DateTime CaptureUtcTimestamp => Model.ImageFile.CreationTime.ToUniversalTime();

        #endregion

        #region command

        public ICommand CopyCommand => new DelegateCommand(() => {
            Model.CopyImage();
        });

        #endregion

        #region function
        #endregion
    }
}
