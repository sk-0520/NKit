using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Manager.Model;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class ManagerForm : Form
    {
        public ManagerForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
        }

        #region property

        ManagerWorker Worker { get; set; }

        #endregion

        #region function

        public void SetWorker(ManagerWorker worker)
        {
            Worker = worker;
        }

        #endregion


    }
}
