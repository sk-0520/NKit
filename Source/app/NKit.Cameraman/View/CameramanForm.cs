using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Cameraman.Model;

namespace ContentTypeTextNet.NKit.Cameraman.View
{
    public partial class CameramanForm : Form
    {
        public CameramanForm()
        {
            InitializeComponent();
        }

        #region property

        CameramanModel Model { get; set; }

        InformationForm InformationForm { get; set; }

        #endregion

        #region function

        public void SetModel(CameramanModel model)
        {
            Model = model;

            Padding = new Padding(Model.BorderWidth);
            BackColor = Model.BorderColor;
            if(BackColor == TransparencyKey) {
                var colors = new[] {
                    Color.Black,
                    Color.Wheat,
                };
                this.hallArea.BackColor = TransparencyKey = colors.First(c => c != TransparencyKey);
            }
        }

        public void HideStatus()
        {
            Visible = false;

            InformationForm.Detach();
        }

        public void ShowStatus(IntPtr hWnd, Rectangle hWndRectangle)
        {
            Location = new Point(
                hWndRectangle.X - Padding.Left,
                hWndRectangle.Y - Padding.Top
            );
            Size = new Size(
                hWndRectangle.Width + Padding.Horizontal,
                hWndRectangle.Height + Padding.Vertical
            );

            InformationForm.Attach(hWnd, hWndRectangle);

            Visible = true;
            Opacity = 1;
        }

        #endregion

        private void CameramanForm_Shown(object sender, EventArgs e)
        {
            InformationForm = new InformationForm();
            InformationForm.Show(this);
        }
    }
}
