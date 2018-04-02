using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            return PdfSearchResult.NotFound;
        }

        #endregion
    }
}
