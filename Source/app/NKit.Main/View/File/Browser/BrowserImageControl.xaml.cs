using System;
using System.Collections.Generic;
using System.IO;
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
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    /// <summary>
    /// BrowserImageControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BrowserImageControl : UserControl
    {
        public BrowserImageControl()
        {
            InitializeComponent();
        }

        #region command

        public ICommand ResetCommand => new DelegateCommand(() => {
            this.zoom.Value = 1;
        });

        #endregion

        #region function

        ImageSource GetImageSource(FileInfo fileInfo)
        {
            using(var stream = fileInfo.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)) {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
                var bitmap = new WriteableBitmap(decoder.Frames[0]);
                bitmap.Freeze();
                return bitmap;
            }
        }

        void SetBrowser(BrowserViewModel browser)
        {
            this.image.Source = GetImageSource(browser.FileInfo);
        }

        #endregion

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;
            if(browser != null && browser.IsImage) {
                SetBrowser(browser);
            }
        }
    }
}
