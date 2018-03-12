using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager
{
    partial class Constants
    {
        #region property

        public static string UpdateCheckBranchBaseUri => ConfigurationManager.AppSettings["update-check-branch-base-uri"];
        public static string UpdateCheckBranchTargetName => ConfigurationManager.AppSettings["update-check-branch-target-name"];
        public static string UpdateCheckVersionBaseUri => ConfigurationManager.AppSettings["update-check-version-base-uri"];
        public static string UpdateCheckVersionFilePath => ConfigurationManager.AppSettings["update-check-version-file-path"];

        #endregion
    }
}
