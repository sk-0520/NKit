using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.Library.PInvoke.Windows;
using ContentTypeTextNet.NKit.Cameraman.Model;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Cameraman.View
{
    public partial class InformationForm : Form
    {
        public InformationForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
        }

        #region property

        CameramanModel Model {get;set;}
        public Point OffsetPoint { get; set; } = new Point(32, 32);
        public int WindowTextBufferLength { get; set; } = 256;
        public int WindowClassBufferLength { get; set; } = 256;

        CameramanForm CameramanForm { get; set; } = new CameramanForm();

        #endregion

        #region function

        public void SetModel(CameramanModel model)
        {
            Model = model;

            CameramanForm.SetModel(Model);
        }


        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {
                var nowOffset = Cursor.Position;
                nowOffset.Offset(OffsetPoint);
                Location = nowOffset;

                var textBuffer = new StringBuilder(WindowTextBufferLength);
                NativeMethods.GetWindowText(hWnd, textBuffer, textBuffer.Capacity);

                var classBuffer = new StringBuilder(WindowClassBufferLength);
                NativeMethods.GetClassName(hWnd, classBuffer, textBuffer.Capacity);

                this.labelCaption.Text = textBuffer.ToString();
                this.labelLocation.Text = hWndRectangle.Location.ToString();
                this.labelSize.Text = hWndRectangle.Size.ToString();
                this.labelClass.Text = classBuffer.ToString();
                this.labelHandle.Text = hWnd.ToString();

                Opacity = 1;
                Visible = true;
            }

            CameramanForm.Attach(hWnd, hWndRectangle);
        }

        public void Detach()
        {
            Visible = false;

            CameramanForm.Detach();
        }

        public bool IsSelfHandle(IntPtr hWnd)
        {
            return Handle == hWnd || CameramanForm.Handle == hWnd;
        }

        #endregion

        private void InformationForm_Shown(object sender, EventArgs e)
        {
            CameramanForm.Show();
        }
    }
}
