using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.NKit.Setting.Define;
using ContentTypeTextNet.NKit.NKit.Setting.File;
using ContentTypeTextNet.NKit.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindItemModel : ModelBase
    {
        public FindItemModel(DirectoryInfo baseDirectory, FileSystemInfo fileSystemInfo, TextSearchResult fileNameSearchResult, FileContentSearchResult fileContentSearchResult, IReadOnlyAssociationFileSetting associationFileSetting, IReadOnlyNKitSetting nkitSetting)
        {
            BaseDirectory = baseDirectory;
            FileSystemInfo = fileSystemInfo;
            FileNameSearchResult = fileNameSearchResult;
            FileContentSearchResult = fileContentSearchResult;
            AssociationFileSetting = associationFileSetting;
            NKitSetting = nkitSetting;
        }

        #region property

        public DirectoryInfo BaseDirectory { get; }
        public FileSystemInfo FileSystemInfo { get; }
        public TextSearchResult FileNameSearchResult { get; }
        public FileContentSearchResult FileContentSearchResult { get; }

        IReadOnlyAssociationFileSetting AssociationFileSetting { get; }
        IReadOnlyNKitSetting NKitSetting { get; }

        public string RelativeDirectoryPath
        {
            get
            {
                var baseUri = new Uri(BaseDirectory.FullName);
                var parentDirectoryUri = new Uri(Path.GetDirectoryName(FileSystemInfo.FullName));
                var relativeUri = baseUri.MakeRelativeUri(parentDirectoryUri);
                return relativeUri.ToString();
            }
        }

        public FileTypeModel FileType => new FileTypeModel((FileInfo)FileSystemInfo, NKitSetting);

        public FileHashModel FileHash => new FileHashModel((FileInfo)FileSystemInfo);

        #endregion

        #region function
        public void OpenFile()
        {
            var opener = new FileOpener();
            opener.Open((FileInfo)FileSystemInfo);
        }

        public void OpenDirectory()
        {
            var opener = new FileOpener();
            opener.OpenParentDirectory(FileSystemInfo);
        }

        public void ShowProperty()
        {
            var opener = new FileOpener();
            opener.ShowProperty(FileSystemInfo, IntPtr.Zero/*TODO: どうすんのよこれ*/);
        }

        public void CopyFileSize()
        {
            var co = new ClipboardOperator();
            var file = (FileInfo)FileSystemInfo;
            co.CopyText(file.Length.ToString());
        }

        public void CopyFile()
        {
            var co = new ClipboardOperator();
            co.CopyFile(FileSystemInfo);
        }

        public void CopyNameWithExtension()
        {
            var co = new ClipboardOperator();
            co.CopyText(FileSystemInfo.Name);
        }

        public void CopyNameWithoutExtension()
        {
            var co = new ClipboardOperator();
            var s = Path.GetFileNameWithoutExtension(FileSystemInfo.Name);
            co.CopyText(s);
        }

        public void CopyDirectory()
        {
            var co = new ClipboardOperator();
            var baseDirPath = Path.GetDirectoryName(FileSystemInfo.FullName);
            co.CopyText(baseDirPath);
        }

        public Process OpenAssociationFile(AssociationFileKind associationFileKind, AssociationOpenParameter parameter)
        {
            var ao = new AssociationFileOpener(AssociationFileSetting);

            return ao.Open(associationFileKind, FileSystemInfo, parameter);
        }

        #endregion
    }
}
