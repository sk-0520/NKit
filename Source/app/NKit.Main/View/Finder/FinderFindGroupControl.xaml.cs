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
using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

namespace ContentTypeTextNet.NKit.Main.View.Finder
{
    /// <summary>
    /// FinderFindGroupControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FinderFindGroupControl : UserControl
    {
        public FinderFindGroupControl()
        {
            InitializeComponent();
        }

        #region command

        ICommand _SaveFileDialogCommand;
        public ICommand SaveFileDialogCommand
        {
            get
            {
                if(this._SaveFileDialogCommand == null) {
                    this._SaveFileDialogCommand = new DelegateCommand<InteractionRequestedEventArgs>(
                        o => {
                            var confirmation = (Confirmation)o.Context;
                            var dialog = (SaveFileDialog)confirmation.Content;
                            confirmation.Confirmed = dialog.ShowDialog().GetValueOrDefault();
                        }
                    );
                }

                return this._SaveFileDialogCommand;
            }
        }

        #endregion
    }
}
