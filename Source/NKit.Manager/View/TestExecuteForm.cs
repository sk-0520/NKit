using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Model;
using ContentTypeTextNet.NKit.Manager.Model.Application;
using ContentTypeTextNet.NKit.Manager.Model.Workspace;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;
using static System.Windows.Forms.ListViewItem;

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class TestExecuteForm : Form
    {
        #region define

        enum TestState
        {
            [EnumResourceDisplay(nameof(Properties.Resources.String_TestExecute_TestState_None))]
            None,
            [EnumResourceDisplay(nameof(Properties.Resources.String_TestExecute_TestState_Testing))]
            Testing,
            [EnumResourceDisplay(nameof(Properties.Resources.String_TestExecute_TestState_Ok))]
            Ok,
            [EnumResourceDisplay(nameof(Properties.Resources.String_TestExecute_TestState_Fail))]
            Fail,
        }

        class ApplicationInfo
        {
            #region property

            public NKitApplicationKind Kind { get; set; }
            public FileInfo File { get; set; }
            public string Name { get; set; }

            public string Arguments { get; set; }

            public TestState State { get; set; }

            #endregion
        }

        struct ListViewItems
        {
            public ListViewItems(ListViewItem item, ListViewSubItem subItem)
            {
                Item = item;
                SubItem = subItem;
            }

            #region property

            public ListViewItem Item { get; }
            public ListViewSubItem SubItem { get; }

            #endregion
        }

        #endregion

        public TestExecuteForm()
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;
            Text = CommonUtility.ReplaceWindowTitle(string.Format(Properties.Resources.String_TestExecute_Title_Format, CommonUtility.ProjectName));
        }

        #region property

        ApplicationManager Manager { get; set; }
        public bool ForceExecute { get; set; }

        List<ApplicationInfo> ApplicationInfos { get; } = new List<ApplicationInfo>();


        AutoResetEvent ExitEvent { get; set; }


        #endregion

        #region function

        public void SetApplicationManager(ApplicationManager manager)
        {
            Manager = manager;

            // ?????????????????????
            var appDir = CommonUtility.GetApplicationDirectory();
            var binDir = CommonUtility.GetBinaryDirectory();

            var nkitDieArgs = $"{CommonUtility.ManagedStartup.ExecuteFlag} {CommonUtility.ManagedStartup.LetsDie}";

            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Main,
                Name = "main",
                Arguments = nkitDieArgs,
            });
            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Rocket,
                Name = "rocket",
                Arguments = nkitDieArgs,
            });
            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Cameraman,
                Name = "cameraman",
                Arguments = nkitDieArgs,
            });
            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.JustLooking,
                Name = "justlooking",
                Arguments = nkitDieArgs,
            });
            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Others,
                Name = "busybox",
                File = CommonUtility.GetBusyBox(false, binDir),
                Arguments = "--help",
            });
            if(Environment.Is64BitOperatingSystem) {
                ApplicationInfos.Add(new ApplicationInfo() {
                    Kind = NKitApplicationKind.Others,
                    Name = "busybox(64bit)",
                    File = CommonUtility.GetBusyBox(true, binDir),
                    Arguments = "--help",
                });
            }

            foreach(var info in ApplicationInfos) {
                var item = new ListViewItem() {
                    Name = info.Name,
                    Text = info.Kind.ToString(),
                    Tag = info,
                };
                item.SubItems.Add(new ListViewItem.ListViewSubItem() {
                    Text = info.Name,
                });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() {
                    Text = DisplayTextUtility.GetDisplayText(info.State),
                });

                this.listApplications.Items.Add(item);
            }

            foreach(ColumnHeader column in this.listApplications.Columns) {
                column.Width = -2;
            }
        }

        ListViewItems GetListViewItems(string key)
        {
            ListViewItem item = null;
            ListViewSubItem stateItem = null;
            var action = new Action(() => {
                item = this.listApplications.Items[key];
                stateItem = item.SubItems[this.columnState.Index];
            });
            if(InvokeRequired) {
                Invoke(action);
            } else {
                action();
            }

            return new ListViewItems(item, stateItem);
        }

        void SetState(ListViewSubItem stateItem, ApplicationInfo info, TestState testState)
        {
            info.State = testState;

            var action = new Action(() => {
                stateItem.Text = DisplayTextUtility.GetDisplayText(info.State);
            });
            if(InvokeRequired) {
                Invoke(action);
            } else {
                action();
            }

        }

        Task ExecuteAsync()
        {
            Manager.ApplicationExited -= Manager_ApplicationExited;
            Manager.ApplicationExited += Manager_ApplicationExited;
            ExitEvent = new AutoResetEvent(false);

            var dummyActiveWorkspace = new ActiveWorkspace() {
                ApplicationId = "????",
                GroupSuicideEventName = "????",
                LogFilePath = "????",
                LogWriter = null,
                ServiceUri = new Uri("http://127.0.0.1"),
            };
            var dummyWorkspaceSetting = new WorkspaceItemSetting() {
                CreatedUtcTimestamp = DateTime.MaxValue,
                DirectoryPath = "????",
                Name = "????",
                UpdatedUtcTimestamp = DateTime.MinValue,
            };

            return Task.Run(() => {
                var workDirPath = Path.GetTempPath();
                foreach(var info in ApplicationInfos) {
                    var listVuewItems = GetListViewItems(info.Name);
                    Debug.Assert(listVuewItems.Item.Tag == info);

                    SetState(listVuewItems.SubItem, info, TestState.Testing);

                    uint manageId = 0;
                    if(info.Kind == NKitApplicationKind.Others) {
                        manageId = Manager.PreparateOtherApplication(NKitApplicationKind.Manager, info.File.FullName, dummyActiveWorkspace, dummyWorkspaceSetting, info.Arguments, workDirPath);
                    } else {
                        manageId = Manager.PreparateNKitApplication(NKitApplicationKind.Manager, info.Kind, dummyActiveWorkspace, dummyWorkspaceSetting, info.Arguments, workDirPath);
                    }
                    Manager.WakeupNKitApplication(NKitApplicationKind.Manager, manageId);

                    if(ExitEvent.WaitOne(Constants.TestExecuteWait)) {
                        // ??????????????????
                        SetState(listVuewItems.SubItem, info, TestState.Ok);
                    } else {
                        // ??????????????????????????????????????????
                        SetState(listVuewItems.SubItem, info, TestState.Fail);
                    }
                }

                Manager.ApplicationExited -= Manager_ApplicationExited;
                ExitEvent.Dispose();
            });
        }

        void RefreshControls(bool isEnabled)
        {
            this.commandClose.Enabled = isEnabled;
            this.commandExecute.Enabled = isEnabled;
        }

        #endregion

        private async void commandExecute_Click(object sender, EventArgs e)
        {
            RefreshControls(false);
            await ExecuteAsync();
            RefreshControls(true);
        }

        private void Manager_ApplicationExited(object sender, EventArgs e)
        {
            ExitEvent.Set();
        }

        private void commandClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void TestExecuteForm_Shown(object sender, EventArgs e)
        {
            if(ForceExecute) {
                RefreshControls(false);
                await ExecuteAsync();
                RefreshControls(true);
                Close();
            }
        }
    }
}
