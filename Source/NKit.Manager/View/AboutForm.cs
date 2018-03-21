using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
            Text = CommonUtility.ReplaceWindowTitle(Text);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            var notesDir = new DirectoryInfo(Path.Combine(CommonUtility.GetDocumentDirectory().FullName, "release-notes"));
            var releaseNoteItems = notesDir.EnumerateFiles("*.md")
                .Select(f => new {
                    File = f,
                    Name = Path.GetFileNameWithoutExtension(f.Name)
                        .Replace("ver_", string.Empty)
                        .Replace("-", ".")
                    ,
                })
                .Where(i => Version.TryParse(i.Name, out var _))
                .Select(i => new ReleaseNoteItem() {
                    Version = Version.Parse(i.Name),
                    Hash = string.Empty,
                    Timestamp = DateTime.MinValue,
                    Content = File.ReadAllText(i.File.FullName)
                })
                .ToList()
            ;
            this.releaseNoteControl1.SetReleaseNotes("history", releaseNoteItems);
        }

        private void commandClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
