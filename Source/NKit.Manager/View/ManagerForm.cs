using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Define;
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
            this.notifyIcon.Icon = Icon;
        }

        #region property

        MainWorker Worker { get; set; }
        ReleaseNoteForm ReleaseNoteForm { get; set; }

        #endregion

        #region function

        public void SetWorker(MainWorker worker)
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
                    this.commandExecuteUpdate.Enabled = Worker.CanUpdate;
                    this.commandShowReleaseNote.Enabled = Worker.CanUpdate;
                    this.commandTestExecute.Enabled = true;
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
                    this.commandExecuteUpdate.Enabled = Worker.CanUpdate;
                    this.commandShowReleaseNote.Enabled = Worker.CanUpdate;
                    this.commandTestExecute.Enabled = true;
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
                    this.commandExecuteUpdate.Enabled = false;
                    this.commandShowReleaseNote.Enabled = Worker.CanUpdate;
                    this.commandTestExecute.Enabled = false;
                    break;

                case Define.WorkspaceState.Updating:
                    this.commandWorkspaceLoad.Enabled = false;
                    this.commandWorkspaceClose.Enabled = false;
                    this.selectWorkspace.Enabled = false;
                    this.commandWorkspaceCopy.Enabled = false;
                    this.commandWorkspaceCreate.Enabled = false;
                    //this.panelWorkspace.Enabled = true;
                    this.inputWorkspaceName.ReadOnly = false;
                    this.inputWorkspaceDirectoryPath.ReadOnly = false;
                    this.inputWorkspaceName.Enabled = false;
                    this.inputWorkspaceDirectoryPath.Enabled = false;
                    this.commandWorkspaceDelete.Enabled = false;
                    this.commandWorkspaceDirectorySelect.Enabled = false;
                    this.commandWorkspaceSave.Enabled = false;
                    this.selectLogging.Enabled = false;
                    this.commandCheckUpdate.Enabled = false;
                    this.commandExecuteUpdate.Enabled = false;
                    this.commandShowReleaseNote.Enabled = Worker.CanUpdate;
                    this.commandTestExecute.Enabled = false;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void ShowReleaseNote()
        {
            Debug.Assert(Worker.CanUpdate);

            if(ReleaseNoteForm == null) {
                ReleaseNoteForm = new ReleaseNoteForm();
                ReleaseNoteForm.IssueBaseUri = Constants.IssuesBaseUri;
                ReleaseNoteForm.FormClosed += ReleaseNoteForm_FormClosed;
            }

            ReleaseNoteForm.ReleaseNoteUri = Worker.ReleaseNoteUri;
            ReleaseNoteForm.SetReleaseNote(Worker.NewVersion, Worker.ReleaseHash, Worker.ReleaseTimestamp, Worker.ReleaseNoteValue);
            ReleaseNoteForm.Show(this);

        }

        #endregion

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            this.labelBuildType.Text = CommonUtility.BuildType;
            this.labelVersionNumber.Text = assembly.GetName().Version.ToString();
            this.labelVersionHash.Text = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;


            // 設定値をどばーっと反映
            Worker.ListupWorkspace(this.selectWorkspace, Guid.Empty);
            this.selectWorkspaceLoadToHide.Checked = Worker.WorkspaceLoadToHide;


            RefreshControls();
            /*
            var r = new ReleaseNoteForm();
            r.IssueBaseUri = Constants.IssuesBaseUri;
            r.ReleaseNoteUri = new Uri("http://localhost/");
            r.SetReleaseNote(new Version("1.2.3.4"), new string('x', 40), DateTime.Now, @"
* a
   * b
        *c
      *d
        *e

123456846
258146586
");
            r.Show();
            */
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
            if(Worker.WorkspaceLoadToHide && Worker.WorkspaceState == WorkspaceState.Running) {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void Worker_WorkspaceExited(object sender, EventArgs e)
        {
            Invoke(new Action(() => {
                RefreshControls();

                // マネージャウィンドウの復帰
                if(WindowState == FormWindowState.Minimized) {
                    WindowState = FormWindowState.Normal;
                }
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
            if(e.Cancel) {
                if(Worker.WorkspaceState == Define.WorkspaceState.Running) {
                    WindowState = FormWindowState.Minimized;
                }
            }
        }

        private async void commandCheckUpdate_Click(object sender, EventArgs e)
        {
            this.commandExecuteUpdate.Enabled = false;
            await Worker.CheckUpdateAsync(this.commandCheckUpdate);
            RefreshControls();
            if(Worker.CanUpdate) {
                ShowReleaseNote();
            }
        }

        private async void commandExecuteUpdate_Click(object sender, EventArgs e)
        {
            var execEvent = new AutoResetEvent(false);

            var task = Worker.ExecuteUpdateAsync(execEvent);

            // 向こうから非同期処理完了前に制御が帰ってきたらコントロール系を更新
            execEvent.WaitOne();
            RefreshControls();

            var updated = await task;
            if(!updated) {
                // あかなんだ
                RefreshControls();
                return;
            }

            if(ReleaseNoteForm != null) {
                ReleaseNoteForm.Close();
            }

            //TODO: 再起動
            Application.Restart();
        }

        private void commandShowReleaseNote_Click(object sender, EventArgs e)
        {
            if(ReleaseNoteForm == null) {
                ShowReleaseNote();
            } else if(ReleaseNoteForm.Visible) {
                ReleaseNoteForm.Activate();
            }
        }

        private void ReleaseNoteForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReleaseNoteForm.FormClosed -= ReleaseNoteForm_FormClosed;
            ReleaseNoteForm.Dispose();
            ReleaseNoteForm = null;
        }

        private void commandTestExecute_Click(object sender, EventArgs e)
        {
            using(var form = new TestExecuteForm()) {
                Worker.ExecuteTest(form, false);
            }
        }

        private void ManagerForm_Shown(object sender, EventArgs e)
        {
            // バージョンアップ時とかなら強制で全アプリケーションを起動
            if(Worker.IsFirstExecute || Worker.IsUpdatedFirstExecute) {
#if DEBUG
                if(Worker.IsUpdatedFirstExecute) {
                    // デバッグ中にばっしばし動くのはちょっと勘弁
                    return;
                }
#endif
                using(var form = new TestExecuteForm()) {
                    Worker.ExecuteTest(form, true);
                }
            }
        }

        private void ManagerForm_SizeChanged(object sender, EventArgs e)
        {
            if(Worker == null) {
                return;
            }

            if(Worker.WorkspaceState == WorkspaceState.Running) {
                if(WindowState == FormWindowState.Minimized) {
                    this.notifyIcon.Visible = true;
                } else {
                    this.notifyIcon.Visible = false;
                }
            } else {
                this.notifyIcon.Visible = false;
            }

            ShowInTaskbar = !this.notifyIcon.Visible;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void selectWorkspaceLoadToHide_CheckedChanged(object sender, EventArgs e)
        {
            Worker.WorkspaceLoadToHide = this.selectWorkspaceLoadToHide.Checked;
        }

        private void commandWorkspaceDirectorySelect_Click(object sender, EventArgs e)
        {
            var dirPath = this.inputWorkspaceDirectoryPath.Text;
            if(!string.IsNullOrWhiteSpace(dirPath)) {
                dirPath = Environment.ExpandEnvironmentVariables(dirPath);
            }
            if(string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath)) {
                dirPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }

            using(var dialog = new FolderBrowserDialog()) {
                dialog.SelectedPath = dirPath;
                dialog.ShowNewFolderButton = true;
                if(dialog.ShowDialog() == DialogResult.OK) {
                    this.inputWorkspaceDirectoryPath.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
