using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemMicrosoftOfficeDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemMicrosoftOfficeDetailViewModel(FindItemModel model)
            : base(model)
        { }

        #region property

        public MicrosoftOfficeFileType OfficeType => Model.FileContentSearchResult.MicrosoftOffice.OfficeType;

        public FindItemMicrosoftOfficeWordDetailViewModel Word => new FindItemMicrosoftOfficeWordDetailViewModel(Model);

        #endregion

        #region FindItemDetailViewModelBase

        public override bool Showable => Model.FileContentSearchResult.MicrosoftOffice.IsMatched;

        #endregion
    }

    public class FindItemMicrosoftOfficeWordDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemMicrosoftOfficeWordDetailViewModel(FindItemModel model)
            : base(model)
        { }

        #region property

        public IReadOnlyList<TextSearchMatch> Elements
        {
            get
            {
                if(!Model.FileContentSearchResult.MicrosoftOffice.IsMatched) {
                    return null;
                }

                var wordResult = Model.FileContentSearchResult.MicrosoftOffice as MicrosoftOfficeWordSearchResult;
                if(wordResult == null) {
                    return null;
                }

                var list = new List<TextSearchMatch>();

                foreach(var result in wordResult.ElementResults) {
                    switch(result) {
                        case MicrosoftOfficeWordParagraphSearchResult paragraph:
                            list.AddRange(paragraph.TextResult.Matches);
                            break;

                        case MicrosoftOfficeWordTableSearchResult table:
                            list.AddRange(table.CellResults.SelectMany(c => c.TextResult.Matches));
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                return list;
            }
            set { /* TwoWay ダミー */}
        }

        #endregion

        #region FindItemDetailViewModelBase

        public override bool Showable => throw new NotImplementedException();

        #endregion
    }
}
