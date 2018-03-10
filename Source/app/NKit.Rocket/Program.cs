
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
            using(var logSwitcher = new LogSwitcher(NKitApplicationKind.Rocket, StartupOptions.ServiceUri)) {
                logSwitcher.Initialize();
                Log.Initialize(logSwitcher);
                var logger = Log.CreateLogger();

                logger.Information("!!START!!");
                logger.Information($"this template compiled: 2018-03-10 21:24:34Z UTC");

                if(StartupOptions.LetsDeath) {
                    logger.Information("let's death");
                    return 0;
                }

                var model = new ContentTypeTextNet.NKit.Rocket.Model.RocketModel(args);
                var result = model.Run(CancellationToken.None);

                logger.Information($"RESULT STATUS =================");
                logger.Information($"{model.StartTimestamp}");
                logger.Information($"{model.PreparationSpan}");
                logger.Information($"{model.EndTimestamp}");
                logger.Information($"{model.EndTimestamp - model.StartTimestamp}");
                logger.Information($"{model.RunState}");
                logger.Information($"{result}");

                logger.Information("!!END!!");

                return result;
            }
        }
    }
}

