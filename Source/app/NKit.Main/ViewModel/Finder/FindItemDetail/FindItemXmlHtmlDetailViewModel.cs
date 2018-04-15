using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Searcher;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemXmlHtmlDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemXmlHtmlDetailViewModel(FindItemModel model)
            : base(model)
        { }

        #region property

        public IReadOnlyList<TextSearchMatch> Matches
        {
            get
            {
                if(!Model.FileContentSearchResult.XmlHtml.IsMatched) {
                    return null;
                }

                var list = new List<TextSearchMatch>(Model.FileContentSearchResult.XmlHtml.Results.Count);
                foreach(var result in Model.FileContentSearchResult.XmlHtml.Results) {
                    if(result.NodeType == HtmlAgilityPack.HtmlNodeType.Comment) {
                        var comment = (XmlHtmlCommentSearchResult)result;
                        list.AddRange(comment.Matches);
                    } else if(result.NodeType == HtmlAgilityPack.HtmlNodeType.Text) {
                        var text = (XmlHtmlTextSearchResult)result;
                        list.AddRange(text.Matches);
                    } else {
                        var element = (XmlHtmlElementSearchResult)result;
                        list.AddRange(element.ElementResult.Matches);
                        foreach(var attribute in element.AttributeKeyResults) {
                            list.AddRange(attribute.KeyResult.Matches);
                            list.AddRange(attribute.ValueResult.Matches);
                        }
                    }
                }

                return list;
            }
            set { /* TwoWay */ }
        }

        #endregion

        #region FindItemDetailViewModelBase

        public override string Header => Properties.Resources.String_ViewModel_Finder_FindItemDetail_XmlHtml;
        public override bool Showable => Model.FileContentSearchResult.XmlHtml.IsMatched;
        public override bool IsEnabled => true;

        #endregion
    }
}
