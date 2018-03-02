using System;
using System.Collections.Generic;
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
using ContentTypeTextNet.NKit.Main.ViewModel.Finder;

namespace ContentTypeTextNet.NKit.Main.View.Finder
{
    /// <summary>
    /// FinderSingleFileControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FinderSingleFileControl : UserControl
    {
        public FinderSingleFileControl()
        {
            InitializeComponent();
        }


        #region SingleItem

        public FindItemViewModel SingleItem
        {
            get { return (FindItemViewModel)GetValue(SingleItemProperty); }
            set { SetValue(SingleItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleItemProperty = DependencyProperty.Register(
            nameof(SingleItem),
            typeof(FindItemViewModel),
            typeof(FinderSingleFileControl),
            new PropertyMetadata(default(FindItemViewModel), SingleItemPropertyChanged)
        );

        private static void SingleItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FinderSingleFileControl)d).SingleItem = (FindItemViewModel)e.NewValue;
        }

        #endregion

    }
}
