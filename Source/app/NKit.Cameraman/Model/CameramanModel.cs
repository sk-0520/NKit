using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Cameraman.Model
{
    public class CameramanModel: ModelBase
    {
        public CameramanModel(string[] arguments)
        {
            Arguments = arguments;
        }
        #region property

        string[] Arguments { get; }

        #endregion
    }
}
