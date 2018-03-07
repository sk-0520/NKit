using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Rocket.Model;

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
            Trace.WriteLine("!!START!!");

            var rocket = new RocketModel(args);
            var result =  rocket.Run(CancellationToken.None);


            Trace.WriteLine($"{rocket.StartTimestamp}");
            Trace.WriteLine($"{rocket.PreparationSpan}");
            Trace.WriteLine($"{rocket.EndTimestamp}");
            Trace.WriteLine($"{rocket.EndTimestamp - rocket.StartTimestamp}");
            Trace.WriteLine($"{rocket.RunState}");
            Trace.WriteLine($"{result}");

            Trace.WriteLine("!!END!!");

            return result;
        }
    }
}
