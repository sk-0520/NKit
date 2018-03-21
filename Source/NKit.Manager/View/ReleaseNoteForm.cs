using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class ReleaseNoteForm : Form
    {
        public ReleaseNoteForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
            Text = CommonUtility.ReplaceWindowTitle(Text);
        }

        #region property

        public Uri ReleaseNoteUri { get; set; }
        public Uri IssueBaseUri { get; set; }

        public Action ExecuteUpdateAction { get; set; }

        #endregion

        #region function

        public void SetUpdatable(bool canUpdate)
        {
            this.commandUpdate.Enabled = canUpdate;
        }

        string ReplaceReleaceNote(string rawValue)
        {
            var issueRegex = new Regex(@"(?<HASH>#)(?<NUMBER>\d+)");
            var issueReplaced = issueRegex.Replace(rawValue, m => {
                var hash = m.Groups["HASH"].Value;
                var number = m.Groups["NUMBER"].Value;

                return $"[{hash}{number}]({IssueBaseUri}/{number})";
            });

            return issueReplaced;
        }

        public void SetReleaseNote(Version version, string releaseHash, DateTime releaseTimestamp, string releaseNoteValue)
        {
            var replacedReleaceNoteValue = ReplaceReleaceNote(releaseNoteValue);
            var html = Properties.Resources.File_ReleaseNoteMarkdown
                .Replace("${VERSION}", version.ToString())
                .Replace("${TIMESTAMP}", releaseTimestamp.ToString("u"))
                .Replace("${HASH}", releaseHash)
                .Replace("${CONTENT}", replacedReleaceNoteValue)
            ;
            this.viewReleaseNote.DocumentStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        }

        #endregion

        private void commandClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void commandUpdate_Click(object sender, EventArgs e)
        {
            ExecuteUpdateAction?.Invoke();
        }

        private void viewReleaseNote_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var urlText = e.Url.ToString();
            if(urlText.StartsWith(IssueBaseUri.ToString(), StringComparison.InvariantCultureIgnoreCase)) {
                Process.Start(urlText);
                e.Cancel = true;
            }
        }
    }
}
