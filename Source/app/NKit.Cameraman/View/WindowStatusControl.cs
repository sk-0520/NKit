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
using System.Diagnostics;

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
            var sw = Stopwatch.StartNew();

            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout(true))) {
                Debug.WriteLine($"WS, 1 {sw.Elapsed}");
                var textBuffer = new StringBuilder(WindowTextBufferLength);
                NativeMethods.GetWindowText(hWnd, textBuffer, textBuffer.Capacity);

                Debug.WriteLine($"WS, 2 {sw.Elapsed}");
                var classBuffer = new StringBuilder(WindowClassBufferLength);
                NativeMethods.GetClassName(hWnd, classBuffer, textBuffer.Capacity);

                Debug.WriteLine($"WS, 3 {sw.Elapsed}");
                this.labelCaption.Text = textBuffer.ToString();
                Debug.WriteLine($"WS, 4 {sw.Elapsed}");
                this.labelLocation.Text = $"{hWndRectangle.Location.X}, {hWndRectangle.Location.Y}";
                Debug.WriteLine($"WS, 5 {sw.Elapsed}");
                this.labelSize.Text = $"{hWndRectangle.Size.Width} x {hWndRectangle.Size.Height}";
                Debug.WriteLine($"WS, 6 {sw.Elapsed}");
                this.labelClass.Text = classBuffer.ToString();
                Debug.WriteLine($"WS, 7 {sw.Elapsed}");
                this.labelHandle.Text = hWnd.ToString(IntPtr.Size == 4 ? "x08": "x016");
                Debug.WriteLine($"WS, 8 {sw.Elapsed}");
            }
            Debug.WriteLine($"WS, X {sw.Elapsed}");

        }

        #endregion
    }
}
