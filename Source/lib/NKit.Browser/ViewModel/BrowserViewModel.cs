using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.ViewModel
{
    public class BrowserViewModel : SingleModelViewModelBase<BrowserModel>
    {
        #region property

        bool _isBuilded = false;
        bool _isBuilding = false;

        #endregion

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

        public bool IsBuilded
        {
            get { return this._isBuilded; }
            set { SetProperty(ref this._isBuilded, value); }
        }

        public bool IsBuilding
        {
            get { return this._isBuilding; }
            set { SetProperty(ref this._isBuilding, value); }
        }

        #endregion

        #region function

        public bool CanBrowse(BrowserKind browserKind)
        {
            return Model.CanBrowse(browserKind);
        }

        #endregion
    }
}
