using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemTextDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemTextDetailViewModel(FindItemModel model)
            : base(model)
        { }

        #region property

        public Encoding Encoding => Model.FileContentSearchResult.Text.EncodingCheck.Encoding;

        public IReadOnlyList<TextSearchMatch> Matches
        {
            get
            {
                if(!Model.FileContentSearchResult.Text.IsMatched) {
                    return null;
                }

                return Model.FileContentSearchResult.Text.Matches;
            }
        }

        #endregion

        #region FindItemDetailViewModelBase

        public override string Header => Properties.Resources.String_ViewModel_Finder_FindItemDetail_Text;
        public override bool Showable => Model.FileContentSearchResult.Text.IsMatched;

        #endregion
    }
}
