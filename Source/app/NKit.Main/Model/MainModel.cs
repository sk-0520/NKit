using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Main.Model.Cli;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.NKit;
using ContentTypeTextNet.NKit.Main.Model.Other;
using ContentTypeTextNet.NKit.Main.Model.SystemEnvironment;
using ContentTypeTextNet.NKit.Setting;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Mvvm;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class MainModel : ModelBase
    {
        #region property

#if DEBUG
        public bool IsInitialized { get; private set; } = false;
#endif
        public MainSetting Setting { get; private set; }

        public NKitManagerModel NKitManager { get; private set; }
        public FinderManagerModel FinderManager { get; private set; }
        public FileManagerModel FileManager { get; private set; }
        public CaptureManagerModel CaptureManager { get; private set; }
        public SystemEnvironmentManagerModel SystemEnvironmentManager { get; private set; }
        public CliManagerModel CliManager { get; private set; }
        public OtherManagerModel OtherManager { get; private set; }

        LogSwitcher LogSwitcher { get; set; }

        #endregion

        #region function

        public void Initialize()
        {
#if DEBUG
            if(IsInitialized) {
                throw new InvalidOperationException();
            }
#endif
            LogSwitcher = new LogSwitcher(Common.NKitApplicationKind.Main, StartupOptions.ServiceUri);
            Log.Initialize(LogSwitcher);
            LogSwitcher.Initialize();
            // 構造上この子の Logger はおバカのまんまなので直してやる必要あり
            ResetLogger(Log.CreateLogger(this));
            Logger.Information("!!START!!");

            Setting = LoadSetting();
            InitializeMainSetting();

            NKitManager = new NKitManagerModel(Setting);
            FinderManager = new FinderManagerModel(Setting);
            FileManager = new FileManagerModel(Setting);
            CaptureManager = new CaptureManagerModel(Setting);
            SystemEnvironmentManager = new SystemEnvironmentManagerModel(Setting);
            CliManager = new CliManagerModel(Setting);
            OtherManager = new OtherManagerModel(Setting);

#if DEBUG
            IsInitialized = true;
#endif
        }

        public void Uninitialize()
        {
            Logger.Debug("save setting");
            SaveSetting(Setting);

            Logger.Information("!!END!!");
        }


        FileInfo GetSettingFile()
        {
            var path = Path.Combine(StartupOptions.WorkspacePath, CommonUtility.WorkspaceSettingDirectoryName, "main.json");

            var result = new FileInfo(path);
            result.Refresh();

            return result;
        }

        MainSetting LoadSetting()
        {
            var file = GetSettingFile();
            if(file.Exists) {
                try {
                    using(var stream = file.OpenRead()) {
                        var serializer = new JsonSerializer();
                        return serializer.Load<MainSetting>(stream);
                    }
                } catch(Exception ex) {
                    Logger.Error(ex);
                }
            }

            Logger.Information("create main setting");

            return new MainSetting();
        }

        void SaveSetting(MainSetting setting)
        {
            var file = GetSettingFile();

            // 単独起動の場合にのみディレクトリ作成
            if(StartupOptions.ServiceUri == null) {
                if(!file.Directory.Exists) {
                    file.Directory.Create();
                }
            }

            using(var stream = file.Create()) {
                var serializer = new JsonSerializer();
                serializer.Save(setting, stream);
            }
        }

        void InitializeMainSetting()
        {
            // 位置・サイズに全くこだわりなし
            Setting.MainWindow.Clamp(100, 100, 640, 480);
        }

        #endregion
    }
}
