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
    public partial class FrameForm : Form
    {
        public FrameForm()
            :this(AnchorStyles.None)
        { }

        public FrameForm(AnchorStyles postion)
        {
            Postion = postion;
            InitializeComponent();
        }

        #region property

        public AnchorStyles Postion { get; }
        CameramanModel Model { get; set; }

        #endregion

        #region function
        public void SetModel(CameramanModel model)
        {
            Model = model;

            BackColor = Model.Bag.BorderColor;
        }


        public void Detach()
        {
            Visible = false;
        }

        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            Visible = true;
            Opacity = 1;
        }

        #endregion
    }
}
