using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class ManagerWorker : DisposerBase
    {
        #region property

        bool IsFirstExecute { get; set; }
        public bool NeedSave { get; private set; }
        ManagerSetting ManagerSetting { get; set; }
        public bool Accepted { get; set; }

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

            using(var stream = settingFile.OpenWrite()) {
                var serializer = new DataContractSerializer(typeof(ManagerSetting));
                using(var writer = XmlWriter.Create(stream)) {
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

            var version = GetAcceptVersion();

            return ManagerSetting.LastExecuteVersion <= version.MinimumVersion;
        }

        #endregion
    }
}
