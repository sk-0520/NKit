using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Browser.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.View
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
            UserControl.IsVisibleChanged += UserControl_IsVisibleChanged;
        }

        #region property

        TUserControl UserControl { get; set; }
        BrowserViewModel Browser { get; set; }

        public Func<BrowserViewModel, bool> CanBrowse { get; set; }

        public Action<BrowserViewModel> BuildControl { get; set; }

        #endregion

        #region function

        void Build()
        {
            if(UserControl.IsVisible) {
                if(!Browser.IsBuilded && !Browser.IsBuilding) {
                    Browser.IsBuilding = true;

                    BuildControl(Browser);

                    Browser.IsBuilded = true;
                }
            }
        }

        #endregion

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Build();
        }

        private void BrowserDetailControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (BrowserViewModel)e.NewValue;

            // 既に設定済みなら何もしない
            if(Browser == browser) {
                return;
            }

            if(browser != null && CanBrowse(browser)) {
                Browser = browser;
                Build();
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
