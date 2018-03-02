using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Main.Model;
using NPOI.SS.Util;

namespace ContentTypeTextNet.NKit.Main.View.Converter
{

    [ValueConversion(typeof(IReadOnlyMicrosoftOfficeExcelCellAddress), typeof(string))]
    public class MicrosoftOfficeExcelCellAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cellAddress = (IReadOnlyMicrosoftOfficeExcelCellAddress)value;
            if(value == null) {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            var cellReference = new CellReference(cellAddress.RowIndex, cellAddress.ColumnIndex);
            var result = cellReference.FormatAsString();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
