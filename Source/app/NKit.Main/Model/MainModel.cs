using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.App;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.Model.System;
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
        Setting Setting { get; set; }

        public AppManagerModel AppManager { get; private set; }
        public FinderManagerModel FinderManager { get; private set; }
        public FileManagerModel FileManager { get; private set; }
        public CaptureManagerModel CaptureManager { get; private set; }
        public SystemManagerModel SystemManager { get; private set; }

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
            Setting = new Setting();

            AppManager = new AppManagerModel(Setting);
            FinderManager = new FinderManagerModel(Setting);
            FileManager = new FileManagerModel(Setting);
            CaptureManager = new CaptureManagerModel(Setting);
            SystemManager = new SystemManagerModel(Setting);

#if DEBUG
            IsInitialized = true;
#endif
        }

        #endregion
    }
}
