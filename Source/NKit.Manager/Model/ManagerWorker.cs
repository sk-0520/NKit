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
using ContentTypeTextNet.NKit.Manager.Model.Workspace.Setting;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class ManagerWorker : DisposerBase
    {
        #region event

        public event EventHandler<EventArgs> WorkspaceExited;

        #endregion

        #region property

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

        ApplicationManager ApplicationManager { get; } = new ApplicationManager();

        #endregion

        #region function

        void InitializeEnvironmentVariableByCommandLine()
        {
            // コマンドラインから環境変数設定
            var commandLine = new CommandLine();

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
            InitializeEnvironmentVariableByCommandLine();
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
                    Debug.WriteLine(ex);
                } catch(IOException ex) {
                    Debug.WriteLine(ex);
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
                return true;
            }
            if(!Accepted) {
                return true;
            }

            var version = GetAcceptVersion();

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
            ApplicationManager.MainProcessExited -= ApplicationManager_MainProcessExited;
            ApplicationManager.ExecuteMainApplication(SelectedWorkspaceItem);
            ApplicationManager.MainProcessExited += ApplicationManager_MainProcessExited;

            WorkspaceState = WorkspaceState.Running;
        }

        #endregion

        private void ApplicationManager_MainProcessExited(object sender, EventArgs e)
        {
            WorkspaceState = WorkspaceState.Selecting;
            if(WorkspaceExited != null) {
                WorkspaceExited(sender, e);
            }
        }

    }
}
