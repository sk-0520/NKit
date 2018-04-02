using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Utility.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ContentTypeTextNet.NKit.Main.Model.Searcher
{
    public class PdfSearchResult : SearchResultBase
    {
        #region define

        public static PdfSearchResult NotFound { get; } = new PdfSearchResult();

        #endregion

        #region property

        public IReadOnlyList<TextSearchMatch> Matches { get; set; }

        #endregion
    }

    public class PdfSearcher
    {
        #region property
        #endregion

        #region function

        public PdfSearchResult Search(Stream stream, Regex regex)
        {
            var matches = new List<TextSearchMatch>();

            using(var reader = new PdfReader(new KeepOpenStream(stream))) {
                var numberOfPages = reader.NumberOfPages;
                for(var pageNumber = 1; pageNumber <= numberOfPages; pageNumber++) {
                    //var parser = new PdfReaderContentParser(reader);
                    var content = PdfTextExtractor.GetTextFromPage(reader, pageNumber);
                    var ts = new CustomTextSearchMatchTextSeacher() {
                        CustomTextMatchCreator = (int lineNumber, int characterPosition, int length, string lineText) => {
                            return new TextSearchMatch(lineNumber, characterPosition, length, lineText) {
                                Header = $"Page {pageNumber}",
                                Tag = new AssociationDocumentParameter() {
                                    Page = pageNumber,
                                },
                            };
                        }
                    };
                    var result = ts.Search(content, regex);
                    if(result.IsMatched) {
                        matches.AddRange(result.Matches);
                    }
                }

                if(matches.Any()) {
                    return new PdfSearchResult() {
                        IsMatched = true,
                        Matches = matches,
                    };
                }

                return PdfSearchResult.NotFound;
            }
        }

        #endregion
    }
}
