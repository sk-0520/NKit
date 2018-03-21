using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class ClipboardOperator
    {
        #region function

        public bool CopyText(string text)
        {
            try {
                Clipboard.SetText(text);
                return true;
            } catch(Exception ex) {
                Log.Out.Warning(ex);
            }

            return false;
        }

        public bool CopyFiles(IEnumerable<FileSystemInfo> fileSystemInfos)
        {
            try {
                var data = new DataObject();

                var fileNames = fileSystemInfos.Select(f => f.FullName).ToList();
                
                var fileNameCollection = TextUtility.ToStringCollection(fileNames);
                data.SetFileDropList(fileNameCollection);
                data.SetText(string.Join(Environment.NewLine, fileSystemInfos));

                Clipboard.SetDataObject(data);
                return true;
            } catch(Exception ex) {
                Log.Out.Warning(ex);
            }

            return false;
        }

        public bool CopyFile(FileSystemInfo fileSystemInfos)
        {
            return CopyFiles(new[] { fileSystemInfos });
        }

        public bool CopyImage(BitmapSource bitmapSource)
        {
            try {
                Clipboard.SetImage(bitmapSource);
                return true;
            } catch(Exception ex) {
                Log.Out.Warning(ex);
            }

            return false;
        }

        #endregion
    }
}
