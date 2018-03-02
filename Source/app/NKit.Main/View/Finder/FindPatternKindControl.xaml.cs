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
using ContentTypeTextNet.NKit.Main.Define;

namespace ContentTypeTextNet.NKit.Main.View.Finder
{
    /// <summary>
    /// FindPatternKindControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FindPatternKindControl : UserControl
    {
        public FindPatternKindControl()
        {
            InitializeComponent();
        }



        #region FindPatternKind

        public FindPatternKind FindPatternKind
        {
            get { return (FindPatternKind)GetValue(FindPatternKindProperty); }
            set { SetValue(FindPatternKindProperty, value); }
        }

        public static readonly DependencyProperty FindPatternKindProperty = DependencyProperty.Register(
             nameof(FindPatternKind),
             typeof(FindPatternKind),
             typeof(FindPatternKindControl),
             new PropertyMetadata(FindPatternKind.PartialMatch, new PropertyChangedCallback(FindPatternKindChanged)));

        private static void FindPatternKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FindPatternKindControl).FindPatternKind = (FindPatternKind)e.NewValue;
        }


        #endregion


    }
}
