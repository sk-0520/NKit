
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Rocket
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

        static int Main(string[] args)
        {
            StartupOptions.Initialize(args);
            if(StartupOptions.LetsDie) {
                return 0;
            }

            using(var logSwitcher = new LogSwitcher(NKitApplicationKind.Rocket, StartupOptions.ServiceUri)) {
                logSwitcher.Initialize();
                Log.Initialize(logSwitcher);
                var logger = Log.CreateLogger();

                logger.Information("!!START!!");
                logger.Information($"this template compiled: 2018-03-20 02:36:25Z UTC");

                var model = new ContentTypeTextNet.NKit.Rocket.Model.RocketModel(args);
                var result = model.Run(CancellationToken.None);

                logger.Information($"RESULT STATUS =================");
                logger.Information($"{model.StartUtcTimestamp}");
                logger.Information($"{model.PreparateSpan}");
                logger.Information($"{model.EndUtcTimestamp}");
                logger.Information($"{model.EndUtcTimestamp - model.StartUtcTimestamp}");
                logger.Information($"{model.RunState}");
                logger.Information($"{result}");

                logger.Information("!!END!!");

                return result;
            }
        }
    }
}

