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

        public bool IsReadOnly => Model.IsReadOnly;

        public bool IsText => Model.IsText;
        public bool IsXmlHtml => Model.IsXmlHtml;
        public bool IsJson => Model.IsJson;
        public bool IsImage => Model.IsImage;
        public bool IsProgram => Model.IsProgram;

        public bool CanBrowse(BrowserKind browserKind)
        {
            return Model.CanBrowse(browserKind);
        }

        #endregion
    }
}
