using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemPdfDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemPdfDetailViewModel(FindItemModel model)
            : base(model)
        { }

        #region property

        public IReadOnlyList<TextSearchMatch> Matches
        {
            get
            {
                if(!Model.FileContentSearchResult.Pdf.IsMatched) {
                    return null;
                }

                return Model.FileContentSearchResult.Pdf.Matches;
            }
        }

        #endregion

        #region FindItemDetailViewModelBase

        public override string Header => Properties.Resources.String_ViewModel_Finder_FindItemDetail_Pdf;
        public override bool Showable => Model.FileContentSearchResult.Pdf.IsMatched;

        #endregion
    }
}
