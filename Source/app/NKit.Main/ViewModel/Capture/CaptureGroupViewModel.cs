using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModell;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Capture
{
    public class CaptureGroupViewModel : RunnableViewModelBase<CaptureGroupModel, None>
    {
        public CaptureGroupViewModel(CaptureGroupModel model)
            : base(model)
        { }

        #region property

        public string GroupName
        {
            get { return Model.GroupSetting.GroupName; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public CaptureMode CaptureMode
        {
            get { return Model.GroupSetting.CaptureMode; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool IsImmediateSelect
        {
            get { return Model.GroupSetting.IsImmediateSelect; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool OverwriteScrollSetting
        {
            get { return Model.GroupSetting.OverwriteScrollSetting; }
            set { SetPropertyValue(Model.GroupSetting, value); }
        }

        public bool IsEnabledHideHeader
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Header.IsEnabled; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Header, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Header.IsEnabled)); }
        }
        public string HideHeaderElement
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Header.HideElements; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Header, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Header.HideElements)); }
        }

        public bool IsEnabledHideFooter
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Footer.IsEnabled; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Footer, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Footer.IsEnabled)); }
        }
        public string HideFooterElement
        {
            get { return Model.GroupSetting.Scroll.InternetExplorer.Footer.HideElements; }
            set { SetPropertyValue(Model.GroupSetting.Scroll.InternetExplorer.Footer, value, nameof(Model.GroupSetting.Scroll.InternetExplorer.Footer.HideElements)); }
        }


        #endregion

        #region command
        #endregion

        #region function
        #endregion
    }
}
