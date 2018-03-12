using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class ReleaseNoteForm : Form
    {
        public ReleaseNoteForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
        }

        #region property

        public Uri ReleaseNoteUri { get; set; }
        public Uri IssueBaseUri { get; set; }

        #endregion

        #region function

        IEnumerable<string> ReadLines(string text)
        {
            using(var reader = new StringReader(text)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    yield return line;
                }
            }
        }

        void AppendAndSelect(string s, Action<RichTextBox> action = null)
        {
            this.viewReleaseNote.AppendText(s);
            var start = this.viewReleaseNote.TextLength - s.Length;
            var length = s.Length;
            this.viewReleaseNote.Select(start, length);
            if(action != null) {
                var old = new {
                    SelectionColor = this.viewReleaseNote.SelectionColor,
                    SelectionBackColor = this.viewReleaseNote.SelectionBackColor,
                    SelectionAlignment = this.viewReleaseNote.SelectionAlignment,
                    SelectionFont = this.viewReleaseNote.SelectionFont,
                };

                action(this.viewReleaseNote);

                this.viewReleaseNote.Select(start, length);

                this.viewReleaseNote.SelectionColor = old.SelectionColor;
                this.viewReleaseNote.SelectionBackColor = old.SelectionBackColor;
                this.viewReleaseNote.SelectionAlignment = old.SelectionAlignment;
                this.viewReleaseNote.SelectionFont = old.SelectionFont;
            }
        }

        void Break()
        {
            this.viewReleaseNote.AppendText(Environment.NewLine);
        }

        public void SetReleaseNote(Version version, string releaseHash, DateTime releaseTimestamp, string releaseNoteValue)
        {
            this.viewReleaseNote.Clear();

            var header = version.ToString();
            if(releaseTimestamp != DateTime.MinValue) {
                header += ": " + releaseTimestamp;
            }
            AppendAndSelect(header, r => {
                r.SelectionFont = new Font(Font, FontStyle.Bold);
            });

            Break();

            AppendAndSelect($"commit: {releaseHash}");

            Break();

            // シンプルな Markdown を書くようにすれば自力でパースできると思うのでセルフ優しさが必要
            var lines = ReadLines(releaseNoteValue);
            var uriRegex = new Regex(@"(<URI>http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?)");
            var issueRegex = new Regex(@"#(?<ISSUE>\d+)");

            var defaultIndent = this.viewReleaseNote.SelectionIndent;

            foreach(var line in lines) {
                Break();

                var usingLine = line;
                var listItemPosition = line.IndexOf('*');
                var isListItem = listItemPosition != -1;

                if(isListItem) {
                    usingLine = usingLine.Substring(listItemPosition + 1);
                }

                AppendAndSelect(usingLine, r => {
                    if(isListItem) {
                        r.SelectionIndent = listItemPosition * 10;
                        r.SelectionBullet = true;
                    } else {
                        r.SelectionIndent = defaultIndent;
                        r.SelectionBullet = false;
                    }
                });
            }
        }

        #endregion
    }
}
