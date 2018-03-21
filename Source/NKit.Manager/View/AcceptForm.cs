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
    public partial class AcceptForm : Form
    {
        public AcceptForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
            Text = CommonUtility.ReplaceWindowTitle(Text);
        }


        #region function

        MainWorker Worker { get; set; }

        #endregion

        #region function

        void AppendVersionMessage()
        {
            void AppendAndSelect(string s, Action<RichTextBox> action = null)
            {
                this.viewMessage.AppendText(s);
                this.viewMessage.Select(this.viewMessage.TextLength - s.Length, s.Length);
                if(action != null) {
                    var old = new {
                        SelectionColor = this.viewLicense.SelectionColor,
                        SelectionBackColor = this.viewLicense.SelectionBackColor,
                        SelectionAlignment = this.viewLicense.SelectionAlignment,
                        SelectionFont = this.viewLicense.SelectionFont,
                    };

                    action(this.viewMessage);

                    this.viewLicense.SelectionColor = old.SelectionColor;
                    this.viewLicense.SelectionBackColor = old.SelectionBackColor;
                    this.viewLicense.SelectionAlignment = old.SelectionAlignment;
                    this.viewLicense.SelectionFont = old.SelectionFont;
                }
            }
            void Break()
            {
                this.viewMessage.AppendText(Environment.NewLine);
            }

            Break();

            AppendAndSelect("りれき", r => {
                r.SelectionColor = Color.Red;
            });


            var version = Worker.GetAcceptVersion();
            foreach(var item in version.Items.OrderByDescending(i => i.Version)) {
                Break();

                var versionTitle = item.Version.ToString(4);
                AppendAndSelect(versionTitle, r => {
                    r.SelectionColor = Color.White;
                    r.SelectionBackColor = Color.Black;
                });
                Break();

                this.viewMessage.SelectionBullet = true;
                foreach(var value in item.Values) {
                    AppendAndSelect(value);
                    Break();
                }
                this.viewMessage.SelectionBullet = false;
            }

        }

        public void SetWorker(MainWorker worker)
        {
            Worker = worker;

            using(var acceptMessageStream = new MemoryStream(Properties.Resources.File_AcceptMessage)) {
                this.viewMessage.LoadFile(acceptMessageStream, RichTextBoxStreamType.RichText);

                if(!Worker.IsFirstExecute) {
                    AppendVersionMessage();
                }
            }

            var nkitLicensePath = Path.Combine(CommonUtility.GetDocumentDirectory().FullName, "license", "NKit.rtf");
            this.viewLicense.LoadFile(nkitLicensePath);

            RefreshReadState();
        }

        private void RefreshReadState()
        {
            this.commanAccept.Enabled = this.selectedRead.Checked;
        }

        #endregion

        private void selectedRead_CheckedChanged(object sender, EventArgs e)
        {
            RefreshReadState();
        }

        private void commanAccept_Click(object sender, EventArgs e)
        {
            if(!this.selectedRead.Checked) {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void commandCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
