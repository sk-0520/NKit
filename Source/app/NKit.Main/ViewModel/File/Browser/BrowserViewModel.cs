using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel.File.Browser
{
    public class BrowserViewModel : SingleModelViewModelBase<BrowserModel>
    {
        public BrowserViewModel(BrowserModel model)
            : base(model)
        { }
    }
}
