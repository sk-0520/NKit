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
    public partial class FrameForm : Form
    {
        public FrameForm()
            :this(AnchorStyles.None)
        { }

        public FrameForm(AnchorStyles position)
        {
            Position = position;
            InitializeComponent();
        }

        #region property

        public AnchorStyles Position { get; }
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

        Rectangle GetAreaFromBaseArea(Rectangle baseArea)
        {
            // 左右の縦線を伸ばす感じ
            switch(Position) {
                case AnchorStyles.Left:
                    return new Rectangle(
                        baseArea.Left - Model.Bag.BorderWidth,
                        baseArea.Top - Model.Bag.BorderWidth,
                        Model.Bag.BorderWidth,
                        baseArea.Height + (Model.Bag.BorderWidth * 2)
                    );

                case AnchorStyles.Right:
                    return new Rectangle(
                        baseArea.Right,
                        baseArea.Top - Model.Bag.BorderWidth,
                        Model.Bag.BorderWidth,
                        baseArea.Height + (Model.Bag.BorderWidth * 2)
                    );

                case AnchorStyles.Top:
                    return new Rectangle(
                        baseArea.Left,
                        baseArea.Top - Model.Bag.BorderWidth,
                        baseArea.Width,
                        Model.Bag.BorderWidth
                    );

                case AnchorStyles.Bottom:
                    return new Rectangle(
                        baseArea.Left,
                        baseArea.Bottom,
                        baseArea.Width,
                        Model.Bag.BorderWidth
                    );

                default:
                    throw new NotImplementedException();
            }
        }

        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            var newArea = GetAreaFromBaseArea(hWndRectangle);


            Opacity = 0;

            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {

                Location = newArea.Location;
                Size = newArea.Size;


                Visible = true;
                Opacity = 1;
            }
        }

        #endregion
    }
}
