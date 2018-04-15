using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public interface ISelectable
    {
        #region property

        bool IsSelected { get; set; }

        #endregion
    }

    public interface IExpandable
    {
        #region property

        bool IsExpanded { get; set; }

        #endregion
    }
}
