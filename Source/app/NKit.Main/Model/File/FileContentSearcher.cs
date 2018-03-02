using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Microsoft.Office;
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

        Stream FileStream { get; }

        #endregion

        #region function

        public TextSearchResult SearchText(Regex regex)
        {
            if(File.Length == 0) {
                return TextSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var ts = new TextSearcher();
            return ts.Search(FileStream, regex);
        }

        public MicrosoftOfficeExcelSearchResult SearchMicrosoftExcel(MicrosoftOfficeFileType excelType, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeExcelSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var es = new MicrosoftOfficeExcelSearcher();
            return es.Search(excelType, FileStream, regex, setting);
        }

        public MicrosoftOfficeWordSearchResult SearchMicrosoftWord(MicrosoftOfficeFileType wordType, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            if(File.Length == 0) {
                return MicrosoftOfficeWordSearchResult.NotFound;
            }

            FileStream.Position = 0;

            var ws = new MicrosoftOfficeWordSearcher();
            return ws.Search(wordType, FileStream, regex, setting);
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
                    return SearchMicrosoftExcel(officeType, regex, parameter.Excel);

                case MicrosoftOfficeFileType.Word2007:
                    return SearchMicrosoftWord(officeType, regex, parameter.Word);

                default:
                    return MicrosoftOfficeSearchResultBase.NotFound;
            }

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
