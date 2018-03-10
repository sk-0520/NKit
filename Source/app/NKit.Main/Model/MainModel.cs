using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.NKit;
using ContentTypeTextNet.NKit.Main.Model.System;
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
        MainSetting Setting { get; set; }

        public NKitManagerModel NKitManager { get; private set; }
        public FinderManagerModel FinderManager { get; private set; }
        public FileManagerModel FileManager { get; private set; }
        public CaptureManagerModel CaptureManager { get; private set; }
        public SystemManagerModel SystemManager { get; private set; }

        LogSwitcher LogSwitcher { get; set; }

        #endregion

        #region function

        public void Initialize()
        {
#if DEBUG
            if (IsInitialized)
            {
                throw new InvalidOperationException();
            }
#endif
            LogSwitcher = new LogSwitcher(Common.NKitApplicationKind.Main, StartupOptions.ServiceUri);
            Log.Initialize(LogSwitcher);

            Setting = new MainSetting();

            NKitManager = new NKitManagerModel(Setting);
            FinderManager = new FinderManagerModel(Setting);
            FileManager = new FileManagerModel(Setting);
            CaptureManager = new CaptureManagerModel(Setting);
            SystemManager = new SystemManagerModel(Setting);

            LogSwitcher.Initialize();

            // 構造上この子の Logger はおバカのまんまなので直してやる必要あり
            ResetLogger(Log.CreateLogger(this));

            Logger.Information("!!START!!");

#if DEBUG
            IsInitialized = true;
#endif
        }

        #endregion
    }
}
