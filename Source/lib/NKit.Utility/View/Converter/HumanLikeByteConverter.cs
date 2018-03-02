using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.View.Converter
{
    [ValueConversion(typeof(long), typeof(string))]
    public class HumanLikeByteConverter : IValueConverter
    {
        #region property

        public string SizeFormat { get; set; }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = System.Convert.ToInt64(value);

            var dc = new DisplayConverter();

            if(string.IsNullOrWhiteSpace(SizeFormat)) {
                return dc.ToHumanLikeByte(size);
            }

            return dc.ToHumanLikeByte(size, SizeFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
