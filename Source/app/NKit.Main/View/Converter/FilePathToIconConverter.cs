using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.View.Converter
{
    public class FilePathToIconConverter : IValueConverter
    {
        #region property

        public IconScale IconScale { get; set; } = IconScale.Small;

        Cacher<string, ImageSource> FullPathIconCacher { get; } = new Cacher<string, ImageSource>();
        Cacher<string, ImageSource> ExtensionIconCacher { get; } = new Cacher<string, ImageSource>();

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            if(string.IsNullOrWhiteSpace(path)) {
                return null;
            }

            if(PathUtility.HasIconPath(path)) {
                return FullPathIconCacher.Get(path, () => IconUtility.Load(path, IconScale, 0));
            } else {
                var ext = Path.GetExtension(path).Replace(".", string.Empty).ToLower();
                return ExtensionIconCacher.Get(ext, () => IconUtility.Load(path, IconScale, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
