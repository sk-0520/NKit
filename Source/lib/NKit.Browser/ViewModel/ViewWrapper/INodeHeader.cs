using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Browser.ViewModel.ViewWrapper
{
    interface INodeHeader
    {
        #region property

        string DisplayHeader { get; }

        bool IsSelected { get; set; }

        #endregion
    }
}
