using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.View.Converter
{
    [ValueConversion(typeof(byte[]), typeof(string))]
    public class BinaryToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) {
                return DependencyProperty.UnsetValue;
            }
            var binary = (byte[])value;
            var dc = new DisplayConverter();
            return dc.ToHexString(binary);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
