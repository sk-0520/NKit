using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class ClipboardOperator
    {
        #region function

        public void CopyText(string text)
        {
            try {
                Clipboard.SetText(text);
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        public void CopyFiles(IEnumerable<FileSystemInfo> fileSystemInfos)
        {
            try {
                var data = new DataObject();

                var fileNames = fileSystemInfos.Select(f => f.FullName).ToList();
                
                var fileNameCollection = TextUtility.ToStringCollection(fileNames);
                data.SetFileDropList(fileNameCollection);
                data.SetText(string.Join(Environment.NewLine, fileSystemInfos));

                Clipboard.SetDataObject(data);
            } catch(Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        public void CopyFile(FileSystemInfo fileSystemInfos)
        {
            CopyFiles(new[] { fileSystemInfos });
        }

        #endregion
    }
}
