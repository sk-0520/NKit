using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public abstract class SearchParameterBase
    { }

    public abstract class SearchResultBase
    {
        #region property

        public bool IsMatched { get; set; }

        #endregion
    }
}
