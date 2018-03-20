using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Capture;
using ContentTypeTextNet.NKit.Main.ViewModel.App;
using ContentTypeTextNet.NKit.Main.ViewModel.Capture;
using ContentTypeTextNet.NKit.Main.ViewModel.Cli;
using ContentTypeTextNet.NKit.Main.ViewModel.File;
using ContentTypeTextNet.NKit.Main.ViewModel.Finder;
using ContentTypeTextNet.NKit.Main.ViewModel.SystemEnvironment;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel
{
    public class MainViewModel : SingleModelViewModelBase<MainModel>
    {
        public MainViewModel(MainModel model)
            : base(model)
        {
            NKitManager = new NKitManagerViewModel(Model.NKitManager);
            CaptureManager = new CaptureManagerViewModel(Model.CaptureManager);
            FileManager = new FileManagerViewModel(Model.FileManager);
            FinderManager = new FinderManagerViewModel(Model.FinderManager);
            SystemEnvironmentManager = new SystemEnvironmentManagerViewModel(Model.SystemEnvironmentManager);
            CliManager = new CliManagerViewModel(Model.CliManager);
        }

        #region property

        public NKitManagerViewModel NKitManager { get; }
        public CaptureManagerViewModel CaptureManager { get; }
        public FileManagerViewModel FileManager { get; }
        public FinderManagerViewModel FinderManager { get; }
        public SystemEnvironmentManagerViewModel SystemEnvironmentManager { get; }
        public CliManagerViewModel CliManager { get; }

        #endregion
    }
}
