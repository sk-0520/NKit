using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main
{
    partial class Constants
    {
        #region variable

        static ConfigurationCacher appConfig = new ConfigurationCacher();

        #endregion

        #region property

        public static int FinderHistoryLimit { get; } = int.Parse(ConfigurationManager.AppSettings["finder-history-limit"]);

        #endregion
    }
}
