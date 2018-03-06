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

            var processModel = new ProcessModel(args);
            var task =  processModel.RunAsync(CancellationToken.None);

            Trace.WriteLine($"{processModel.StartTimestamp}");

            task.ConfigureAwait(false);
            task.Wait();
            var result = task.Result;

            Trace.WriteLine($"{processModel.PreparationSpan}");
            Trace.WriteLine($"{processModel.EndTimestamp}");
            Trace.WriteLine($"{processModel.EndTimestamp - processModel.StartTimestamp}");
            Trace.WriteLine($"{processModel.RunState}");
            Trace.WriteLine($"{result}");

            Trace.WriteLine("!!END!!");

            return result;
        }
    }
}
