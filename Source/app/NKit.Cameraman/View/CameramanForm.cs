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
using ContentTypeTextNet.NKit.Common;

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

        public void Detach()
        {
            Visible = false;
        }

        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            var newLocation = new Point(
                hWndRectangle.X - Padding.Left,
                hWndRectangle.Y - Padding.Top
            );
            var newSize = new Size(
                hWndRectangle.Width + Padding.Horizontal,
                hWndRectangle.Height + Padding.Vertical
            );

            if(Visible && newLocation == Location && newSize == Size) {
                return;
            }

            Opacity = 0;
            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {

                Location = new Point(
                    hWndRectangle.X - Padding.Left,
                    hWndRectangle.Y - Padding.Top
                );
                Size = new Size(
                    hWndRectangle.Width + Padding.Horizontal,
                    hWndRectangle.Height + Padding.Vertical
                );


                Visible = true;
                Opacity = 1;
            }
        }

        #endregion
    }
}
