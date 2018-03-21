using System;
using System.Collections.Generic;
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

        public Uri IssueBaseUri { get; } = Constants.IssuesBaseUri;

        #endregion

        #region function

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

        public void SetReleaseNote(string title, IEnumerable<ReleaseNoteItem> items)
        {
            //var replacedReleaceNoteValue = ReplaceReleaceNote(releaseNoteValue);
            //var html = Properties.Resources.File_ReleaseNoteMarkdown
            //    .Replace("${VERSION}", version.ToString())
            //    .Replace("${TIMESTAMP}", releaseTimestamp.ToString("u"))
            //    .Replace("${HASH}", releaseHash)
            //    .Replace("${CONTENT}", replacedReleaceNoteValue)
            //;
            //this.viewReleaseNote.DocumentStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        }


        #endregion
    }
}
