using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf;

namespace ContentTypeTextNet.NKit.Main.Model.Searcher
{
    public class PdfSearchResult : SearchResultBase
    {
        #region define

        public static PdfSearchResult NotFound { get; } = new PdfSearchResult();

        #endregion
    }

    public class PdfSearcher
    {
        #region property
        #endregion

        #region function

        public PdfSearchResult Search(Stream stream, Regex regex)
        {
            var reader = new PdfReader(stream);

            return PdfSearchResult.NotFound;
        }

        #endregion
    }
}
