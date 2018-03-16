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
        }

        #region property

        CameramanModel Model { get; set; }

        #endregion

        #region function
        public void SetModel(CameramanModel model)
        {
            Model = model;

            this.labelSelectKey.Text = model.SelectKeys.ToString();
            this.labelTakeShotKey.Text = model.ShotKeys.ToString();
            this.labelExitKey.Text = model.ExitKey.ToString();
            this.labelContinuation.Text = model.IsContinuation.ToString();
        }
        #endregion
    }
}
