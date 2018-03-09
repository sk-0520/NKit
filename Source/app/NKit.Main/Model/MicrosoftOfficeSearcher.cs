using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.Microsoft.Office;
using ContentTypeTextNet.NKit.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.WP.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public abstract class MicrosoftOfficeSearchParameterBase : SearchParameterBase
    { }

    public sealed class MicrosoftOfficeSearchParameter : MicrosoftOfficeSearchParameterBase
    {
        #region property

        public IReadOnlyFindMicrosoftOfficeExcelContentSetting Excel { get; set; }
        public IReadOnlyFindMicrosoftOfficeWordContentSetting Word { get; set; }

        #endregion
    }

    public interface IReadOnlyMicrosoftOfficeExcelCellAddress
    {
        #region property

        int RowIndex { get; }
        int ColumnIndex { get; }

        #endregion
    }

    public abstract class MicrosoftOfficeSearchResultBase : SearchResultBase
    {
        #region define

        class MicrosoftOfficeNotFoundSearchResult : MicrosoftOfficeSearchResultBase
        { }

        public static MicrosoftOfficeSearchResultBase NotFound { get; } = new MicrosoftOfficeNotFoundSearchResult();

        #endregion
        #region property

        public MicrosoftOfficeFileType OfficeType { get; set; }

        #endregion
    }

    public class MicrosoftOfficeExcelCellSearchResult : SearchResultBase, IReadOnlyMicrosoftOfficeExcelCellAddress
    {
        #region define

        public static MicrosoftOfficeExcelCellSearchResult NotFound { get; } = new MicrosoftOfficeExcelCellSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region IReadOnlyMicrosoftOfficeExcelCellAddress
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        #endregion

        #region property

        public bool IsFormula { get; set; }
        public IReadOnlyList<TextSearchMatch> Matches { get; set; }

        public IReadOnlyList<TextSearchMatch> CommentMatch { get; set; }


        #endregion
    }

    public class MicrosoftOfficeExcelShapeSearchResult : SearchResultBase, IReadOnlyMicrosoftOfficeExcelCellAddress
    {
        #region define

        public static MicrosoftOfficeExcelShapeSearchResult NotFound { get; } = new MicrosoftOfficeExcelShapeSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region IReadOnlyMicrosoftOfficeExcelCellAddress
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        #endregion

        #region property

        public IReadOnlyList<TextSearchMatch> Matches { get; set; }

        #endregion
    }

    public class MicrosoftOfficeExcelSheetSearchResult : SearchResultBase
    {
        #region property

        public string SheetName { get; set; }

        public int SheetIndex { get; set; }

        public TextSearchResult SheetNameResult { get; set; }

        public List<MicrosoftOfficeExcelCellSearchResult> CellResults { get; set; } = new List<MicrosoftOfficeExcelCellSearchResult>();

        public List<MicrosoftOfficeExcelShapeSearchResult> ShapeResults { get; set; } = new List<MicrosoftOfficeExcelShapeSearchResult>();

        #endregion
    }

    public class MicrosoftOfficeExcelSearchResult : MicrosoftOfficeSearchResultBase
    {
        #region define

        public static new MicrosoftOfficeExcelSearchResult NotFound { get; } = new MicrosoftOfficeExcelSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public List<MicrosoftOfficeExcelSheetSearchResult> MatchSheet { get; set; } = new List<MicrosoftOfficeExcelSheetSearchResult>();

        #endregion
    }

    public class MicrosoftOfficeExcelSearcher
    {
        #region define

        public static int UnknownRowIndex { get; } = -1;
        public static int UnknownColumnIndex { get; } = -1;

        #endregion

        #region function

        MicrosoftOfficeExcelCellSearchResult SearchCell(ExcelCell cell, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            var ts = new CustomTextSearchMatchTextSeacher();

            var isFormula = true;
            TextSearchResult cellSearchResult = null;
            // 数式そのものを優先して検査対象とする
            if(setting.PriorityFormula && cell.Raw.CellType == CellType.Formula) {
                cellSearchResult = ts.Search(cell.Raw.CellFormula, regex);
            }

            // 数式で検索できなかったりそもそも検索対象指定じゃないので普通に検索する
            if(cellSearchResult == null || !cellSearchResult.IsMatched) {
                cellSearchResult = ts.Search(cell.GetDisplayText(), regex);
                isFormula = false;
            }

            // コメントまで検索
            TextSearchResult commentSearchResult = null;
            if(setting.CommentInCell && cell.Raw.CellComment != null) {
                // TODO: コメント検索なんとかせにゃね
                var comment = cell.Raw.CellComment.String.String;
                Debug.WriteLine("not impl: cell comment");
            }
            if(commentSearchResult == null) {
                commentSearchResult = TextSearchResult.NotFound;
            }

            if(!cellSearchResult.IsMatched && !commentSearchResult.IsMatched) {
                return MicrosoftOfficeExcelCellSearchResult.NotFound;
            }

            foreach(var match in cellSearchResult.Matches.Concat(commentSearchResult.Matches)) {
                var cellReference = new CellReference(cell.Raw);
                match.Footer = " CELL";
                match.Header = $"[{cellReference.FormatAsString()}] ";
                match.Tag = new AssociationSpreadSeetParameter() {
                    SheetName = cell.Raw.Sheet.SheetName,
                    RowIndex = cell.Raw.RowIndex,
                    ColumnIndex = cell.Raw.ColumnIndex,
                };
            }

            var result = new MicrosoftOfficeExcelCellSearchResult() {
                IsMatched = cellSearchResult.IsMatched || commentSearchResult.IsMatched,
                RowIndex = cell.Raw.RowIndex,
                ColumnIndex = cell.Raw.ColumnIndex,
                IsFormula = isFormula,
                Matches = cellSearchResult.Matches,
                CommentMatch = commentSearchResult.Matches
            };

            return result;
        }

        MicrosoftOfficeExcelSheetSearchResult SearchSheet(ExcelSheet sheet, int sheetIndex, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            var result = new MicrosoftOfficeExcelSheetSearchResult() {
                SheetName = sheet.Raw.SheetName,
                SheetIndex = sheetIndex,
            };

            if(setting.SheetName) {
                var ts = new TextSearcher();
                result.SheetNameResult = ts.Search(result.SheetName, regex);
            } else {
                result.SheetNameResult = TextSearchResult.NotFound;
            }

            for(var rowIndex = sheet.Raw.FirstRowNum; rowIndex <= sheet.Raw.LastRowNum; rowIndex++) {
                var row = sheet.Raw.GetRow(rowIndex);
                if(row == null) {
                    continue;
                }
                var cells = row.Cells
                    .Where(c => c != null)
                    // コメント検索を行う場合は全数セル必要だけどコメント不要なら空のセルは外せる、と思ってたけどコメントの取り方が分からん！
                    .Where(c => setting.CommentInCell || c.CellType != CellType.Blank)
                    .Select(c => new ExcelCell(c))
                ;
                foreach(var cell in cells) {
                    var cellSearchResult = SearchCell(cell, regex, setting);
                    if(cellSearchResult.IsMatched) {
                        result.CellResults.Add(cellSearchResult);
                    }
                }
            }

            if(setting.TextInShape) {
                var shapeResults = SearchShape(sheet, regex, setting);
                foreach(var shapeResult in shapeResults.Where(s => s.IsMatched)) {
                    result.ShapeResults.Add(shapeResult);
                }
            }

            result.IsMatched = new[]
            {
                result.SheetNameResult.IsMatched,
                result.CellResults.Any(s => s.IsMatched),
                result.ShapeResults.Any(s => s.IsMatched),
            }.Any(b => b);

            return result;
        }

        bool IsEnabledTextShape1997(HSSFShape shape)
        {
            if(shape is HSSFComment) {
                // コメントて。。。どないなってんのさ
                return false;
            }

            if(shape is HSSFPicture) {
                return false;
            }

            return shape is HSSFSimpleShape;
        }

        IEnumerable<MicrosoftOfficeExcelShapeSearchResult> SearchShape1997(ExcelSheet sheet, HSSFPatriarch patriarch, Regex regex, IReadOnlyFindMicrosoftOfficeCommonContentSetting setting)
        {
            var shapes = patriarch
                .GetShapes()
                .Where(s => IsEnabledTextShape1997(s))
            ;
            var ts = new TextSearcher();

            foreach(var shape in shapes) {
                if(shape is HSSFSimpleShape textShape) {
                    string shapeText;
                    try {
                        // NPOIからぬるり来るけど事前に確かめる方法がない(と思う)
                        shapeText = textShape.String.String;
                    } catch(NullReferenceException ex) {
                        Log.Out.Debug(ex);
                        continue;
                    }

                    var searchResult = ts.Search(shapeText, regex);
                    if(searchResult.IsMatched) {
                        var result = new MicrosoftOfficeExcelShapeSearchResult() {
                            IsMatched = searchResult.IsMatched,
                            Matches = searchResult.Matches,
                            RowIndex = UnknownRowIndex,
                            ColumnIndex = UnknownColumnIndex,
                        };

                        if(shape.Anchor is HSSFClientAnchor clientAncor) {
                            result.RowIndex = clientAncor.Row1;
                            result.ColumnIndex = clientAncor.Col1;

                            foreach(var match in result.Matches) {
                                var cellReference = new CellReference(result.RowIndex, result.ColumnIndex);
                                match.Footer = " SHAPE";
                                match.Header = $"[{cellReference.FormatAsString()}] ";
                                match.Tag = new AssociationSpreadSeetParameter() {
                                    SheetName = sheet.Raw.SheetName,
                                    RowIndex = result.RowIndex,
                                    ColumnIndex = result.ColumnIndex,
                                };
                            }
                        }

                        yield return result;
                    }
                }
            }
        }

        bool IsEnabledTextShape2007(XSSFShape shape)
        {
            if(shape is XSSFPicture) {
                return false;
            }

            return shape is XSSFSimpleShape;
        }

        IEnumerable<MicrosoftOfficeExcelShapeSearchResult> SearchShape2007(ExcelSheet sheet, XSSFDrawing drawing, Regex regex, IReadOnlyFindMicrosoftOfficeCommonContentSetting setting)
        {
            var shapes = drawing
                .GetShapes()
                .Where(s => IsEnabledTextShape2007(s))
            ;
            var ts = new TextSearcher();

            foreach(var shape in shapes) {
                if(shape is XSSFSimpleShape textShape) {
                    var searchResult = ts.Search(textShape.Text, regex);
                    if(searchResult.IsMatched) {
                        var result = new MicrosoftOfficeExcelShapeSearchResult() {
                            IsMatched = searchResult.IsMatched,
                            Matches = searchResult.Matches,
                            RowIndex = UnknownRowIndex,
                            ColumnIndex = UnknownColumnIndex,
                        };

                        if(shape.GetAnchor() is XSSFClientAnchor clientAncor) {
                            result.RowIndex = clientAncor.Row1;
                            result.ColumnIndex = clientAncor.Col1;

                            foreach(var match in result.Matches) {
                                var cellReference = new CellReference(result.RowIndex, result.ColumnIndex);
                                match.Footer = " SHAPE";
                                match.Header = $"[{cellReference.FormatAsString()}] ";
                            }
                        }

                        yield return result;
                    }
                }
            }
        }

        IEnumerable<MicrosoftOfficeExcelShapeSearchResult> SearchShape(ExcelSheet sheet, Regex regex, IReadOnlyFindMicrosoftOfficeCommonContentSetting setting)
        {
            var patriarchBase = sheet.Raw.CreateDrawingPatriarch();

            if(patriarchBase is HSSFPatriarch patriarch) {
                return SearchShape1997(sheet, patriarch, regex, setting);
            } else if(patriarchBase is XSSFDrawing drawing) {
                return SearchShape2007(sheet, drawing, regex, setting);
            }

            // 来んでしょ
            return Enumerable.Empty<MicrosoftOfficeExcelShapeSearchResult>();
        }

        public MicrosoftOfficeExcelSearchResult Search(MicrosoftOfficeFileType excelType, Stream stream, Regex regex, IReadOnlyFindMicrosoftOfficeExcelContentSetting setting)
        {
            var op = new MicrosoftOfficeOperator();
            var book = op.GetWorkbook(stream, excelType);

            var result = new MicrosoftOfficeExcelSearchResult() {
                OfficeType = excelType,
            };

            for(var sheetIndex = 0; sheetIndex < book.NumberOfSheets; sheetIndex++) {
                var sheet = new ExcelSheet(book.GetSheetAt(sheetIndex));
                var sheetResult = SearchSheet(sheet, sheetIndex, regex, setting);
                if(sheetResult.IsMatched) {
                    result.MatchSheet.Add(sheetResult);
                }
            }

            result.IsMatched = result.MatchSheet.Any(s => s.IsMatched);

            return result;
        }


        #endregion
    }

    public abstract class MicrosoftOfficeWordElementSearchResult : SearchResultBase
    {
        #region define

        class _NotFound : MicrosoftOfficeWordElementSearchResult
        {
            public _NotFound()
            {
                IsMatched = false;
            }
        }

        public static MicrosoftOfficeWordElementSearchResult NotFound { get; } = new _NotFound();

        #endregion

        #region property

        public BodyElementType ElementType { get; protected set; }

        #endregion
    }

    public class MicrosoftOfficeWordParagraphSearchResult : MicrosoftOfficeWordElementSearchResult
    {
        #region define

        public static new MicrosoftOfficeWordParagraphSearchResult NotFound { get; } = new MicrosoftOfficeWordParagraphSearchResult() {
            IsMatched = false,
        };

        #endregion

        public MicrosoftOfficeWordParagraphSearchResult()
        {
            ElementType = BodyElementType.PARAGRAPH;
        }

        #region property

        public TextSearchResult TextResult { get; set; }

        #endregion
    }

    public class MicrosoftOfficeWordTableCellSearchResult : MicrosoftOfficeWordParagraphSearchResult
    {
        #region property

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        #endregion
    }

    public class MicrosoftOfficeWordTableSearchResult : MicrosoftOfficeWordElementSearchResult
    {
        #region define

        public static new MicrosoftOfficeWordTableSearchResult NotFound { get; } = new MicrosoftOfficeWordTableSearchResult() {
            IsMatched = false,
        };

        #endregion

        public MicrosoftOfficeWordTableSearchResult()
        {
            ElementType = BodyElementType.TABLE;
        }

        #region property

        public List<MicrosoftOfficeWordTableCellSearchResult> CellResults { get; set; } = new List<MicrosoftOfficeWordTableCellSearchResult>();

        #endregion

    }

    public class MicrosoftOfficeWordSearchResult : MicrosoftOfficeSearchResultBase
    {
        #region define

        public static new MicrosoftOfficeWordSearchResult NotFound { get; } = new MicrosoftOfficeWordSearchResult() {
            IsMatched = false,
        };

        #endregion

        #region property

        public List<MicrosoftOfficeWordElementSearchResult> ElementResults { get; set; } = new List<MicrosoftOfficeWordElementSearchResult>();

        #endregion
    }

    public class MicrosoftOfficeWordSearcher
    {
        #region define
        #endregion

        #region function

        MicrosoftOfficeWordParagraphSearchResult SearchParagraph2007(int elementIndex, XWPFParagraph paragraph, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            var result = new MicrosoftOfficeWordParagraphSearchResult();

            var ts = new CustomTextSearchMatchTextSeacher() {
                CustomTextMatchCreator = (int lineNumber, int characterPostion, int length, string lineText) => {
                    // 行と段落のあれやこれやでぐっちゃぐちゃなので行番号は段落番号に変更
                    return new TextSearchMatch(elementIndex, characterPostion, length, lineText) {
                        DisplayLineNumber = elementIndex + 1,
                        Tag = new AssociationDocumentParameter() {
                            Page = -1,//TODO: ページ番号の取り方が分かるんなら最初から対応してる
                        }
                    };
                },
            };

            var text = paragraph.Text;
            var textResult = ts.Search(text, regex);

            result.TextResult = textResult;

            result.IsMatched = result.TextResult.IsMatched;


            return result;
        }


        MicrosoftOfficeWordTableSearchResult SearchTable2007(int elementIndex, XWPFTable table, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            var tableResult = new MicrosoftOfficeWordTableSearchResult();

            foreach(var (row, rowIndex) in table.Rows.Select((r, i) => (r, i))) {
                var cells = row.GetTableCells();
                foreach(var (cell, cellIndex) in cells.Select((c, i) => (c, i))) {
                    foreach(var p in cell.Paragraphs) {
                        var paragraphResult = SearchParagraph2007(elementIndex, p, regex, setting);
                        if(paragraphResult.IsMatched) {
                            var cellResult = new MicrosoftOfficeWordTableCellSearchResult() {
                                IsMatched = paragraphResult.IsMatched,
                                TextResult = paragraphResult.TextResult,
                                RowIndex = rowIndex,
                                ColumnIndex = cellIndex,
                            };

                            foreach(var macth in cellResult.TextResult.Matches) {
                                // TODO: 何かしらで外部に逃がしたい
                                macth.Header = $"TABLE";
                                macth.Footer = $"[{rowIndex + 1}:{cellIndex + 1}]";
                            }
                            tableResult.CellResults.Add(cellResult);
                        }
                    }
                }
            }

            tableResult.IsMatched = tableResult.CellResults.Any(c => c.IsMatched);
            return tableResult;
        }

        MicrosoftOfficeWordElementSearchResult SearchElement(int elementIndex, IBodyElement element, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            switch(element.ElementType) {
                case BodyElementType.CONTENTCONTROL:
                    Log.Out.Information($"{element.ElementType}");
                    goto default;

                case BodyElementType.PARAGRAPH:
                    return SearchParagraph2007(elementIndex, (XWPFParagraph)element, regex, setting);

                case BodyElementType.TABLE:
                    return SearchTable2007(elementIndex, (XWPFTable)element, regex, setting);

                default:
                    return MicrosoftOfficeWordElementSearchResult.NotFound;
            }
        }

        public MicrosoftOfficeWordSearchResult Search(MicrosoftOfficeFileType wordType, Stream stream, Regex regex, IReadOnlyFindMicrosoftOfficeWordContentSetting setting)
        {
            var op = new MicrosoftOfficeOperator();
            var doc = op.GetDocument(stream, wordType);

            var result = new MicrosoftOfficeWordSearchResult() {
                OfficeType = wordType,
            };


            foreach(var (element, index) in doc.BodyElements.Select((e, i) => (e, i))) {
                var elementResult = SearchElement(index, element, regex, setting);
                if(elementResult.IsMatched) {
                    result.ElementResults.Add(elementResult);
                }
            }

            result.IsMatched = result.ElementResults.Any(i => i.IsMatched);

            return result;
        }

        #endregion
    }
}
