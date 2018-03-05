using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class XmlHtmlSearchResult : SearchResultBase
    {
        #region define

        public static XmlHtmlSearchResult NotFound { get; } = new XmlHtmlSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property



        #endregion
    }

    /// <summary>
    /// XMLとHTMLは全然一緒じゃないよねって気持ち。
    /// </summary>
    public class XmlHtmlSearcher
    {
        #region function

        public XmlHtmlSearchResult Search(Stream stream, Regex regex, Encoding encoding, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            var document = new HtmlDocument() {
                OptionAutoCloseOnEnd = true,
            };
            document.Load(stream);


            return null;
        }

        public XmlHtmlSearchResult Search(Stream stream, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            var ec = new EncodingChecker();



            return null;
        }

        #endregion
    }
}
