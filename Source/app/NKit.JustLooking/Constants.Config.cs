using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.JustLooking
{
    partial class Constants
    {
        #region property

        public static Encoding DefaultEncoding => EncodingUtility.Parse(ConfigurationManager.AppSettings["default-encoding"]);

        #endregion
    }
}
