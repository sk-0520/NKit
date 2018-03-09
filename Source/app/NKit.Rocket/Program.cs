using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Rocket.Model;
using ContentTypeTextNet.NKit.Utility.Model;

namespace NKit.Rocket
{
    class Program
    {
        static Program()
        {
            InitializeApplicationLibraryDirectory();
        }

        static AssemblyResolveHelper AssemblyResolveHelper { get; set; }
        static void InitializeApplicationLibraryDirectory()
        {
            var libDir = CommonUtility.GetLibraryDirectoryForApplication();
            AssemblyResolveHelper = new AssemblyResolveHelper(libDir);
        }

        static int Main(string[] args)
        {
            using(var logSwitcher = new LogSwitcher(NKitApplicationKind.Rocket, new Uri("net.pipe://localhost/cttn-nkit"), "log")) {
                logSwitcher.Initialize();
                Log.Initialize(logSwitcher);
                var logger = Log.CreateLogger();

                logger.Information("!!START!!");

                var rocket = new RocketModel(args);
                var result = rocket.Run(CancellationToken.None);


                logger.Information($"{rocket.StartTimestamp}");
                logger.Information($"{rocket.PreparationSpan}");
                logger.Information($"{rocket.EndTimestamp}");
                logger.Information($"{rocket.EndTimestamp - rocket.StartTimestamp}");
                logger.Information($"{rocket.RunState}");
                logger.Information($"{result}");

                logger.Information("!!END!!");

                return result;
            }
        }
    }
}
