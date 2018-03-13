using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Capture;
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

        #endregion

        #region command
        #endregion

        #region function
        #endregion
    }
}
