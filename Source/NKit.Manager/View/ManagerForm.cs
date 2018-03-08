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

        void SetInputControl(string name, string directoryPath)
        {
            this.inputWorkspaceName.Text = name;
            this.inputWorkspaceDirectoryPath.Text = directoryPath;
        }

        void SetInputControlFromSelectedWorkspaceItem()
        {
            if(Worker.SelectedWorkspaceItem != null) {
                SetInputControl(Worker.SelectedWorkspaceItem.Name, Worker.SelectedWorkspaceItem.DirectoryPath);
            } else {
                SetInputControl(string.Empty, string.Empty);
            }
        }

        void RefreshControls()
        {
            switch(Worker.WorkspaceState) {
                case Define.WorkspaceState.None:
                case Define.WorkspaceState.Creating:
                case Define.WorkspaceState.Copy:
                    this.commandWorkspaceLoad.Enabled = false;
                    this.commandWorkspaceClose.Enabled = false;
                    this.selectWorkspace.Enabled = false;
                    this.commandWorkspaceCopy.Enabled = false;
                    this.commandWorkspaceCreate.Enabled = false;
                    this.panelWorkspace.Enabled = true;
                    // 明示的に作成している場合は削除(キャンセル可能)
                    this.commandWorkspaceDelete.Enabled = Worker.WorkspaceState != Define.WorkspaceState.None;
                    break;

                case Define.WorkspaceState.Selecting:
                    this.commandWorkspaceLoad.Enabled = true;
                    this.commandWorkspaceClose.Enabled = false;
                    this.selectWorkspace.Enabled = true;
                    this.commandWorkspaceCopy.Enabled = true;
                    this.commandWorkspaceCreate.Enabled = true;
                    this.panelWorkspace.Enabled = true;
                    break;

                case Define.WorkspaceState.Running:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            Worker.ListupWorkspace(this.selectWorkspace);
            RefreshControls();
        }

        private void commandWorkspaceSave_Click(object sender, EventArgs e)
        {
            if(Worker.SaveWorkspace(this.errorProvider, this.inputWorkspaceName, this.inputWorkspaceDirectoryPath)) {
                Worker.ListupWorkspace(this.selectWorkspace);
                SetInputControlFromSelectedWorkspaceItem();
                RefreshControls();
            }
        }

        private void commandWorkspaceCreate_Click(object sender, EventArgs e)
        {
            Worker.ClearSelectedWorkspace();
            this.selectWorkspace.SelectedIndex = -1;
            SetInputControl(string.Empty, string.Empty);
            RefreshControls();
        }

        private void commandWorkspaceDelete_Click(object sender, EventArgs e)
        {
            Worker.DeleteSelectedWorkspace();
            Worker.ListupWorkspace(this.selectWorkspace);
            if(Worker.SelectedWorkspaceItem != null) {
                SetInputControlFromSelectedWorkspaceItem();
            } else {
                SetInputControl(string.Empty, string.Empty);
            }
            RefreshControls();
        }

        private void selectWorkspace_SelectedIndexChanged(object sender, EventArgs e)
        {
            Worker.ChangeSelectedItem(this.selectWorkspace);
            if(Worker.WorkspaceState == Define.WorkspaceState.Selecting) {
                SetInputControlFromSelectedWorkspaceItem();
            } else {
                SetInputControl(string.Empty, string.Empty);
            }
        }

        private void commandWorkspaceCopy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO: not impl");
            var __TODO__ = false;
            if(__TODO__) {
                Worker.CopySelectedWorkspace();
                SetInputControlFromSelectedWorkspaceItem();
                RefreshControls();
            }
        }
    }
}
