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
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Main.View.Control
{
    /// <summary>
    /// SearchPatternSelectControl .xaml の相互作用ロジック
    /// </summary>
    public partial class SearchPatternSelectControl : UserControl
    {
        public SearchPatternSelectControl()
        {
            InitializeComponent();
        }


        #region SearchPatternKind

        public SearchPatternKind SearchPatternKind
        {
            get { return (SearchPatternKind)GetValue(SearchPatternKindProperty); }
            set { SetValue(SearchPatternKindProperty, value); }
        }

        public static readonly DependencyProperty SearchPatternKindProperty = DependencyProperty.Register(
             nameof(SearchPatternKind),
             typeof(SearchPatternKind),
             typeof(SearchPatternSelectControl),
             new PropertyMetadata(SearchPatternKind.PartialMatch, new PropertyChangedCallback(SearchPatternKindChanged)));

        private static void SearchPatternKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SearchPatternSelectControl).SearchPatternKind = (SearchPatternKind)e.NewValue;
        }


        #endregion


    }
}
