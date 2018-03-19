using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ContentTypeTextNet.NKit.Utility.View.Converter
{
    public class FilePathToImageConverter : IValueConverter
    {
        #region property

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filePath = (string)value;
            if(string.IsNullOrWhiteSpace(filePath)) {
                return value;
            }

            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
                var bitmap = new WriteableBitmap(decoder.Frames[0]);
                bitmap.Freeze();
                return bitmap;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
