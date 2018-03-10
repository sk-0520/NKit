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
using ContentTypeTextNet.NKit.Setting.Define;

namespace ContentTypeTextNet.NKit.Main.View.Control
{
    /// <summary>
    /// FlagMatchSelectControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FlagMatchSelectControl : UserControl
    {
        public FlagMatchSelectControl()
        {
            InitializeComponent();
        }

        #region FlagMatchKind

        public FlagMatchKind FlagMatchKind
        {
            get { return (FlagMatchKind)GetValue(FlagMatchKindProperty); }
            set { SetValue(FlagMatchKindProperty, value); }
        }

        public static readonly DependencyProperty FlagMatchKindProperty = DependencyProperty.Register(
             nameof(FlagMatchKind),
             typeof(FlagMatchKind),
             typeof(FlagMatchSelectControl),
             new PropertyMetadata(FlagMatchKind.Has, new PropertyChangedCallback(FlagMatchKindChanged)));

        private static void FlagMatchKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FlagMatchSelectControl).FlagMatchKind = (FlagMatchKind)e.NewValue;
        }


        #endregion

    }
}
