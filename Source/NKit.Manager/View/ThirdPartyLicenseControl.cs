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

        #region function

        string FormatComponent(ThirdPartyComponent component)
        {
            var sb = new StringBuilder();

            void AppendPairLine(string head, string body) {
                sb.Append('\t');
                sb.Append(head);
                sb.Append(": ");
                sb.Append(body);
                sb.AppendLine();
            }

            sb.Append(component.Name);
            sb.AppendLine();
            if(!string.IsNullOrEmpty(component.ProjectUri)) {
                AppendPairLine("project", component.ProjectUri);
            }

            AppendPairLine("license", component.LicenseName);
            if(!string.IsNullOrEmpty(component.LicenseUri)) {
                AppendPairLine("license page", component.LicenseUri);
            }

            return sb.ToString();
        }

        #endregion

        private void ThirdPartyLicenseControl_Load(object sender, EventArgs e)
        {
            if(DesignMode) {
                return;
            }

            var path = Path.Combine(CommonUtility.GetDocumentDirectory().FullName, "license", "third-party.xml");
            var license = ManagerWorker.LoadXmlObject<ThirdPartyLicense>(path);
            this.viewComponents.Text = string.Join(Environment.NewLine, license.Components.Select(c => FormatComponent(c)));

        }
    }
}
