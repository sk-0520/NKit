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

        #region property

        string VersionInformation => $"{Assembly.GetExecutingAssembly().GetName().Version}:{ProductVersion}";

        #endregion

        #region function

        void Execute(string target)
        {
            try {
                Process.Start(target);
            }catch(Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        private void AboutForm_Load(object sender, EventArgs e)
        {

            this.viewVersion.Text = VersionInformation;

            var notesDir = new DirectoryInfo(Path.Combine(CommonUtility.GetDocumentDirectory().FullName, "release-notes"));
            var releaseNoteItems = notesDir.EnumerateFiles("ver_*.md", SearchOption.AllDirectories)
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
                .OrderByDescending(i => i.Version)
                .ToList()
            ;
            this.releaseNoteControl1.SetReleaseNotes("history", releaseNoteItems);
        }

        private void commandClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Execute(Constants.ApplicationWebPage);
        }

        private void linkDevelopment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Execute(Constants.ApplicationDevelopmentPage);
        }

        private void linkForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Execute(Constants.ApplicationForumPage);
        }

        private void commandApp_Click(object sender, EventArgs e)
        {
            Execute(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location));
        }

        private void commandData_Click(object sender, EventArgs e)
        {
            Execute(CommonUtility.GetUserDirectory().FullName);
        }

        private void commandVersionCopy_Click(object sender, EventArgs e)
        {
            try {
                Clipboard.SetText(VersionInformation);
            } catch(Exception ex) {
                Trace.TraceError(ex.ToString());
            }
        }
    }
}
