using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.NKit;

namespace ContentTypeTextNet.NKit.Main.ViewModel.App
{
    public class NKitManagerViewModel : ManagerViewModelBase<NKitManagerModel>
    {
        public NKitManagerViewModel(NKitManagerModel model)
            : base(model)
        { }

        #region property

        public string FinderTextFilePattern
        {
            get { return Model.Finder.TextFileNamePattern; }
            set { SetPropertyValue(Model.Finder, value, nameof(Model.Finder.TextFileNamePattern)); }
        }

        public string FinderMicrosoftOfficeFileNamePattern
        {
            get { return Model.Finder.MicrosoftOfficeFileNamePattern; }
            set { SetPropertyValue(Model.Finder, value, nameof(Model.Finder.MicrosoftOfficeFileNamePattern)); }
        }

        public string FinderPdfFileNamePattern
        {
            get { return Model.Finder.PdfFileNamePattern; }
            set { SetPropertyValue(Model.Finder, value, nameof(Model.Finder.PdfFileNamePattern)); }
        }

        public string FinderXmlHtmlFileNamePattern
        {
            get { return Model.Finder.XmlHtmlFileNamePattern; }
            set { SetPropertyValue(Model.Finder, value, nameof(Model.Finder.XmlHtmlFileNamePattern)); }
        }

        #endregion
    }
}
