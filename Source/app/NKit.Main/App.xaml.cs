using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.View;
using ContentTypeTextNet.NKit.Main.ViewModel;

namespace ContentTypeTextNet.NKit.Main
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        #region property

        AssemblyResolveHelper AssemblyResolveHelper { get; set; }

        #endregion

        #region function

        void InitializeApplicationLibraryDirectory()
        {
            var libDir = CommonUtility.GetLibraryDirectoryForApplication();
            AssemblyResolveHelper = new AssemblyResolveHelper(libDir);
        }

        Window StartApplication()
        {
            var model = new MainModel();
            model.Initialize();

            var viewModel = new MainViewModel(model);
            var view = new MainWindow() {
                DataContext = viewModel,
            };

            return view;
        }

        #endregion

        #region Application

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeApplicationLibraryDirectory();

            base.OnStartup(e);

            MainWindow = StartApplication();

            MainWindow.Show();
        }

        #endregion
    }
}
