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
using ContentTypeTextNet.NKit.Main.Model.Searcher;
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

        public TextSearchResult Text { get; set; } = TextSearchResult.NotFound;
        public MicrosoftOfficeSearchResultBase MicrosoftOffice { get; set; } = MicrosoftOfficeSearchResultBase.NotFound;
        public PdfSearchResult Pdf { get; set; } = PdfSearchResult.NotFound;
        public XmlHtmlSearchResult XmlHtml { get; set; } = XmlHtmlSearchResult.NotFound;

        #endregion
    }


    public class FileContentSearcher : ModelBase
    {
        public FileContentSearcher(FileInfo file)
        {
            File = file;

            FileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            Debug.Assert(FileStream.CanSeek);
            FileStreamPostionKeeper = new StreamPostionKeeper(FileStream);
        }

        #region property

        FileInfo File { get; }
        ILogFactory LogFactory { get; }

        Stream FileStream { get; }
        StreamPostionKeeper FileStreamPostionKeeper { get; }

        TextSearchResult TextSearchResult { get;set;}
        MicrosoftOfficeSearchResultBase MicrosoftOfficeSearchResult { get;set;}
        PdfSearchResult PdfSearchResult { get; set; }
        XmlHtmlSearchResult XmlHtmlSearchResult { get;set;}

        #endregion

        #region function

        public TextSearchResult SearchText(Regex regex)
        {
            if(File.Length == 0) {
                return TextSearchResult.NotFound;
            }

            FileStreamPostionKeeper.Reset();

            var ts = new TextSearcher();
            return TextSearchResult = ts.Search(FileStream, regex);
        }

        public MicrosoftOfficeExcelSearchResult SearchMicrosoftExcel(MicrosoftOfficeFileType excelType, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeExcelSearchResult.NotFound;
            }

            FileStreamPostionKeeper.Reset();

            var es = new MicrosoftOfficeExcelSearcher();
            var result = es.Search(excelType, new KeepOpenStream(FileStream), regex, setting);
            MicrosoftOfficeSearchResult = result;
            return result;
        }

        public MicrosoftOfficeWordSearchResult SearchMicrosoftWord(MicrosoftOfficeFileType wordType, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeWordSearchResult.NotFound;
            }

            FileStreamPostionKeeper.Reset();

            var ws = new MicrosoftOfficeWordSearcher();
            var result = ws.Search(wordType, new KeepOpenStream(FileStream), regex, setting);
            MicrosoftOfficeSearchResult = result;
            return result;
        }

        /// <summary>
        /// MS Office ???????????????
        /// <para>??????????????????????????????????????????</para>
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

        public PdfSearchResult SearchPdf(Regex regex)
        {
            if(File.Length == 0) {
                return PdfSearchResult.NotFound;
            }

            FileStreamPostionKeeper.Reset();

            var ps = new PdfSearcher();
            return PdfSearchResult = ps.Search(new KeepOpenStream(FileStream), regex);
        }

        public XmlHtmlSearchResult SearchXmlHtml(Regex regex, IReadOnlyFindXmlHtmlContentSetting setting)
        {
            if(File.Length == 0) {
                return XmlHtmlSearchResult.NotFound;
            }
            FileStreamPostionKeeper.Reset();

            XmlHtmlSearchResult result;
            var xs = new XmlHtmlSearcher();
            if(TextSearchResult != null && TextSearchResult.EncodingCheck != null && TextSearchResult.EncodingCheck.IsSuccess) {
                result = xs.Search(new KeepOpenStream(FileStream), regex, TextSearchResult.EncodingCheck.Encoding, setting);
            } else {
                result = xs.Search(new KeepOpenStream(FileStream), regex, setting);
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
                    FileStreamPostionKeeper.Dispose();
                    FileStream.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
