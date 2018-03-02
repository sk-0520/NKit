using System;
using System.IO;
using System.Reflection;

namespace ContentTypeTextNet.NKit.Common
{
    public class AssemblyResolveHelper : IDisposable
    {
        public AssemblyResolveHelper(DirectoryInfo libraryDirectory)
        {
            LibraryDirectory = libraryDirectory;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        ~AssemblyResolveHelper()
        {
            Dispose(false);
        }

        #region property

        public bool IsDisposed { get; private set; }

        public DirectoryInfo LibraryDirectory { get; }

        #endregion

        #region function

        protected void Dispose(bool disposing)
        {
            if(IsDisposed) {
                return;
            }

            if(disposing) {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name).Name + ".dll";
            var path = Path.Combine(LibraryDirectory.FullName, name);
            var absPath = Path.GetFullPath(path);
            if(File.Exists(absPath)) {
                var asm = Assembly.LoadFrom(absPath);
                return asm;
            }

            // 見つかんないともう何もかもおかしい、と思ったけど resource.dll もこれで飛んでくんのかい
            //throw new FileNotFoundException(absPath);
            return null;
        }

    }
}
