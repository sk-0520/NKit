using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Main.Model.Finder;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemBrowserDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemBrowserDetailViewModel(FindItemModel model)
            : base(model)
        {
            BrowserModel = Model.GetBrowser();
        }

        #region property

        BrowserModel BrowserModel { get; }

        public BrowserViewModel Browser
        {
            get { return new BrowserViewModel(BrowserModel); }
        }

        #endregion

        #region FindItemDetailViewModelBase

        public override string Header => Properties.Resources.String_ViewModel_Finder_FindItemDetail_Browse;

        public override bool Showable => Browser.BrowserKind != BrowserKind.Unknown;

        #endregion
    }
}
