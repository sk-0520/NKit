using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Microsoft.Office;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileContentSearchResult : SearchResultBase
    {
        #region define

        public static FileContentSearchResult NotFound { get; } = new FileContentSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public TextSearchResult Text { get; set; }
        public MicrosoftOfficeSearchResultBase MicrosoftOffice { get; set; }
        public XmlHtmlSearchResult XmlHtml { get; set; }

        #endregion
    }


    public class FileContentSearcher : ModelBase
    {
        public FileContentSearcher(FileInfo file)
        {
            File = file;

            FileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            Debug.Assert(FileStream.CanSeek);
        }

        #region property

        FileInfo File { get; }
        ILogFactory LogFactory { get; }

        Stream FileStream { get; }

        TextSearchResult TextSearchResult { get;set;}
        MicrosoftOfficeSearchResultBase MicrosoftOfficeSearchResult { get;set;}
        XmlHtmlSearchResult XmlHtmlSearchResult { get;set;}

        #endregion

        #region function

        public TextSearchResult SearchText(Regex regex)
        {
            if(File.Length == 0) {
                return TextSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var ts = new TextSearcher();
            return TextSearchResult = ts.Search(FileStream, regex);
        }

        public MicrosoftOfficeExcelSearchResult SearchMicrosoftExcel(MicrosoftOfficeFileType excelType, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeExcelSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var es = new MicrosoftOfficeExcelSearcher();
            var result = es.Search(excelType, FileStream, regex, setting);
            MicrosoftOfficeSearchResult = result;
            return result;
        }

        public MicrosoftOfficeWordSearchResult SearchMicrosoftWord(MicrosoftOfficeFileType wordType, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeWordSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var ws = new MicrosoftOfficeWordSearcher();
            var result = ws.Search(wordType, FileStream, regex, setting);
            MicrosoftOfficeSearchResult = result;
            return result;
        }

        /// <summary>
        /// MS Office 一括検索。
        /// <para>将来用に大きめのパラメータ。</para>
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public MicrosoftOfficeSearchResultBase SearchMicrosoftOffice(Regex regex, MicrosoftOfficeSearchParameter parameter)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeSearchResultBase.NotFound;
            }

            var oo = new MicrosoftOfficeOperator();
            var officeType = oo.GetOfficeTypeFromFilePath(File.Name);

            switch(officeType) {
                case MicrosoftOfficeFileType.Excel1997:
                case MicrosoftOfficeFileType.Excel2007:
                    return MicrosoftOfficeSearchResult = SearchMicrosoftExcel(officeType, regex, parameter.Excel);

                case MicrosoftOfficeFileType.Word2007:
                    return MicrosoftOfficeSearchResult = SearchMicrosoftWord(officeType, regex, parameter.Word);

                default:
                    return MicrosoftOfficeSearchResultBase.NotFound;
            }

        }

        public XmlHtmlSearchResult SearchXmlHtml(Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            if(File.Length == 0) {
                return XmlHtmlSearchResult.NotFound;
            }
            FileStream.Position = 0;

            XmlHtmlSearchResult result;
            var xs = new XmlHtmlSearcher();
            if(TextSearchResult != null && TextSearchResult.EncodingCheck != null && TextSearchResult.EncodingCheck.IsSuccess) {
                result = xs.Search(FileStream, regex, TextSearchResult.EncodingCheck.Encoding, setting);
            } else {
                result = xs.Search(FileStream, regex, setting);
            }
            XmlHtmlSearchResult = result;
            return result;
        }

        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    FileStream.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
