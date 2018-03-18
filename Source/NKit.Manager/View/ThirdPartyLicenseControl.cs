using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class ThirdPartyLicenseControl : UserControl
    {
        public ThirdPartyLicenseControl()
        {
            InitializeComponent();
        }

        private void ThirdPartyLicenseControl_Load(object sender, EventArgs e)
        {
            if(DesignMode) {
                return;
            }

            var path = Path.Combine(CommonUtility.GetDocumentDirectory().FullName, "license", "third-party.xml");
            var license = ManagerWorker.LoadXmlObject<ThirdPartyLicense>(path);
        }
    }
}
