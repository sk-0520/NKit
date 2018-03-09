using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ContentTypeTextNet.NKit.Main.Model.Microsoft.Office
{
    public class ExcelSheet : RawModel<ISheet>
    {
        public ExcelSheet(ISheet sheet)
            : base(sheet)
        { }

        #region property
        #endregion

        #region function

        [Obsolete]
        public IReadOnlyList<IExcelShape> GetShapes()
        {
            var patriarchBase = Raw.CreateDrawingPatriarch();

            if(patriarchBase is HSSFPatriarch patriarch) {
                // xls
                return patriarch.GetShapes().Select(s => new Excel1997Shape(s)).ToList();
            } else if(patriarchBase is XSSFDrawing drawing) {
                // xlsx
                return drawing.GetShapes().Select(s => new Excel2007Shape(s)).ToList();
            }

            // そもそも到達すんのかね
            return Enumerable.Empty<IExcelShape>().ToList();
        }

        #endregion
    }

    public class ExcelCell : RawModel<ICell>
    {
        public ExcelCell(ICell cell)
            : base(cell)
        { }

        #region property
        #endregion

        #region function

        string GetDisplayTextFromDate()
        {
            Debug.Assert(Raw.CellType == CellType.Numeric || (Raw.CellType == CellType.Formula && Raw.CachedFormulaResultType == CellType.Numeric));
            Debug.Assert(DateUtil.IsCellDateFormatted(Raw));

            var format = Raw.CellStyle.GetDataFormatString();
            try {
                return Raw.DateCellValue.ToString(format);
            } catch(FormatException ex) {
                Logger.Warning(ex);
                return Raw.DateCellValue.ToString("u");
            }
        }

        string GetDisplayTextFromNumeric()
        {
            Debug.Assert(Raw.CellType == CellType.Numeric || (Raw.CellType == CellType.Formula && Raw.CachedFormulaResultType == CellType.Numeric));

            if(DateUtil.IsCellDateFormatted(Raw)) {
                return GetDisplayTextFromDate();
            }
            return Raw.NumericCellValue.ToString();
        }

        string GetDisplayTextFromString()
        {
            Debug.Assert(Raw.CellType == CellType.String || (Raw.CellType == CellType.Formula && Raw.CachedFormulaResultType == CellType.String));

            return Raw.StringCellValue.ToString();
        }

        string GetDisplayTextCore(CellType cellType)
        {
            switch(cellType) {
                case CellType.Numeric:
                    return GetDisplayTextFromNumeric();

                case CellType.String:
                    return GetDisplayTextFromString();

                case CellType.Formula:
                    throw new NotSupportedException($"${cellType}: 来ないハズ！");

                default:
                    return Raw.ToString();
            }
        }

        public string GetDisplayText()
        {
            if(Raw.CellType == CellType.Formula) {
                return GetDisplayTextCore(Raw.CachedFormulaResultType);
            }
            return GetDisplayTextCore(Raw.CellType);
        }


        public void SetValue(object o)
        {
            if(o == null || Convert.IsDBNull(o)) {
                Raw.SetCellValue(default(string));
            } else {
                var type = o.GetType();
                var map = new Dictionary<Type, Action>() {
                    { typeof(string), () => Raw.SetCellValue((string)o) },
                    { typeof(int), () => Raw.SetCellValue((int)o) },
                    { typeof(uint), () => Raw.SetCellValue((uint)o) },
                    { typeof(long), () => Raw.SetCellValue((long)o) },
                    { typeof(ulong), () => Raw.SetCellValue((ulong)o) },
                    { typeof(DateTime), () => Raw.SetCellValue((DateTime)o) },
                    { typeof(bool), () => Raw.SetCellValue((bool)o) },
                    { typeof(IRichTextString), () => Raw.SetCellValue((IRichTextString)o) },
                };
                Action action;
                if(map.TryGetValue(type, out action)) {
                    action();
                } else {
                    // 来ないことを祈る
                    Debug.WriteLine($"{type} is not found");
                }
            }
        }

        #endregion
    }

    #region こんなもん使えんわ

    public interface IExcelShape
    {
        #region property
        #endregion

        #region function
        #endregion
    }

    public class Excel1997Shape : RawModel<HSSFShape>, IExcelShape
    {
        public Excel1997Shape(HSSFShape shape)
            : base(shape)
        { }

        #region IExcelShape
        #endregion
    }

    public class Excel2007Shape : RawModel<XSSFShape>, IExcelShape
    {
        public Excel2007Shape(XSSFShape shape)
            : base(shape)
        { }

        #region IExcelShape
        #endregion
    }

    #endregion

}
