using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        #region variable

        WindowState _windowState;

        #endregion

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

        #region window

        public WindowState WindowState
        {
            get { return this._windowState; }
            set { SetProperty(ref this._windowState, value); }
        }

        public double Left
        {
            get { return Model.Setting.MainWindow.Left; }
            set
            {
                if(WindowState == WindowState.Normal) {
                    SetPropertyValue(Model.Setting.MainWindow, value, nameof(Model.Setting.MainWindow.Left));
                }
            }
        }
        public double Top
        {
            get { return Model.Setting.MainWindow.Top; }
            set
            {
                if(WindowState == WindowState.Normal) {
                    SetPropertyValue(Model.Setting.MainWindow, value, nameof(Model.Setting.MainWindow.Top));
                }
            }
        }
        public double Width
        {
            get { return Model.Setting.MainWindow.Width; }
            set
            {
                if(WindowState == WindowState.Normal) {
                    SetPropertyValue(Model.Setting.MainWindow, value, nameof(Model.Setting.MainWindow.Width));
                }
            }
        }
        public double Height
        {
            get { return Model.Setting.MainWindow.Height; }
            set
            {
                if(WindowState == WindowState.Normal) {
                    SetPropertyValue(Model.Setting.MainWindow, value, nameof(Model.Setting.MainWindow.Height));
                }
            }
        }

        #endregion

        #endregion
    }
}
