using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Cameraman.Model;

namespace ContentTypeTextNet.NKit.Cameraman.View
{
    public partial class NavigationControl : UserControl
    {
        public NavigationControl()
        {
            InitializeComponent();

            foreach(var control in GetChildrenControls(this, true)) {
                control.MouseEnter += controls_MouseEnter;
                control.MouseLeave += controls_MouseLeave;
            }
        }

        #region property

        CameramanModel Model { get; set; }

        #endregion

        #region function

        static IEnumerable<Control> GetChildrenControls(Control target, bool recursive)
        {
            foreach(Control control in target.Controls) {
                yield return control;
                if(recursive) {
                    foreach(var child in  GetChildrenControls(control, recursive)) {
                        yield return child;
                    }
                }
            }
        }

        public void SetModel(CameramanModel model)
        {
            Model = model;

            this.linkSelectKey.Text = model.Bag.SelectKeys.ToString();
            this.labelTakeShotKey.Text = model.Bag.ShotKeys.ToString();
            this.linkExitKey.Text = model.Bag.ExitKey.ToString();
            this.labelContinuation.Text = model.Bag.IsContinuation.ToString();
        }
        #endregion

        private void controls_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void controls_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void linkExitKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Model.Exit();
        }

        private void linkSelectKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(!Model.NowSelecting) {
                Model.StartSelectView();
            }
        }
    }
}
