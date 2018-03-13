using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Define;
using ContentTypeTextNet.NKit.Manager.Model.Application;
using ContentTypeTextNet.NKit.Manager.Model.Log;
using ContentTypeTextNet.NKit.Manager.Model.Update;
using ContentTypeTextNet.NKit.Manager.Model.Workspace;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;
using ContentTypeTextNet.NKit.Manager.View;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class ManagerWorker : DisposerBase
    {
        #region define

        [DllImport("shell32.dll", SetLastError = true)]
        static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        #endregion

        #region event

        public event EventHandler<EventArgs> WorkspaceExited;
        public event EventHandler<LogEventArgs> OutputLog;

        #endregion

        public ManagerWorker()
        {
            LogManager = new LogManager();
            LogManager.LogWrite += LogManager_LogWrite;

            ApplicationManager = new ApplicationManager(LogManager);
            ApplicationManager.MainApplicationExited += ApplicationManager_MainApplicationExited;

            UpdateManager = new UpdateManager(LogManager);
        }


        #region property

        LogManager LogManager { get; }
        ILogger Logger { get; set; }

        ApplicationManager ApplicationManager { get; }
        UpdateManager UpdateManager { get; }

        NKitApplicationTalkerHost NKitApplicationTalkerHost { get; set; }
        NKitLoggingTalkerHost NKitLoggingTalkerHost { get; set; }

        public bool IsFirstExecute { get; private set; }
        public bool IsUpdatedFirstExecute { get; private set; }
        public bool Accepted
        {
            get { return ManagerSetting.Accepted; }
            set { ManagerSetting.Accepted = value; }
        }

        public bool CanUpdate => UpdateManager.HasUpdate;
        public Version NewVersion => UpdateManager.NewVersion;
        public string ReleaseNoteValue => UpdateManager.ReleaseNoteValue;
        public Uri ReleaseNoteUri => UpdateManager.ReleaseNoteUri;
        public string ReleaseHash => UpdateManager.ReleaseHash;
        public DateTime ReleaseTimestamp => UpdateManager.ReleaseTimestamp;

        public bool NeedSave { get; private set; } = true;
        ManagerSetting ManagerSetting { get; set; }

        public WorkspaceState WorkspaceState { get; private set; }

        public WorkspaceItemSetting SelectedWorkspaceItem { get; private set; }

        ActiveWorkspace ActiveWorkspace { get; } = new ActiveWorkspace();

        #endregion

        #region function

        public ILogger CreateLogger(string subject)
        {
            return LogManager.CreateLogger(NKitApplicationKind.Manager, subject);
        }

        void InitializeLogger(CommandLine commandLine)
        {
            Logger = LogManager.CreateLogger(NKitApplicationKind.Manager, "WORKER");

            string outputLogPath = null;
            // こいつは環境変数を展開しない。というか InitializeEnvironmentVariable と合わなくなるんでしたくない
            if(commandLine.HasOption("log-dir")) {
                var path = commandLine.GetValue("log-dir");
                if(!string.IsNullOrWhiteSpace(path)) {
                    var dir = Directory.CreateDirectory(path);
                    outputLogPath = Path.Combine(dir.FullName, DateTime.Now.ToString("yyyy-MM-dd_hhmmss") + ".log");
                    var writer = File.CreateText(outputLogPath);
                    writer.AutoFlush = true;
                    LogManager.AttachOutputWriter(writer, false);
                }
            }

#if DEBUG
            // 通常の Debug をラップ
            // このためだけにわざわざ Debug.WriteLine を LogManager に書きたくないのですよ
            LogManager.AttachOutputWriter(System.Console.Out, true);
            // べつに消さんでもいいとおもいました
            //Debug.Listeners.Remove("Default");
#endif
            Logger.Information($"initialized logger");

            if(outputLogPath != null) {
                Logger.Information($"output log: {outputLogPath}");
            } else {
                Logger.Debug($"output log: nothing");
            }
        }

        void InitializeEnvironmentVariable(CommandLine commandLine)
        {
            void Set(string commandlineKey, string environmentVariableKey)
            {
                if(commandLine.HasOption(commandlineKey)) {
                    var value = commandLine.GetValue(commandlineKey);
                    if(!string.IsNullOrWhiteSpace(value)) {
                        Environment.SetEnvironmentVariable(environmentVariableKey, value);
                    }
                }
            }

            Set("user_root", CommonUtility.EnvironmentKeyUserDirectory);
            Set("data_root", CommonUtility.EnvironmentKeyDataDirectory);

            Logger.Information($"user dir: {CommonUtility.GetUserDirectory().FullName}");
            Logger.Information($"data dir: {CommonUtility.GetDataDirectory().FullName}");
        }

        FileInfo GetSettingFile()
        {
            var userDir = CommonUtility.GetUserDirectory();
            var settingPath = Path.Combine(userDir.FullName, "setting.xml");

            var result = new FileInfo(settingPath);
            result.Refresh();

            return result;
        }

        public void Initialize()
        {
            var commandLine = new CommandLine();

            InitializeLogger(commandLine);
            InitializeEnvironmentVariable(commandLine);

            var baseId = DateTime.Now.ToFileTime();
            ActiveWorkspace.ApplicationId = $"NKIT_{baseId}_ID";
            SetCurrentProcessExplicitAppUserModelID(ActiveWorkspace.ApplicationId);
        }

        public void LoadSetting()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var currentTimestamp = DateTime.UtcNow;

            var settingFile = GetSettingFile();

            if(settingFile.Exists) {
                try {
                    using(var stream = File.Open(settingFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        var serializer = new DataContractSerializer(typeof(ManagerSetting));
                        using(var reader = XmlReader.Create(stream)) {
                            ManagerSetting = (ManagerSetting)serializer.ReadObject(reader);
                            IsUpdatedFirstExecute = ManagerSetting.LastExecuteVersion < currentVersion;
                            ManagerSetting.LastExecuteVersion = currentVersion;
                            ManagerSetting.LastExecuteTimestamp = currentTimestamp;
                            ManagerSetting.ExecuteCount += 1;
                            IsFirstExecute = false;
                            return;
                        }
                    }
                } catch(SerializationException ex) {
                    Logger.Error(ex);
                } catch(IOException ex) {
                    Logger.Error(ex);
                }
            }


            // 何もかもを突き抜けてきた場合は初めて起動したものとみなす
            ManagerSetting = new ManagerSetting() {
                Accepted = false,
                ExecuteCount = 1,
                FirstExecuteVersion = currentVersion,
                FirstExecuteTimestamp = currentTimestamp,
                LastExecuteVersion = currentVersion,
                LastExecuteTimestamp = currentTimestamp,
            };
            IsFirstExecute = true;
        }

        public void SaveSetting()
        {
            var settingFile = GetSettingFile();

            using(var stream = settingFile.Create()) {
                using(var writer = XmlWriter.Create(stream)) {
                    var serializer = new DataContractSerializer(typeof(ManagerSetting));
                    serializer.WriteObject(writer, ManagerSetting);
                }
            }
        }

        T LoadXmlObject<T>(string path)
        {
            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var reader = XmlReader.Create(stream)) {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
            }
        }

        public AcceptVersion GetAcceptVersion()
        {
            var versionDefinePath = Path.Combine(CommonUtility.GetEtcDirectory().FullName, "version.xml");
            return LoadXmlObject<AcceptVersion>(versionDefinePath);
        }

        public bool CheckNeedAccept()
        {
            if(IsFirstExecute) {
                Logger.Debug("first");
                return true;
            }
            if(!Accepted) {
                Logger.Debug("not accepted");
                return true;
            }

            var version = GetAcceptVersion();

            Logger.Debug($"{ManagerSetting.LastExecuteVersion} <= {version.MinimumVersion}");
            return ManagerSetting.LastExecuteVersion <= version.MinimumVersion;
        }

        public void ListupWorkspace(ComboBox targetControl, Guid selectedWorkspaceId)
        {
            targetControl.Items.Clear();

            if(!ManagerSetting.Workspace.Items.Any()) {
                WorkspaceState = WorkspaceState.None;
                SelectedWorkspaceItem = null;
                return;
            }

            var items = ManagerSetting.Workspace.Items
                .OrderByDescending(i => i.UpdatedTimestamp)
                .Select(i => new CustomDisplayItem<WorkspaceItemSetting>(i) {
                    CustomDisplayText = v => v.Name
                })
                .ToArray()
            ;

            targetControl.Items.AddRange(items);

            var targetGuid = selectedWorkspaceId == Guid.Empty
                ? ManagerSetting.Workspace.LastUseWorkspaceId
                : selectedWorkspaceId
            ;
            var lastUseItem = items.SingleOrDefault(i => i.Value.Id == targetGuid);
            if(lastUseItem != null) {
                targetControl.SelectedItem = lastUseItem;
            } else if(items.Any()) {
                targetControl.SelectedIndex = 0;
            }

            SelectedWorkspaceItem = ((CustomDisplayItem<WorkspaceItemSetting>)targetControl.SelectedItem).Value;

            WorkspaceState = WorkspaceState.Selecting;
        }

        public bool SaveWorkspace(ErrorProvider errorProvider, Control nameControl, Control directoryControl, CheckBox loggingControl)
        {

            var hasError = false;

            var name = nameControl.Text;
            if(string.IsNullOrWhiteSpace(name)) {
                errorProvider.SetError(nameControl, "empty");
                hasError = true;
            } else {
                errorProvider.SetError(nameControl, null);
            }

            var directoryPath = directoryControl.Text;
            if(string.IsNullOrWhiteSpace(directoryPath)) {
                errorProvider.SetError(directoryControl, "empty");
                hasError = true;
            } else {
                errorProvider.SetError(directoryControl, null);
            }

            var logging = loggingControl.Checked;

            if(hasError) {
                return false;
            }


            var currentTimestamp = DateTime.UtcNow;

            if(WorkspaceState == WorkspaceState.None || WorkspaceState == WorkspaceState.Creating) {
                Debug.Assert(SelectedWorkspaceItem == null);
                // 新規作成
                var item = new WorkspaceItemSetting() {
                    CreatedTimestamp = currentTimestamp,
                };

                var items = ManagerSetting.Workspace.Items.ToList();
                items.Add(item);
                ManagerSetting.Workspace.Items = items.ToArray();

                SelectedWorkspaceItem = item;
            } else if(WorkspaceState == WorkspaceState.Copy) {
                // コピー
                throw new NotImplementedException("ワークスペース内のデータが決まったら対応する");
            }

            SelectedWorkspaceItem.Name = name;
            SelectedWorkspaceItem.DirectoryPath = directoryPath;
            SelectedWorkspaceItem.UpdatedTimestamp = currentTimestamp;
            SelectedWorkspaceItem.Logging = logging;

            WorkspaceState = WorkspaceState.Selecting;

            return true;
        }

        public void ClearSelectedWorkspace()
        {
            SelectedWorkspaceItem = null;
            WorkspaceState = WorkspaceState.Creating;
        }

        public void CopySelectedWorkspace()
        {
            WorkspaceState = WorkspaceState.Copy;
        }

        public void DeleteSelectedWorkspace()
        {
            if(WorkspaceState == WorkspaceState.Creating) {
                Debug.Assert(SelectedWorkspaceItem == null);
                return;
            }

            ManagerSetting.Workspace.Items = ManagerSetting.Workspace.Items
                .Where(i => i.Id != SelectedWorkspaceItem.Id)
                .ToArray()
            ;
        }

        public void ChangeSelectedItem(ComboBox targetControl)
        {
            if(WorkspaceState == WorkspaceState.Creating) {
                return;
            }

            if(targetControl.SelectedIndex == -1) {
                SelectedWorkspaceItem = null;
                if(0 < targetControl.Items.Count) {
                    WorkspaceState = WorkspaceState.Selecting;
                } else {
                    WorkspaceState = WorkspaceState.None;
                }
            } else {
                SelectedWorkspaceItem = ((CustomDisplayItem<WorkspaceItemSetting>)targetControl.SelectedItem).Value;
                WorkspaceState = WorkspaceState.Selecting;
            }
        }

        public void LoadSelectedWorkspace()
        {
            if(string.IsNullOrWhiteSpace(SelectedWorkspaceItem.DirectoryPath)) {
                Logger.Warning("workspace dir path is empry");
                return;
            }
            var workspaceDirPath = Environment.ExpandEnvironmentVariables(SelectedWorkspaceItem.DirectoryPath);
            if(Directory.Exists(workspaceDirPath)) {
                //TODO: ワークスペースチェック
            } else {
                // 何やってるか忘れないために冗長に書いとく
                try {
                    Directory.CreateDirectory(workspaceDirPath);
                } catch(DirectoryNotFoundException ex) {
                    Logger.Error(ex);
                    Logger.Warning("drive!");
                    return;
                }
                Directory.CreateDirectory(Path.Combine(workspaceDirPath, CommonUtility.WorkspaceSettingDirectoryName));
                Directory.CreateDirectory(Path.Combine(workspaceDirPath, CommonUtility.WorkspaceLogDirectoryName));
                Directory.CreateDirectory(Path.Combine(workspaceDirPath, CommonUtility.WorkspaceTemporaryDirectoryName));
            }

            // ワークスペースにログ出力開始
            if(SelectedWorkspaceItem.Logging) {
                ActiveWorkspace.LogFilePath = Path.Combine(workspaceDirPath, CommonUtility.WorkspaceLogDirectoryName, DateTime.Now.ToString("yyyy-MM-dd_hhmmss") + ".log");
                ActiveWorkspace.LogWriter = File.CreateText(ActiveWorkspace.LogFilePath);
                LogManager.AttachOutputWriter(ActiveWorkspace.LogWriter, true);
                Logger.Information($"work space log: {ActiveWorkspace.LogFilePath}");
            } else {
                ActiveWorkspace.LogFilePath = null;
                ActiveWorkspace.LogWriter = null;
                Logger.Information("work space log: none");
            }

            // 直近ワークスペース設定
            ManagerSetting.Workspace.LastUseWorkspaceId = SelectedWorkspaceItem.Id;

            // 起動処理
            var aboutId = DateTime.Now.ToFileTime().ToString();
            ActiveWorkspace.ServiceUri = new Uri($"net.pipe://localhost/cttn-nkit-{aboutId}");
            ActiveWorkspace.ExitEventName = $"exit-{aboutId}";

            NKitApplicationTalkerHost = new NKitApplicationTalkerHost(ActiveWorkspace.ServiceUri, CommonUtility.AppAddress);
            NKitApplicationTalkerHost.ApplicationWakeup += NKitApplicationTasker_ApplicationWakeup;
            NKitApplicationTalkerHost.Open();

            NKitLoggingTalkerHost = new NKitLoggingTalkerHost(ActiveWorkspace.ServiceUri, CommonUtility.LogAddress);
            NKitLoggingTalkerHost.LoggingWrite += NKitLoggingTalkerHost_LoggingWrite;
            NKitLoggingTalkerHost.Open();

            ApplicationManager.ExecuteMainApplication(ActiveWorkspace, SelectedWorkspaceItem);

            WorkspaceState = WorkspaceState.Running;

            SaveSetting();
        }

        public bool CheckCanExit()
        {
            var result = SelectedWorkspaceItem == null || (WorkspaceState != WorkspaceState.Running && WorkspaceState != WorkspaceState.Updating);

            Logger.Information($"can exit: {result}");

            return result;
        }

        public Task CheckUpdateAsync(Control updateFireControl)
        {
            updateFireControl.Enabled = false;
            return UpdateManager.CheckUpdateAsync().ContinueWith(t => {
                updateFireControl.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Task<bool> ExecuteUpdateAsync(EventWaitHandle executeEvent)
        {
            //TODO: すでに動いてるやつらを落としたりなんかする


            // 怖いし設定は保存しとく
            SaveSetting();

            WorkspaceState = WorkspaceState.Updating;
            // GUI側の待機状態を解除
            executeEvent.Set();

            // アップデート処理開始
            return UpdateManager.UpdateAsync().ContinueWith(t => {
                // ダメだったら何食わぬ顔で元に戻す
                if(!t.Result) {
                    if(SelectedWorkspaceItem!=null) {
                        WorkspaceState = WorkspaceState.Selecting;
                    } else {
                        WorkspaceState = WorkspaceState.None;
                    }
                }

                return t.Result;
            });
        }

        public void ExecuteTest(TestExecuteForm testExecuteForm, bool force)
        {
            testExecuteForm.SetApplicationManager(ApplicationManager);
            testExecuteForm.ForceExecute = force;
            testExecuteForm.ShowDialog();
        }

        #endregion

        #region DisposerBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(NKitApplicationTalkerHost != null) {
                        NKitApplicationTalkerHost.ApplicationWakeup -= NKitApplicationTasker_ApplicationWakeup;
                        NKitApplicationTalkerHost.Dispose();
                    }
                    if(NKitLoggingTalkerHost != null) {
                        NKitLoggingTalkerHost.LoggingWrite -= NKitLoggingTalkerHost_LoggingWrite;
                        NKitLoggingTalkerHost.Dispose();
                    }

                    ApplicationManager.MainApplicationExited -= ApplicationManager_MainApplicationExited;
                    ApplicationManager.Dispose();

                    LogManager.LogWrite -= LogManager_LogWrite;
                    LogManager.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        private void ApplicationManager_MainApplicationExited(object sender, EventArgs e)
        {
            NKitApplicationTalkerHost.ApplicationWakeup -= NKitApplicationTasker_ApplicationWakeup;
            NKitApplicationTalkerHost.Dispose();

            NKitLoggingTalkerHost.LoggingWrite -= NKitLoggingTalkerHost_LoggingWrite;
            NKitLoggingTalkerHost.Dispose();

            WorkspaceState = WorkspaceState.Selecting;
            if(WorkspaceExited != null) {
                WorkspaceExited(sender, e);
            }

            if(ActiveWorkspace.LogWriter != null) {
                LogManager.DetachOutputWriter(ActiveWorkspace.LogWriter);
                ActiveWorkspace.LogWriter.Dispose();
            }
        }

        private void NKitApplicationTasker_ApplicationWakeup(object sender, TalkApplicationWakeupEventArgs e)
        {
            ApplicationManager.ExecuteNKitApplication(e.SenderApplication, e.TargetApplication, ActiveWorkspace, SelectedWorkspaceItem, e.Arguments, e.WorkingDirectoryPath);
        }

        private void NKitLoggingTalkerHost_LoggingWrite(object sender, TalkLoggingWriteEventArgs e)
        {
            LogManager.WriteTalkLog(e);
        }

        private void LogManager_LogWrite(object sender, LogEventArgs e)
        {
            if(OutputLog != null) {
                OutputLog(this, e);
            }
        }


    }
}
