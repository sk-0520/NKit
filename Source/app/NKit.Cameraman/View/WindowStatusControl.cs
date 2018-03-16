using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.Library.PInvoke.Windows;

namespace ContentTypeTextNet.NKit.Cameraman.View
{
    public partial class WindowStatusControl : UserControl
    {
        public WindowStatusControl()
        {
            InitializeComponent();
        }

        #region property

        public int WindowTextBufferLength { get; set; } = 256;
        public int WindowClassBufferLength { get; set; } = 256;

        #endregion

        #region function

        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {
                var textBuffer = new StringBuilder(WindowTextBufferLength);
                NativeMethods.GetWindowText(hWnd, textBuffer, textBuffer.Capacity);

                var classBuffer = new StringBuilder(WindowClassBufferLength);
                NativeMethods.GetClassName(hWnd, classBuffer, textBuffer.Capacity);

                this.labelCaption.Text = textBuffer.ToString();
                this.labelLocation.Text = hWndRectangle.Location.ToString();
                this.labelSize.Text = hWndRectangle.Size.ToString();
                this.labelClass.Text = classBuffer.ToString();
                this.labelHandle.Text = hWnd.ToString();
            }

        }

        #endregion
    }
}
