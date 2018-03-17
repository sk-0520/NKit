using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContentTypeTextNet.NKit.Main.View.Control
{
    /// <summary>
    /// KeySettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class KeySettingControl : UserControl
    {
        public KeySettingControl()
        {
            InitializeComponent();

            var alphabet = GetKeysRange(Key.A, Key.Z);
            var numbersDigitals = GetKeysRange(Key.D0, Key.D9);
            var numbersPads = GetKeysRange(Key.NumPad0, Key.NumPad9);
            var function = GetKeysRange(Key.F1, Key.F22);

            var keys = new List<Key>();
            keys.Add(Key.None);
            keys.AddRange(alphabet);
            keys.AddRange(numbersDigitals);
            keys.AddRange(numbersPads);
            keys.AddRange(function);

            ItemsSource = new ObservableCollection<Key>(keys);
        }

        #region KeyItems

        #region ItemsSource

        public IEnumerable<Key> ItemsSource
        {
            get { return (IEnumerable<Key>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<Key>),
            typeof(KeySettingControl),
            new PropertyMetadata(Enumerable.Empty<Key>(), PropertyChangedCallback)
        );

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KeySettingControl)d).ItemsSource = (IEnumerable<Key>)e.NewValue;
        }

        #endregion

        #endregion

        #region Key

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
             nameof(Key),
             typeof(Key),
             typeof(KeySettingControl),
             new PropertyMetadata(Key.None, new PropertyChangedCallback(KeyChanged)));

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as KeySettingControl).Key = (Key)e.NewValue;
        }


        #endregion

        #region ModifierKeys

        public ModifierKeys ModifierKeys
        {
            get { return (ModifierKeys)GetValue(ModifierKeysProperty); }
            set { SetValue(ModifierKeysProperty, value); }
        }

        public static readonly DependencyProperty ModifierKeysProperty = DependencyProperty.Register(
             nameof(ModifierKeys),
             typeof(ModifierKeys),
             typeof(KeySettingControl),
             new PropertyMetadata(ModifierKeys.None, new PropertyChangedCallback(ModifierKeysChanged)));

        private static void ModifierKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as KeySettingControl).ModifierKeys = (ModifierKeys)e.NewValue;
        }


        #endregion

        #region function

        static IEnumerable<Key> GetKeysRange(Key startKey, Key endKey)
        {
            return Enumerable.Range((int)startKey, endKey - startKey + 1).Cast<Key>();
        }

        #endregion

    }
}
