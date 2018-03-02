using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ContentTypeTextNet.NKit.Utility.View.Converter
{
    [ValueConversion(typeof(KeyBinding), typeof(string))]
    public class KeyBindingToGestureTextConverter : IValueConverter
    {
        #region define

        static KeyBinding DefaultKeyBinding { get; } = new KeyBinding();

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var keyBinding = value as KeyBinding;
            if(keyBinding == null) {
                return value;
            }

            var keys = new List<string>(5);

            // 装飾キー
            if(keyBinding.Modifiers != ModifierKeys.None) {
                if(keyBinding.Modifiers.HasFlag(ModifierKeys.Windows)) {
                    keys.Add("Windows");
                }
                if(keyBinding.Modifiers.HasFlag(ModifierKeys.Control)) {
                    keys.Add("Ctrl");
                }
                if(keyBinding.Modifiers.HasFlag(ModifierKeys.Shift)) {
                    keys.Add("Shift");
                }
                if(keyBinding.Modifiers.HasFlag(ModifierKeys.Alt)) {
                    keys.Add("Alt");
                }
            }

            keys.Add(keyBinding.Key.ToString());

            return string.Join(" + ", keys);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
