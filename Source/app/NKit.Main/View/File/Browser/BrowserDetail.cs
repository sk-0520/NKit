using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Main.ViewModel.File.Browser;

namespace ContentTypeTextNet.NKit.Main.View.File.Browser
{
    public interface IBrowserDetail
    {
        bool CanBrowse(BrowserViewModel browser);
        void BuildControl(BrowserViewModel browser);
    }

    public class BrowserDetail<TUserControl>
        where TUserControl : UserControl, IBrowserDetail
    {
        public BrowserDetail(TUserControl userControl)
        {
            UserControl = userControl;
            UserControl.DataContextChanged += BrowserDetailControl_DataContextChanged;
        }

        #region property

        TUserControl UserControl { get; set; }
        BrowserViewModel Browser { get; set; }

        #endregion

        #region function

        public Func<BrowserViewModel, bool> CanBrowse { get; set; }

        public Action<BrowserViewModel> BuildControl { get; set; }

        #endregion

        private void BrowserDetailControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;
            if(browser != null && CanBrowse(browser)) {
                Browser = browser;
                BuildControl(Browser);
            } else {
                Browser = null;
            }
        }

    }

    public static class BrowserDetail
    {
        public static BrowserDetail<TUserControl> Create<TUserControl>(TUserControl userControl)
        where TUserControl : UserControl, IBrowserDetail
        {
            var detail = new BrowserDetail<TUserControl>(userControl);
            detail.CanBrowse = userControl.CanBrowse;
            detail.BuildControl = userControl.BuildControl;

            return detail;
        }
}
}
