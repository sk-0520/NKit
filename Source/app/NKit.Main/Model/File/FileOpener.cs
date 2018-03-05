using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.Library.PInvoke.Windows;

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
            try {
                return Process.Start("explorer", $"/e, /select,{fileSystemInfo.FullName}");
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }

            return null;
        }

        public void ShowProperty(FileSystemInfo fileSystemInfo, IntPtr hWnd)
        {
            NativeMethods.SHObjectProperties(hWnd, SHOP.SHOP_FILEPATH, fileSystemInfo.FullName, string.Empty);
        }
    }
}
