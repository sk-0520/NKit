using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public interface ISelectable
    {
        #region property

        bool IsSelected { get; set; }

        #endregion
    }
}
