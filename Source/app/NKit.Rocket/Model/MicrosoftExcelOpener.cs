using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model.Unmanaged;
using Excel = Microsoft.Office.Interop.Excel;

namespace ContentTypeTextNet.NKit.Rocket.Model
{
    public class MicrosoftExcelOpener : ComApplicationOpenerBase
    {
        public MicrosoftExcelOpener(string filePath, string spreadSheetSheetName, int spreadSheetRowIndex, int spreadSheetColumnIndex)
            :base(filePath)
        {
            SpreadSheetSheetName = spreadSheetSheetName;
            SpreadSheetRowIndex = spreadSheetRowIndex;
            SpreadSheetColumnIndex = spreadSheetColumnIndex;
        }

        #region property

        string SpreadSheetSheetName { get; }
        int SpreadSheetRowIndex { get; }
        int SpreadSheetColumnIndex { get; }
        int SpreadSheetCellAddress { get; }

        #endregion

        #region function

        bool Show(ComModel<Excel.Application> excel, ComModel<Excel.Workbook> workbook)
        {
            // インデックス自体は 0 始まりだが Excel 側が 1 始まりのため補正が必要
            var y = SpreadSheetRowIndex + 1;
            var x = SpreadSheetColumnIndex + 1;

            for(var i = 1; i <= workbook.Com.Sheets.Count; i++) {
                using(var sheet = ComModel.Create((Excel.Worksheet)workbook.Com.Sheets.Item[i])) {
                    if(sheet.Com.Name == SpreadSheetSheetName) {
                        using(var range = ComModel.Create((Excel.Range)sheet.Com.Cells[y, x])) {
                            excel.Com.Visible = true;
                            sheet.Com.Select();
                            range.Com.Select();
                            // 終了すると開いた意味ないのね
                            CanApplicationQuit = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        #endregion

        #region OpenerBase

        public override bool Open()
        {
            ComModel<Excel.Application> excel = null;
            
            try {
                excel = new ComModel<Excel.Application>(new Excel.Application());
                ApplicationQuitAction = () => excel.Com.Quit();
            } catch(COMException ex) {
                Trace.WriteLine(ex);
                // Excel が入ってなさげなので通常のファイルオープンでさよなら。
                // シェルから開けないんならこっちの責任じゃない
                var command = $"{FilePath}#{SpreadSheetSheetName}!{SpreadSheetCellAddress}";
                Process.Start(FilePath);
                return false;
            }

            using(excel) {
                try {
                    excel.Com.Visible = false;

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
                                            if(CanApplicationQuit) {
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
                                if(CanApplicationQuit) {
                                    workbook.Com.Close(false);
                                }
                            }
                        }
                    }
                } finally {
                    QuitAppication();
                }
            }
        }

        #endregion


    }
}
