using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.JustLooking.Model;
using ContentTypeTextNet.NKit.JustLooking.View;
using ContentTypeTextNet.NKit.JustLooking.ViewModel;

namespace NKit.JustLooking
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        #region property

        AssemblyResolveHelper AssemblyResolveHelper { get; set; }

        JustLookingModel JustLookingModel { get; set; }
        #endregion

        #region function

        void InitializeApplicationLibraryDirectory()
        {
            var libDir = CommonUtility.GetLibraryDirectoryForApplication();
            AssemblyResolveHelper = new AssemblyResolveHelper(libDir);
        }

        Window StartApplication(string[] arguments)
        {
            JustLookingModel = new JustLookingModel();
            JustLookingModel.Initialize(arguments);

            var viewModel = new JustLookingViewModel(JustLookingModel);
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

            StartupOptions.Initialize(e.Args);
            if(StartupOptions.LetsDie) {
                Shutdown(0);
                return;
            }

            MainWindow = StartApplication(e.Args);

            MainWindow.Show();
        }

        #endregion
    }
}
