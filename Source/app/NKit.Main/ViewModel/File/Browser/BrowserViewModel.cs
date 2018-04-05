using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel.File.Browser
{
    public class BrowserViewModel : SingleModelViewModelBase<BrowserModel>
    {
        public BrowserViewModel(BrowserModel model)
            : base(model)
        { }

        #region property

        public BrowserKind BrowserKind => Model.BrowserKind;

        public FileInfo FileInfo => Model.FileInfo;

        public Encoding Encoding => Model.Encoding;

        public bool IsEditable => Model.IsEditable;

        public bool IsText => BrowserKind.IsText();

        #endregion
    }
}
