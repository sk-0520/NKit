using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using Excel = Microsoft.Office.Interop.Excel;

namespace ContentTypeTextNet.NKit.Process.Model
{
    public class ExcelOpener : DisposerBase
    {
        public ExcelOpener(string filePath, string spreadSheetSheetName, int spreadSheetRowIndex, int spreadSheetColumnIndex)
        {
            FilePath = filePath;
            SpreadSheetSheetName = spreadSheetSheetName;
            SpreadSheetRowIndex = spreadSheetRowIndex;
            SpreadSheetColumnIndex = spreadSheetColumnIndex;
        }

        #region property

        string FilePath { get; }
        string SpreadSheetSheetName { get; }
        int SpreadSheetRowIndex { get; }
        int SpreadSheetColumnIndex { get; }

        bool ExcelQuit { get; set; } = true;

        #endregion

        #region function

        bool Show(ComModel<Excel.Application> excel, ComModel<Excel.Workbook> workbook)
        {
            for(var i = 1; i <= workbook.Com.Sheets.Count; i++) {
                using(var sheet = ComModel.Create((Excel.Worksheet)workbook.Com.Sheets.Item[i])) {
                    if(sheet.Com.Name == SpreadSheetSheetName) {
                        using(var range = ComModel.Create((Excel.Range)sheet.Com.Cells[SpreadSheetRowIndex, SpreadSheetColumnIndex])) {
                            excel.Com.Visible = true;
                            sheet.Com.Select();
                            range.Com.Select();
                            // 終了すると開いた意味ないのね
                            ExcelQuit = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Open()
        {
            using(var excel = ComModel.Create(new Excel.Application())) {
                try {
                    using(var workbooks = ComModel.Create(excel.Com.Workbooks)) {
                        // 起動インスタンスでしかできないんすね。もっかしなにか解決策あるかもしれんし一応コードは生きたまま残しておく
                        var __TODO__ = false;
                        if(__TODO__) {
                            // 既に開かれているワークブックがあればそいつを操作する
                            for(var i = 1; i <= workbooks.Com.Count; i++) {
                                using(var workbook = ComModel.Create(workbooks.Com.Item[i])) {
                                    if(string.Equals(FilePath, workbook.Com.Path, StringComparison.InvariantCultureIgnoreCase)) {
                                        try {
                                            return Show(excel, workbook);
                                        } finally {
                                            if(ExcelQuit) {
                                                workbook.Com.Close(false);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        using(var workbook = ComModel.Create(workbooks.Com.Open(FilePath))) {
                            try {
                                return Show(excel, workbook);
                            } finally {
                                if(ExcelQuit) {
                                    workbook.Com.Close(false);
                                }
                            }
                        }
                    }
                } finally {
                    if(ExcelQuit) {
                        excel.Com.Quit();
                    }
                }
            }
        }

        #endregion


    }
}
