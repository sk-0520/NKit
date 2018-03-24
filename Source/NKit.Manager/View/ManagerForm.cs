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
            Text = CommonUtility.ReplaceWindowTitle(Text);

            this.notifyIcon.Icon = Icon;

            #region DEBUG
#if DEBUG || BETA
            AllowDrop = true;
            DragEnter += ReleaseNoteForm_DragEnterAndDragOver;
            DragOver += ReleaseNoteForm_DragEnterAndDragOver;
            DragDrop += ReleaseNoteForm_DragDrop;
#endif
            #endregion
        }

        #region property

        MainWorker Worker { get; set; }
        ReleaseNoteForm ReleaseNoteForm { get; set; }
        AboutForm AboutForm { get; set; }

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
                    this.commandShowAbout.Enabled = true;
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
                    this.commandShowAbout.Enabled = true;
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
                    this.commandShowAbout.Enabled = true;
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
                    this.commandShowAbout.Enabled = false;
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
                ReleaseNoteForm.FormClosed += ReleaseNoteForm_FormClosed;
                ReleaseNoteForm.ExecuteUpdateAction = this.commandExecuteUpdate.PerformClick;
            }

            ReleaseNoteForm.ReleaseNoteUri = Worker.ReleaseNoteUri;
            ReleaseNoteForm.SetUpdatable(Worker.WorkspaceState != WorkspaceState.Updating);
            ReleaseNoteForm.SetReleaseNote(Worker.ReleaseNote);
            ReleaseNoteForm.Show(this);
        }

        void ShowAbout()
        {
            if(AboutForm == null) {
                AboutForm = new AboutForm();
                AboutForm.FormClosed += AboutForm_FormClosed;
            }

            AboutForm.Show(this);
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
            this.selectWorkspaceLoadToMinimize.Checked = Worker.WorkspaceLoadToMinimize;

            RefreshControls();

#if DEBUG
            //this.commandTestExecute.PerformClick();
            //Close();
#endif
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
            if(Worker.IsLockedSelectedWorkspace()) {
                if((ModifierKeys & Keys.Shift) == Keys.Shift) {
                    var result = MessageBox.Show("workspace is locked, unlock?", "force", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if(result != DialogResult.Yes) {
                        return;
                    }
                    if(!Worker.RemoveSelectedWorkspaceLockFile()) {
                        return;
                    }
                } else {
                    MessageBox.Show("workspace is locked");
                    return;
                }
            }

            Worker.LoadSelectedWorkspace();
            RefreshControls();
            if(Worker.WorkspaceLoadToMinimize && Worker.WorkspaceState == WorkspaceState.Running) {
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
                TopMost = false;
                TopMost = true;
                TopMost = false;
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
            Worker.CloseWorkspace();
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !Worker.CheckCanExit();
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

            if(AboutForm != null) {
                AboutForm.Close();
            }

            if(ReleaseNoteForm != null) {
                ReleaseNoteForm.SetUpdatable(false);
            }

            var task = Worker.ExecuteUpdateAsync(execEvent);

            // 向こうから非同期処理完了前に制御が帰ってきたらコントロール系を更新
            execEvent.WaitOne();
            RefreshControls();

            var updated = await task;

            if(ReleaseNoteForm != null) {
                ReleaseNoteForm.SetUpdatable(true);
            }

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
            ReleaseNoteForm.ExecuteUpdateAction = null;
            ReleaseNoteForm.Dispose();
            ReleaseNoteForm = null;
        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AboutForm.FormClosed -= AboutForm_FormClosed;
            AboutForm.Dispose();
            AboutForm = null;
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

            // ワークスペースを開くように言われてるんなら開く
            var commandLine = new CommandLine();
            if(commandLine.HasOption("load_workspace")) {
                var logger = Worker.CreateLogger("LOAD");
                try {
                    //NOTE: ほんとは指定ワークスペースにしたかったけど諸々の事情により見送り、将来的に必要であればオプション指定で開けるようにする
                    if(Worker.SelectedWorkspaceItem != null) {
                        logger.Information("load last workspace");
                        this.commandWorkspaceLoad.PerformClick();
                    } else {
                        logger.Error("last workspace: empty");
                    }
                } finally {
                    if(logger is IDisposable diposer) {
                        diposer.Dispose();
                    }
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
                    this.notifyIcon.Visible = Worker.WorkspaceRunningMinimizeToNotifyArea;
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

        private void selectWorkspaceLoadToMinimize_CheckedChanged(object sender, EventArgs e)
        {
            Worker.WorkspaceLoadToMinimize = this.selectWorkspaceLoadToMinimize.Checked;
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

        private void selectWorkspaceRunningMinimizeToNotifyArea_CheckedChanged(object sender, EventArgs e)
        {
            Worker.WorkspaceRunningMinimizeToNotifyArea = this.selectWorkspaceRunningMinimizeToNotifyArea.Checked;
        }

        private void commandShowAbout_Click(object sender, EventArgs e)
        {
            if(AboutForm == null) {
                ShowAbout();
            } else if(AboutForm.Visible) {
                AboutForm.Activate();
            }
        }

        #region DEBUG
#if DEBUG || BETA
        private void ReleaseNoteForm_DragEnterAndDragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(files.Length == 1) {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void ReleaseNoteForm_DragDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(files.Length == 1) {
                    var file = files[0];

                    // デバッグコードなので Dispose は考えない
                    var form = new ReleaseNoteForm();
                    form.SetReleaseNote(new ReleaseNoteItem() {
                        Version = Assembly.GetExecutingAssembly().GetName().Version,
                        Hash = Application.ProductVersion,
                        // これは渡された時からローカルタイム
                        Timestamp = DateTime.Now,
                        Content = File.ReadAllText(file),
                    });
                    form.Show(this);
                }
            }
        }
#endif
        #endregion

    }
}
