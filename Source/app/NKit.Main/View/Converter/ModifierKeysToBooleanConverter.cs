using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ContentTypeTextNet.NKit.Main.View.Converter
{
    public class ModifierKeysToBooleanConverter : IValueConverter
    {
        #region property

        ModifierKeys ModifierKeys { get; set; }

        #endregion

        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModifierKeys mask = (ModifierKeys)parameter;
            ModifierKeys = (ModifierKeys)value;
            return ((mask & ModifierKeys) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModifierKeys ^= (ModifierKeys)parameter;
            return ModifierKeys;
        }

        #endregion
    }
}
