using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Searcher;

namespace ContentTypeTextNet.NKit.Main.View.Converter
{
    [ValueConversion(typeof(IEnumerable<SearchResultBase>), typeof(IEnumerable<TextSearchMatch>))]
    public class MicrosoftOfficeExcelMatchesConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value) {
                case IEnumerable<MicrosoftOfficeExcelCellSearchResult> cells:
                    return cells.SelectMany(r => r.Matches.Concat(r.CommentMatch));

                case IEnumerable<MicrosoftOfficeExcelShapeSearchResult> shapes:
                    return shapes.SelectMany(r => r.Matches);

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
