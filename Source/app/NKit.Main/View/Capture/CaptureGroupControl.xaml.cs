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
using ContentTypeTextNet.NKit.Main.ViewModel.Capture;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

namespace ContentTypeTextNet.NKit.Main.View.Capture
{
    /// <summary>
    /// CaptureGroupControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureGroupControl : UserControl
    {
        public CaptureGroupControl()
        {
            InitializeComponent();
        }

        #region command

        public ICommand ScrollCommand => new DelegateCommand<InteractionRequestedEventArgs>(p => Scroll((ScrollNotification<CaptureImageViewModel>)p.Context));

        #endregion

        #region function

        void Scroll(ScrollNotification<CaptureImageViewModel> scrollNotification)
        {
            this.captureItemsScroller.ScrollToBottom();
        }

        #endregion
    }
}
