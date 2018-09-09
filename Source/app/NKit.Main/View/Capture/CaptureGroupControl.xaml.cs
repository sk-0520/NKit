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

        ICommand _ScrollCommand;
        public ICommand ScrollCommand
        {
            get
            {
                if(this._ScrollCommand == null) {
                    this._ScrollCommand = new DelegateCommand<InteractionRequestedEventArgs>(p => Scroll((ScrollNotification<CaptureImageViewModel>)p.Context));
                }
                return this._ScrollCommand;
            }
        }

        #endregion

        #region function

        void Scroll(ScrollNotification<CaptureImageViewModel> scrollNotification)
        {
            this.captureItemsContainer.ScrollIntoView(scrollNotification.Target);
        }

        #endregion
    }
}
