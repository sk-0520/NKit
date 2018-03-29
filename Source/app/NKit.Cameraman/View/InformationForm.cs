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
            Text = CommonUtility.ReplaceWindowTitle(Text);

            var positions = new[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Bottom };
            FrameForms = new FrameForm[positions.Length];
            for(var i = 0; i < FrameForms.Length; i++) {
                FrameForms[i] = new FrameForm(positions[i]);
            }

            if(!DesignMode) {
                FrameForm prevForm = null;
                foreach(var frameForm in FrameForms) {
                    if(prevForm != null) {
                        frameForm.Show(prevForm);
                    } else {
                        frameForm.Show();
                    }
                    prevForm = frameForm;
                }
                prevForm.AddOwnedForm(this);
            }
        }

        #region property

        CameramanModel Model { get; set; }
        public Point OffsetPoint { get; set; } = new Point(32, 32);

        public double WindowStateOpacity { get; set; } = 1;
        public double NavigationMouseInOpacity { get; set; } = 0.9;
        public double NavigationMouseOutOpacity { get; set; } = 0.5;


        FrameForm[] FrameForms { get; }

        #endregion

        #region function

        public void SetModel(CameramanModel model)
        {
            Model = model;

            DoFrameForms(f => f.SetModel(Model));

            this.navigationControl.SetModel(Model);
        }

        void DoFrameForms(Action<FrameForm> action)
        {
            foreach(var frameForm in FrameForms) {
                action(frameForm);
            }
        }

        public void Attach(IntPtr hWnd, Rectangle hWndRectangle)
        {
            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {
                Visible = true;
                Opacity = WindowStateOpacity;

                var nowOffset = Cursor.Position;
                nowOffset.Offset(OffsetPoint);
                Location = nowOffset;

                this.windowStatusControl.Attach(hWnd, hWndRectangle);

                this.navigationControl.Visible = false;
                this.windowStatusControl.Visible = true;
            }

            DoFrameForms(f => f.Attach(hWnd, hWndRectangle));
        }

        public void Detach()
        {
            DoFrameForms(f => f.Detach());

            this.windowStatusControl.Visible = false;
            this.navigationControl.Visible = false;

            Visible = false;
        }

        public void ShowNavigation()
        {
            SuspendLayout();
            using(new ActionDisposer(d => ResumeLayout())) {
                this.windowStatusControl.Visible = false;
                this.navigationControl.Visible = true;
            }

            // デスクトップのどっかに配置させる
            //ArrangementNavigation(Cursor.Position);
            var margin = new Size(
                NativeMethods.GetSystemMetrics(SM.SM_CXSMSIZE),
                NativeMethods.GetSystemMetrics(SM.SM_CYSMSIZE)
            );
            var area = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(
                area.Right - Size.Width - margin.Width,
                area.Bottom - Size.Height - margin.Height
            );
            SetNavigationOpacity(ContainsCursorInWindow());
            Visible = true;
        }

        void SetNavigationOpacity(bool isMouseIn)
        {
            Opacity = isMouseIn
                ? NavigationMouseInOpacity
                : NavigationMouseOutOpacity
            ;
        }

        bool ContainsCursorInWindow()
        {
            var windowArea = RectangleToScreen(new Rectangle(new Point(), Size));
            return windowArea.Contains(Cursor.Position);
        }


        [Obsolete]
        void ArrangementNavigation(Point cursorPosition)
        {
            var area = Screen.PrimaryScreen.WorkingArea;
            // プライマリスクリーンに配置するけど一応それ以外のディスプレイにも配置可能な想定にしておく。
            // ちょっとくらい余白入れてもいいかも

            if((area.X + area.Width) / 2 < cursorPosition.X) {
                // 左側
                Location = new Point(
                    area.X,
                    area.Bottom - Size.Height
                );
            } else {
                // 右側
                Location = new Point(
                    area.Right - Size.Width,
                    area.Bottom - Size.Height
                );
            }
        }

        public bool IsSelfHandle(IntPtr hWnd)
        {
            return Handle == hWnd;
        }

        #endregion

        private void InformationForm_Shown(object sender, EventArgs e)
        {
        }

        private void InformationForm_MouseEnter(object sender, EventArgs e)
        {
            SetNavigationOpacity(true);
        }

        private void InformationForm_MouseLeave(object sender, EventArgs e)
        {
            SetNavigationOpacity(ContainsCursorInWindow());
        }
    }
}
