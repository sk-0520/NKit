using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

namespace ContentTypeTextNet.NKit.Main.Model.Microsoft.Office
{
    public class MicrosoftOfficeOperator
    {
        public MicrosoftOfficeFileType GetOfficeTypeFromFilePath(string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            bool IsMatch(params string[] exts)
            {
                return exts.Any(s => string.Equals(fileExt, "." + s, StringComparison.OrdinalIgnoreCase));
            }

            if(IsMatch("xlsx", "xlsm")) {
                return MicrosoftOfficeFileType.Excel2007;
            }
            if(IsMatch("xls")) {
                return MicrosoftOfficeFileType.Excel1997;
            }

            if(IsMatch("doc")) {
                return MicrosoftOfficeFileType.Word1997;
            }
            if(IsMatch("docx", "docm")) {
                return MicrosoftOfficeFileType.Word2007;
            }

            return MicrosoftOfficeFileType.Unknown;
        }

        public IWorkbook GetWorkbook(Stream stream, MicrosoftOfficeFileType excelType)
        {
            switch(excelType) {
                case MicrosoftOfficeFileType.Excel1997:
                    return new HSSFWorkbook(stream);

                case MicrosoftOfficeFileType.Excel2007:
                    return new XSSFWorkbook(stream);

                default:
                    throw new ArgumentException($"{nameof(excelType)} is {excelType}, support: {MicrosoftOfficeFileType.Excel1997}, {MicrosoftOfficeFileType.Excel2007}");
            }
        }

        public IWorkbook GetWorkbook(FileInfo file)
        {
            var excelType = GetOfficeTypeFromFilePath(file.Name);
            if(!(excelType == MicrosoftOfficeFileType.Excel1997 || excelType == MicrosoftOfficeFileType.Excel2007)) {
                throw new ArgumentException($"{nameof(file)} is not excel book");
            }

            using(var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return GetWorkbook(stream, excelType);
            }
        }

        public IWorkbook CreateWorkBook(MicrosoftOfficeFileType excelType)
        {
            switch(excelType) {
                case MicrosoftOfficeFileType.Excel1997:
                    return new XSSFWorkbook();

                case MicrosoftOfficeFileType.Excel2007:
                    return new HSSFWorkbook();

                default:
                    throw new ArgumentException($"{nameof(excelType)} is {excelType}, support: {MicrosoftOfficeFileType.Excel1997}, {MicrosoftOfficeFileType.Excel2007}");
            }
        }

        public IBody GetDocument(Stream stream, MicrosoftOfficeFileType wordType)
        {
            switch(wordType) {
                case MicrosoftOfficeFileType.Word1997:
                    goto default;

                case MicrosoftOfficeFileType.Word2007:
                    return new XWPFDocument(stream);

                default:
                    throw new ArgumentException($"{nameof(wordType)} is {wordType}, support: /*{MicrosoftOfficeFileType.Word1997}*/ ^_^, {MicrosoftOfficeFileType.Word2007}");
            }
        }

        public IBody GetDocument(FileInfo file)
        {
            var wordType = GetOfficeTypeFromFilePath(file.Name);
            if(!(wordType == MicrosoftOfficeFileType.Word1997 || wordType == MicrosoftOfficeFileType.Word2007)) {
                throw new ArgumentException($"{nameof(file)} is not word document");
            }

            using(var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return GetDocument(stream, wordType);
            }
        }

        public IBody CreateDocument(MicrosoftOfficeFileType wordType)
        {
            switch(wordType) {
                case MicrosoftOfficeFileType.Word1997:
                    goto default;

                case MicrosoftOfficeFileType.Word2007:
                    return new XWPFDocument();

                default:
                    throw new ArgumentException($"{nameof(wordType)} is {wordType}, support: /*{MicrosoftOfficeFileType.Word1997}*/ ^v^, {MicrosoftOfficeFileType.Word2007}");
            }
        }
    }
}
