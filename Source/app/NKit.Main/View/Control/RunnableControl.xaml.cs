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
using ContentTypeTextNet.NKit.Utility.ViewModell;

namespace ContentTypeTextNet.NKit.Main.View.Control
{
    /// <summary>
    /// Runnable.xaml の相互作用ロジック
    /// </summary>
    public partial class RunnableControl : UserControl
    {
        public RunnableControl()
        {
            InitializeComponent();
        }

        #region RunnableItem

        public IRunnableItem RunnableItem
        {
            get { return (IRunnableItem)GetValue(RunnableItemProperty); }
            set { SetValue(RunnableItemProperty, value); }
        }

        public static readonly DependencyProperty RunnableItemProperty = DependencyProperty.Register(
             nameof(RunnableItem),
             typeof(IRunnableItem),
             typeof(RunnableControl),
             new PropertyMetadata(default(IRunnableItem), new PropertyChangedCallback(RunnableItemChanged)));

        private static void RunnableItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RunnableControl).RunnableItem = (IRunnableItem)e.NewValue;
        }


        #endregion

        #region RunContent

        public object RunContent
        {
            get { return (object)GetValue(RunContentProperty); }
            set { SetValue(RunContentProperty, value); }
        }

        public static readonly DependencyProperty RunContentProperty = DependencyProperty.Register(
             nameof(RunContent),
             typeof(object),
             typeof(RunnableControl),
             new PropertyMetadata(Properties.Resources.String_View_Control_RunnableControl_RunContent_Text, new PropertyChangedCallback(RunContentChanged)));

        private static void RunContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RunnableControl).RunContent = e.NewValue;
        }


        #endregion

        #region CancelContent
        public object CancelContent
        {
            get { return (object)GetValue(CancelContentProperty); }
            set { SetValue(CancelContentProperty, value); }
        }

        public static readonly DependencyProperty CancelContentProperty = DependencyProperty.Register(
             nameof(CancelContent),
             typeof(object),
             typeof(RunnableControl),
             new PropertyMetadata(Properties.Resources.String_View_Control_RunnableControl_CancelContent_Text, new PropertyChangedCallback(CancelContentChanged)));

        private static void CancelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RunnableControl).CancelContent = e.NewValue;
        }


        #endregion

    }
}
