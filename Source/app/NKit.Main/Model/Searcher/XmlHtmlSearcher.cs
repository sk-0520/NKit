using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using Hnx8.ReadJEnc;
using HtmlAgilityPack;

namespace ContentTypeTextNet.NKit.Main.Model.Searcher
{
    public abstract class XmlHtmlSearchResultBase : SearchResultBase
    { }

    public class XmlHtmlAttributeSearchResult : XmlHtmlSearchResultBase
    {
        #region define

        public static XmlHtmlAttributeSearchResult NotFound { get; } = new XmlHtmlAttributeSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public string Key { get; set; }

        public TextSearchResult KeyResult { get; set; }

        public TextSearchResult ValueResult { get; set; }

        #endregion
    }

    public abstract class XmlHtmlNodeSearchResultBase : XmlHtmlSearchResultBase
    {
        #region define

        class XmlHtmlNodeNotFoundSearchResult : XmlHtmlNodeSearchResultBase
        { }

        public static XmlHtmlNodeSearchResultBase NotFound { get; } = new XmlHtmlNodeNotFoundSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public HtmlNodeType NodeType { get; set; }

        #endregion
    }

    public class XmlHtmlElementSearchResult : XmlHtmlNodeSearchResultBase
    {
        #region define

        public static new XmlHtmlElementSearchResult NotFound { get; } = new XmlHtmlElementSearchResult() {
            IsMatched = false,
        };

        #endregion

        public XmlHtmlElementSearchResult()
        {
            NodeType = HtmlNodeType.Element;
        }

        #region property

        public TextSearchResult ElementResult { get; set; }

        public List<XmlHtmlAttributeSearchResult> AttributeKeyResults { get; set; } = new List<XmlHtmlAttributeSearchResult>();

        #endregion
    }

    public class XmlHtmlCommentSearchResult : XmlHtmlNodeSearchResultBase
    {
        #region define

        public static new XmlHtmlCommentSearchResult NotFound { get; } = new XmlHtmlCommentSearchResult() {
            IsMatched = false,
        };

        #endregion

        public XmlHtmlCommentSearchResult()
        {
            NodeType = HtmlNodeType.Comment;
        }

        #region property

        public IReadOnlyList<TextSearchMatch> Matches { get; set; }

        #endregion
    }

    public class XmlHtmlTextSearchResult : XmlHtmlNodeSearchResultBase
    {
        #region define

        public static new XmlHtmlTextSearchResult NotFound { get; } = new XmlHtmlTextSearchResult() {
            IsMatched = false,
        };

        #endregion

        public XmlHtmlTextSearchResult()
        {
            NodeType = HtmlNodeType.Text;
        }

        #region property

        public IReadOnlyList<TextSearchMatch> Matches { get; set; }

        #endregion
    }

    public class XmlHtmlSearchResult : XmlHtmlSearchResultBase
    {
        #region define

        public static XmlHtmlSearchResult NotFound { get; } = new XmlHtmlSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public List<XmlHtmlNodeSearchResultBase> Results { get; set; } = new List<XmlHtmlNodeSearchResultBase>();

        #endregion
    }

    /// <summary>
    /// XMLとHTMLは全然一緒じゃないよねって気持ち。
    /// </summary>
    public class XmlHtmlSearcher
    {
        #region property

        public ReadJEnc ReadJEnc { get; set; } = ReadJEnc.JP;

        /// <summary>
        /// エンコーディングをチェックする際に用いるデータ長。
        /// <para>この長さに満たない場合は入力データを全て用いる。</para>
        /// <para><see cref="TextSearcher"/>と違って、エンコーディング情報の取得に失敗した場合やエンコーディング指定ミスは例外で死ぬ。</para>
        /// <para>64KBありゃLHOにも引っかかんねぇし多分大丈夫っしょ。</para>
        /// </summary>
        public int EncodingCheckMaximumLength { get; set; } = 64 * 1024;

        #endregion

        #region function

        XmlHtmlCommentSearchResult SearchComment(HtmlNode node, Regex regex)
        {
            var ts = new CustomTextSearchMatchTextSeacher() {
                CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                    return new TextSearchMatch(lineNumber, characterPostion, length, lineText) {
                        Header = $"{node.ParentNode.OriginalName}",
                        Footer = "COMMENT",
                        DisplayLineNumber = node.Line,
                        DisplayCharacterPosition = node.LinePosition,
                    };
                },
            };

            var textResult = ts.Search(node.InnerText, regex);

            if(!textResult.IsMatched) {
                return XmlHtmlCommentSearchResult.NotFound;
            }

            return new XmlHtmlCommentSearchResult() {
                IsMatched = true,
                Matches = textResult.Matches,
            };
        }

        XmlHtmlTextSearchResult SearchText(HtmlNode node, Regex regex)
        {
            var ts = new CustomTextSearchMatchTextSeacher() {
                CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                    return new TextSearchMatch(lineNumber, characterPostion, length, lineText) {
                        Header = $"{node.ParentNode.OriginalName}",
                        Footer = "TEXT",
                        DisplayLineNumber = node.Line,
                        DisplayCharacterPosition = node.LinePosition,
                    };
                },
            };
            var textResult = ts.Search(node.InnerText, regex);

            if(!textResult.IsMatched) {
                return XmlHtmlTextSearchResult.NotFound;
            }

            return new XmlHtmlTextSearchResult() {
                IsMatched = true,
                Matches = textResult.Matches,
            };
        }


        XmlHtmlAttributeSearchResult SearchAttribute(HtmlAttribute attribute, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            var result = new XmlHtmlAttributeSearchResult() {
                Key = attribute.OriginalName,
            };

            if(setting.AttributeKey) {
                var ts = new CustomTextSearchMatchTextSeacher() {
                    CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                        return new TextSearchMatch(lineNumber, characterPostion, length, lineText) {
                            Header = $"{attribute.OwnerNode.OriginalName}",
                            Footer = "ATTR-KEY",
                            DisplayLineNumber = attribute.Line,
                            DisplayCharacterPosition = attribute.LinePosition,
                        };
                    },
                };
                result.KeyResult = ts.Search(attribute.OriginalName, regex);
            }

            if(setting.AttributeValue) {
                var ts = new CustomTextSearchMatchTextSeacher() {
                    CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                        return new TextSearchMatch(lineNumber, characterPostion, length, lineText) {
                            Header = $"{attribute.OwnerNode.OriginalName}",
                            Footer = "ATTR-KEY",
                            DisplayLineNumber = attribute.Line,
                            DisplayCharacterPosition = attribute.LinePosition,
                        };
                    },
                };
                result.ValueResult = ts.Search(attribute.Value, regex);
            }

            result.IsMatched = result.KeyResult.IsMatched || result.ValueResult.IsMatched;

            return result;
        }

        bool IsIgnoreAttribute(HtmlNode node, HtmlAttribute attribute)
        {
            // TODO: 未実装
            return false;
        }

        XmlHtmlElementSearchResult SearchElement(HtmlNode node, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            Debug.Assert(node.NodeType == HtmlNodeType.Element);

            var result = new XmlHtmlElementSearchResult();

            if(setting.Element) {
                var ts = new CustomTextSearchMatchTextSeacher() {
                    CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                        return new TextSearchMatch(lineNumber, characterPostion, length, lineText) {
                            Header = $"{node.ParentNode.OriginalName}",
                            Footer = "ELEMENT",
                            DisplayLineNumber = node.Line,
                            DisplayCharacterPosition = node.LinePosition,
                        };
                    },
                };
                result.ElementResult = ts.Search(node.OriginalName, regex);
            } else {
                result.ElementResult = TextSearchResult.NotFound;
            }

            if(node.NodeType == HtmlNodeType.Element && (setting.AttributeKey || setting.AttributeValue)) {
                result.AttributeKeyResults = node.Attributes
                    .Where(a => setting.IgnoreHtmlLinkValue ? !IsIgnoreAttribute(node, a) : true)
                    .Select(a => SearchAttribute(a, regex, setting))
                    .ToList()
                ;
            }

            result.IsMatched = result.ElementResult.IsMatched || result.AttributeKeyResults.Any(a => a.IsMatched);

            return result;
        }

        XmlHtmlNodeSearchResultBase SearchNode(HtmlNode node, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            if(node.NodeType == HtmlNodeType.Comment) {
                if(setting.Comment) {
                    return SearchComment(node, regex);
                }
            } else if(node.NodeType == HtmlNodeType.Text) {
                if(setting.Text) {
                    return SearchText(node, regex);
                }
            } else {
                return SearchElement(node, regex, setting);
            }

            return XmlHtmlNodeSearchResultBase.NotFound;
        }

        IEnumerable<XmlHtmlNodeSearchResultBase> SearchNodes(HtmlNode node, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            //Debug.Assert(node.NodeType != HtmlNodeType.Document);

            if(node.NodeType == HtmlNodeType.Element || node.NodeType == HtmlNodeType.Text || node.NodeType == HtmlNodeType.Comment) {
                var nodeResult = SearchNode(node, regex, setting);
                if(nodeResult.IsMatched) {
                    yield return nodeResult;
                }
            }

            if(node.NodeType == HtmlNodeType.Element || node.NodeType == HtmlNodeType.Document) {
                var childResults = node.ChildNodes
                    .Select(n => SearchNodes(n, regex, setting))
                    .SelectMany(rs => rs)
                    .Where(r => r.IsMatched)
                ;

                foreach(var childResult in childResults) {
                    yield return childResult;
                }
            }
        }

        public XmlHtmlSearchResult Search(Stream stream, Regex regex, Encoding encoding, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            var document = new HtmlDocument() {
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = encoding,
            };
            document.Load(stream);

            var result = new XmlHtmlSearchResult();

            var nodeResults = SearchNodes(document.DocumentNode, regex, setting);
            result.Results.AddRange(nodeResults);

            result.IsMatched = result.Results.Any(r => r.IsMatched);

            return result;
        }

        public XmlHtmlSearchResult Search(Stream stream, Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            var checkLength = (int)Math.Min(EncodingCheckMaximumLength, stream.Length);
            var ec = new EncodingChecker();

            EncodingCheckResult encodingCheckResult;
            using(new StreamPostionKeeper(stream)) {
                var binary = new byte[checkLength];
                stream.Read(binary, 0, binary.Length);
                encodingCheckResult = ec.GetEncoding(ReadJEnc, binary);
            }

            if(!encodingCheckResult.IsSuccess) {
                return XmlHtmlSearchResult.NotFound;
            }

            return Search(stream, regex, encodingCheckResult.Encoding, setting);
        }

        #endregion
    }
}
