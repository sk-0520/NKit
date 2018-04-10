using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.JustLooking.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.JustLooking.ViewModel
{
    public class JustLookingViewModel : SingleModelViewModelBase<JustLookingModel>
    {
        public JustLookingViewModel(JustLookingModel model)
            : base(model)
        { }

        #region property

        public BrowserViewModel Browser => new BrowserViewModel(Model.Browser);

        public string FilePath => Model.FilePath;

        #endregion

        #region command
        #endregion

        #region function
        #endregion
    }
}
