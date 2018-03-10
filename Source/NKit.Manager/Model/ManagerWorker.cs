using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Manager.Define;
using ContentTypeTextNet.NKit.Manager.Model.Application;
using ContentTypeTextNet.NKit.Manager.Model.Log;
using ContentTypeTextNet.NKit.Manager.Model.Workspace;
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class ManagerWorker : DisposerBase
    {
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
        }


        #region property

        LogManager LogManager { get; }
        ILogger Logger { get; set; }

        ApplicationManager ApplicationManager { get; }

        NKitApplicationTalkerHost NKitApplicationTalkerHost { get; set; }
        NKitLoggingTalkerHost NKitLoggingTalkerHost { get; set; }

        public bool IsFirstExecute { get; private set; }
        public bool Accepted
        {
            get { return ManagerSetting.Accepted; }
            set { ManagerSetting.Accepted = value; }
        }

        public bool NeedSave { get; private set; } = true;
        ManagerSetting ManagerSetting { get; set; }

        public WorkspaceState WorkspaceState { get; private set; }

        public WorkspaceItemSetting SelectedWorkspaceItem { get; private set; }

        WorkspaceVolatilityItem WorkspaceVolatilityItem { get; } = new WorkspaceVolatilityItem();


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

            WorkspaceVolatilityItem.ApplicationId = $"NKIT_{DateTime.Now.ToFileTime()}_ID";
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


        public void ListupWorkspace(ComboBox targetControl)
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

            var lastUseItem = items.SingleOrDefault(i => i.Value.Id == ManagerSetting.Workspace.LastUseWorkspaceId);
            if(lastUseItem != null) {
                targetControl.SelectedItem = lastUseItem;
            } else if(items.Any()) {
                targetControl.SelectedIndex = 0;
            }

            SelectedWorkspaceItem = ((CustomDisplayItem<WorkspaceItemSetting>)targetControl.SelectedItem).Value;

            WorkspaceState = WorkspaceState.Selecting;
        }

        public bool SaveWorkspace(ErrorProvider errorProvider, Control nameControl, Control directoryControl)
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
            NKitApplicationTalkerHost = new NKitApplicationTalkerHost(new Uri("net.pipe://localhost/cttn-nkit"), "app");
            NKitApplicationTalkerHost.ApplicationWakeup += NKitApplicationTasker_ApplicationWakeup;
            NKitApplicationTalkerHost.Open();

            NKitLoggingTalkerHost = new NKitLoggingTalkerHost(new Uri("net.pipe://localhost/cttn-nkit"), "log");
            NKitLoggingTalkerHost.LoggingWrite += NKitLoggingTalkerHost_LoggingWrite;
            NKitLoggingTalkerHost.Open();

            ApplicationManager.ExecuteMainApplication(WorkspaceVolatilityItem, SelectedWorkspaceItem);

            WorkspaceState = WorkspaceState.Running;
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
        }

        private void NKitApplicationTasker_ApplicationWakeup(object sender, TalkApplicationWakeupEventArgs e)
        {
            ApplicationManager.ExecuteNKitApplication(e.SenderApplication, e.TargetApplication, WorkspaceVolatilityItem, SelectedWorkspaceItem, e.Arguments, e.WorkingDirectoryPath);
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
