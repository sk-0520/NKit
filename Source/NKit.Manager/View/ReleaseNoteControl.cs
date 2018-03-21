using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Manager.Model;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public class ReleaseNoteControl: WebBrowser
    {
        #region property

        public Uri IssueBaseUri { get; set; }

        #endregion

        #region function

        string ReplaceReleaceNoteContent(string rawValue)
        {
            var issueRegex = new Regex(@"(?<HASH>#)(?<NUMBER>\d+)");
            var issueReplaced = issueRegex.Replace(rawValue, m => {
                var hash = m.Groups["HASH"].Value;
                var number = m.Groups["NUMBER"].Value;

                return $"[{hash}{number}]({IssueBaseUri ?? Constants.IssuesBaseUri}/{number})";
            });

            return issueReplaced;
        }

        string ReplaceReleaseNoteItem(ReleaseNoteItem item)
        {
            var replacedContent = ReplaceReleaceNoteContent(item.Content);
            var html = Properties.Resources.File_ReleaseNoteLayout
                .Replace("${VERSION}", item.Version.ToString())
                .Replace("${TIMESTAMP}", item.Timestamp == DateTime.MinValue ? string.Empty: item.Timestamp.ToString("u"))
                .Replace("${HASH}", item.Hash)
                .Replace("${CONTENT}", replacedContent)
            ;
            return html;
        }

        public void SetReleaseNotes(string title, IEnumerable<ReleaseNoteItem> items)
        {
            var releaseNoteContents = string.Join(Environment.NewLine, items.Select(i => ReplaceReleaseNoteItem(i)));

            var html = Properties.Resources.File_ReleaseNoteDocument
                .Replace("${TITLE}", title)
                .Replace("${ALL-CONTENTS}", releaseNoteContents)
            ;
            DocumentStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        }


        #endregion

        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            var urlText = e.Url.ToString();
            if(urlText.StartsWith((IssueBaseUri ?? Constants.IssuesBaseUri).ToString(), StringComparison.InvariantCultureIgnoreCase)) {
                Process.Start(urlText);
                e.Cancel = true;
            }

            base.OnNavigating(e);
        }
    }
}
