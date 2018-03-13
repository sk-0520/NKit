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

namespace ContentTypeTextNet.NKit.Manager.View
{
    public partial class TestExecuteForm : Form
    {
        #region define

        enum TestState
        {
            None,
            Testing,
            Ok,
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

        #endregion

        public TestExecuteForm()
        {
            InitializeComponent();
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

            // プログラム列挙
            var appDir = CommonUtility.GetApplicationDirectory();
            var binDir = CommonUtility.GetBinaryDirectory();

            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Main,
                Name = "main",
                Arguments = "--nkit_lets_die",
            });
            ApplicationInfos.Add(new ApplicationInfo() {
                Kind = NKitApplicationKind.Rocket,
                Name = "rocket",
                Arguments = "--nkit_lets_die",
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
                    Text = info.State.ToString(),
                });

                this.listApplications.Items.Add(item);
            }
        }

        Task ExecuteAsync()
        {
            Manager.ApplicationExited -= Manager_ApplicationExited;
            Manager.ApplicationExited += Manager_ApplicationExited;
            ExitEvent = new AutoResetEvent(false);

            var dummyActiveWorkspace = new ActiveWorkspace() {
                ApplicationId = "🐁",
                ExitEventName = "🐂",
                LogFilePath = "🐅",
                LogWriter = null,
                ServiceUri = new Uri("http://127.0.0.1"),
            };
            var dummyWorkspaceSetting = new WorkspaceItemSetting() {
                CreatedTimestamp = DateTime.MaxValue,
                DirectoryPath = "🐇",
                Name = "🐉",
                UpdatedTimestamp = DateTime.MinValue,
            };
            var workDirPath = Path.GetTempPath();
            foreach(var info in ApplicationInfos) {
                var listViewItem = this.listApplications.Items[info.Name];
                Debug.Assert(listViewItem.Tag == info);

                info.State = TestState.Testing;

                listViewItem.SubItems[this.columnState.Index].Text = info.State.ToString();

                if(info.Kind == NKitApplicationKind.Others) {
                    Manager.ExecuteOtherApplication(NKitApplicationKind.Manager, info.File.FullName, dummyActiveWorkspace, dummyWorkspaceSetting, info.Arguments, workDirPath);
                } else {
                    Manager.ExecuteNKitApplication(NKitApplicationKind.Manager, info.Kind, dummyActiveWorkspace, dummyWorkspaceSetting, info.Arguments, workDirPath);
                }

                ExitEvent.WaitOne();
                // 死んだ！
                info.State = TestState.Ok;
                listViewItem.SubItems[this.columnState.Index].Text = info.State.ToString();
            }

            Manager.ApplicationExited -= Manager_ApplicationExited;
            ExitEvent.Dispose();

            return Task.CompletedTask;
        }

        #endregion

        private void commandExecute_Click(object sender, EventArgs e)
        {
            ExecuteAsync();
        }

        private void Manager_ApplicationExited(object sender, EventArgs e)
        {
            ExitEvent.Set();
        }


    }
}
