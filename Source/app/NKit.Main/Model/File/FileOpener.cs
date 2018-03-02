using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileOpener
    {
        Process OpenCore(string path)
        {
            try {
                return Process.Start(path);
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }

            return null;

        }
        public Process Open(FileInfo file)
        {
            return OpenCore(file.FullName);
        }

        public Process Open(DirectoryInfo directory)
        {
            return OpenCore(directory.FullName);
        }

        public Process OpenParentDirectory(FileSystemInfo fileSystemInfo)
        {
            throw new NotImplementedException("Explorerに渡すやつ, 後でやる");
        }

        public void ShowProperty(FileSystemInfo fileSystemInfo)
        {
            throw new NotImplementedException("windows api");
        }
    }
}
