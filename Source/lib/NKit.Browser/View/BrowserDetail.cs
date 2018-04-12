using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Common;

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
                if(Browser.CanBuild(UserControl)) {
                    Browser.BeginBuild(UserControl);
                    using(new ActionDisposer(d => { Browser.EndBuild(UserControl); })) {
                        BuildControl(Browser);
                    }
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

            if(browser != null && !browser.IsDisposed && CanBrowse(browser)) {
                Browser = browser;
                Build();
            } else if(Browser != null) {
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
