
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Cameraman
{
    class Program
    {
        static AssemblyResolveHelper AssemblyResolveHelper { get; set; }

        static void InitializeApplicationLibraryDirectory()
        {
            var libDir = CommonUtility.GetLibraryDirectoryForApplication();
            AssemblyResolveHelper = new AssemblyResolveHelper(libDir);
        }

        static Program()
        {
            InitializeApplicationLibraryDirectory();
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            StartupOptions.Initialize(args);
            if(StartupOptions.LetsDie) {
                return;
            }

            using(var logSwitcher = new LogSwitcher(NKitApplicationKind.Cameraman, StartupOptions.ServiceUri)) {
                logSwitcher.Initialize();
                Log.Initialize(logSwitcher);
                var logger = Log.CreateLogger();

                logger.Information("!!START!!");
                logger.Information($"this template compiled: 2018-03-21 05:56:44Z UTC");

                var model = new ContentTypeTextNet.NKit.Cameraman.Model.CameramanModel(args);
                var form = new ContentTypeTextNet.NKit.Cameraman.View.InformationForm();
                form.SetModel(model);

                                model.Execute(form);
                
//                logger.Information($"RESULT STATUS =================");
//                logger.Information($"{model.StartUtcTimestamp}");
//                logger.Information($"{model.PreparateSpan}");
//                logger.Information($"{model.EndUtcTimestamp}");
//                logger.Information($"{model.EndUtcTimestamp - model.StartUtcTimestamp}");
//                logger.Information($"{model.RunState}");
//                logger.Information($"{result}");

                logger.Information("!!END!!");

//                return result;
            }
        }
    }
}

