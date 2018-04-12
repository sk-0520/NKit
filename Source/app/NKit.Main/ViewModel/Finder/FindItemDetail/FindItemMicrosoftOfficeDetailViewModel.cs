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

        public FindItemMicrosoftOfficeExcelBookDetailViewModel Excel => new FindItemMicrosoftOfficeExcelBookDetailViewModel(Model);
        public FindItemMicrosoftOfficeWordDetailViewModel Word => new FindItemMicrosoftOfficeWordDetailViewModel(Model);

        #endregion

        #region FindItemDetailViewModelBase

        public override bool Showable => Model.FileContentSearchResult.MicrosoftOffice.IsMatched;

        #endregion
    }

    public class FindItemMicrosoftOfficeExcelBookDetailViewModel : FindItemDetailViewModelBase
    {
        #region variable

        IReadOnlyList<FindItemMicrosoftOfficeExcelSheetDetailViewModel> _sheets;

        #endregion

        public FindItemMicrosoftOfficeExcelBookDetailViewModel(FindItemModel model)
            : base(model)
        { }

        public IReadOnlyList<FindItemMicrosoftOfficeExcelSheetDetailViewModel> Sheets
        {
            get
            {
                if(this._sheets == null) {
                    if(!Model.FileContentSearchResult.MicrosoftOffice.IsMatched) {
                        this._sheets = Enumerable.Empty<FindItemMicrosoftOfficeExcelSheetDetailViewModel>().ToList();
                        return null;
                    }

                    var excelResult = Model.FileContentSearchResult.MicrosoftOffice as MicrosoftOfficeExcelSearchResult;
                    if(excelResult == null) {
                        this._sheets = Enumerable.Empty<FindItemMicrosoftOfficeExcelSheetDetailViewModel>().ToList();
                        return null;
                    }

                    this._sheets = excelResult.MatchSheet
                        .Select(s => new FindItemMicrosoftOfficeExcelSheetDetailViewModel(Model, s))
                        .ToList()
                    ;
                }

                return this._sheets;
            }
        }

        #region FindItemDetailViewModelBase

        public override bool Showable => throw new NotSupportedException();

        #endregion
    }

    public class FindItemMicrosoftOfficeExcelSheetDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemMicrosoftOfficeExcelSheetDetailViewModel(FindItemModel model, MicrosoftOfficeExcelSheetSearchResult sheetResult)
            : base(model)
        {
            SheetResult = sheetResult;
        }

        #region property

        MicrosoftOfficeExcelSheetSearchResult SheetResult { get; }

        public string SheetName => SheetResult.SheetName;
        public IReadOnlyList<TextSearchMatch> SheetNameMatches => SheetResult.SheetNameResult.Matches;

        public IReadOnlyList<MicrosoftOfficeExcelCellSearchResult> CellResults  => SheetResult.CellResults;
        public IReadOnlyList<MicrosoftOfficeExcelShapeSearchResult> ShapeResults => SheetResult.ShapeResults;

        #endregion

        #region FindItemDetailViewModelBase

        public override bool Showable => throw new NotSupportedException();

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

        public override bool Showable => throw new NotSupportedException();

        #endregion
    }
}
