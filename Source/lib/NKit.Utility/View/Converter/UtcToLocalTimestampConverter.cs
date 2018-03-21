using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.View.Converter
{
    [ValueConversion(typeof(DateTime), typeof(DateTime))]
    public class UtcToLocalTimestampConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) {
                return null;
            }

            if(value is DateTime timestamp) {
                if(timestamp.Kind == DateTimeKind.Utc) {
                    return timestamp.ToLocalTime();
                }
                return timestamp;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) {
                return null;
            }

            if(value is DateTime timestamp) {
                if(timestamp.Kind == DateTimeKind.Local) {
                    return timestamp.ToUniversalTime();
                }
                return timestamp;
            }

            if(value is string timestampFormat) {
                if(DateTime.TryParse(timestampFormat, out var convertedTimestamp)){
                    if(convertedTimestamp.Kind == DateTimeKind.Local) {
                        return convertedTimestamp.ToUniversalTime();
                    }
                    return convertedTimestamp;
                }
            }

            if(value is long timestampBinnary) {
                try {
                    var convertedTimestamp = DateTime.FromBinary(timestampBinnary);
                    if(convertedTimestamp.Kind == DateTimeKind.Local) {
                        return convertedTimestamp.ToUniversalTime();
                    }
                    return convertedTimestamp;
                } catch(ArgumentException ex) {
                    Log.Out.Error(ex);
                }
            }

            return value;
        }

        #endregion
    }
}
