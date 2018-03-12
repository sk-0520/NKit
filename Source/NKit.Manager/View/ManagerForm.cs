using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Manager.Model;
using ContentTypeTextNet.NKit.Manager.Model.Log;

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
            Worker.WorkspaceExited += Worker_WorkspaceExited;
            Worker.OutputLog += Worker_OutputLog;
        }

        void SetInputControl(string name, string directoryPath, bool logging)
        {
            this.inputWorkspaceName.Text = name;
            this.inputWorkspaceDirectoryPath.Text = directoryPath;
            this.selectLogging.Checked = logging;
        }

        void SetInputControlFromSelectedWorkspaceItem()
        {
            if(Worker.SelectedWorkspaceItem != null) {
                SetInputControl(Worker.SelectedWorkspaceItem.Name, Worker.SelectedWorkspaceItem.DirectoryPath, Worker.SelectedWorkspaceItem.Logging);
            } else {
                SetInputControl(string.Empty, string.Empty, false);
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
                    //this.panelWorkspace.Enabled = true;
                    this.inputWorkspaceName.ReadOnly = false;
                    this.inputWorkspaceDirectoryPath.ReadOnly = false;
                    // 明示的に作成している場合は削除(キャンセル可能)
                    this.commandWorkspaceDelete.Enabled = Worker.WorkspaceState != Define.WorkspaceState.None;
                    this.commandWorkspaceDirectorySelect.Enabled = true;
                    this.commandWorkspaceSave.Enabled = true;
                    this.selectLogging.Enabled = true;
                    break;

                case Define.WorkspaceState.Selecting:
                    this.commandWorkspaceLoad.Enabled = true;
                    this.commandWorkspaceClose.Enabled = false;
                    this.selectWorkspace.Enabled = true;
                    this.commandWorkspaceCopy.Enabled = true;
                    this.commandWorkspaceCreate.Enabled = true;
                    //this.panelWorkspace.Enabled = true;
                    this.inputWorkspaceName.ReadOnly = false;
                    this.inputWorkspaceDirectoryPath.ReadOnly = false;
                    this.commandWorkspaceDelete.Enabled = true;
                    this.commandWorkspaceDirectorySelect.Enabled = true;
                    this.commandWorkspaceSave.Enabled = true;
                    this.selectLogging.Enabled = true;
                    break;

                case Define.WorkspaceState.Running:
                    this.commandWorkspaceLoad.Enabled = false;
                    this.commandWorkspaceClose.Enabled = true;
                    this.selectWorkspace.Enabled = false;
                    this.commandWorkspaceCopy.Enabled = false;
                    this.commandWorkspaceCreate.Enabled = false;
                    //this.panelWorkspace.Enabled = true;
                    this.inputWorkspaceName.ReadOnly = true;
                    this.inputWorkspaceDirectoryPath.ReadOnly = true;
                    this.commandWorkspaceDelete.Enabled = false;
                    this.commandWorkspaceDirectorySelect.Enabled = false;
                    this.commandWorkspaceSave.Enabled = false;
                    this.selectLogging.Enabled = false;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            Worker.ListupWorkspace(this.selectWorkspace, Guid.Empty);
            RefreshControls();
        }

        private void commandWorkspaceSave_Click(object sender, EventArgs e)
        {
            if(Worker.SaveWorkspace(this.errorProvider, this.inputWorkspaceName, this.inputWorkspaceDirectoryPath, this.selectLogging)) {
                Worker.ListupWorkspace(this.selectWorkspace, Worker.SelectedWorkspaceItem.Id);
                SetInputControlFromSelectedWorkspaceItem();
                RefreshControls();
            }
        }

        private void commandWorkspaceCreate_Click(object sender, EventArgs e)
        {
            Worker.ClearSelectedWorkspace();
            this.selectWorkspace.SelectedIndex = -1;
            SetInputControl(string.Empty, string.Empty, false);
            RefreshControls();
        }

        private void commandWorkspaceDelete_Click(object sender, EventArgs e)
        {
            Worker.DeleteSelectedWorkspace();
            Worker.ListupWorkspace(this.selectWorkspace, Guid.Empty);
            if(Worker.SelectedWorkspaceItem != null) {
                SetInputControlFromSelectedWorkspaceItem();
            } else {
                SetInputControl(string.Empty, string.Empty, false);
            }
            RefreshControls();
        }

        private void selectWorkspace_SelectedIndexChanged(object sender, EventArgs e)
        {
            Worker.ChangeSelectedItem(this.selectWorkspace);
            if(Worker.WorkspaceState == Define.WorkspaceState.Selecting) {
                SetInputControlFromSelectedWorkspaceItem();
            } else {
                SetInputControl(string.Empty, string.Empty, false);
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

        private void commandWorkspaceLoad_Click(object sender, EventArgs e)
        {
            Worker.LoadSelectedWorkspace();
            RefreshControls();
        }

        private void Worker_WorkspaceExited(object sender, EventArgs e)
        {
            Invoke(new Action(() => {
                RefreshControls();
            }));
        }
        private void Worker_OutputLog(object sender, LogEventArgs e)
        {
            if(!this.viewLog.IsDisposed) {
                var write = new Action(() => {
                    this.viewLog.Focus();
                    this.viewLog.AppendText(e.WriteValue);
                    this.viewLog.AppendText(Environment.NewLine);
                });

                if(!this.viewLog.Created) {
                    write();
                } else {
                    if(InvokeRequired) {
                        this.viewLog.BeginInvoke(write);
                    } else {
                        write();
                    }
                }
            }
        }

        private void commandWorkspaceClose_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO: not impl");
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !Worker.CheckCanExit();
        }

        private void commandCheckUpdate_Click(object sender, EventArgs e)
        {
            Worker.CheckUpdateAsync();
        }
    }
}
