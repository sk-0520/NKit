using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class ApplicationSwitcher: ModelBase
    {
        public ApplicationSwitcher(Uri serviceUri)
        {
            if(serviceUri != null) {
                ApplicationClient = new NKitApplicationTalkerClient(NKitApplicationKind.Main, serviceUri);
            }
        }

        #region property

        NKitApplicationTalkerClient ApplicationClient { get; }

        #endregion

        #region function

        public void WakeupApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            if(ApplicationClient != null) {

            }

            ApplicationClient.WakeupApplication(targetApplication, arguments, workingDirectoryPath);
        }

        #endregion
    }
}
