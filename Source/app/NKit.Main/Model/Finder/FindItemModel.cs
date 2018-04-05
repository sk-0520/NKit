using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Main.Model.File.Browser;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Setting.File;
using ContentTypeTextNet.NKit.Setting.Finder;
using ContentTypeTextNet.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public class FindItemModel : ModelBase
    {
        public FindItemModel(DirectoryInfo baseDirectory, FileInfo fileInfo, TextSearchResult fileNameSearchResult, bool matchedFileSize, FilePropertySearchResult filePropertySearchResult, FileContentSearchResult fileContentSearchResult, IReadOnlyFinderSetting finderSetting, IReadOnlyAssociationFileSetting associationFileSetting, IReadOnlyNKitSetting nkitSetting)
        {
            BaseDirectory = baseDirectory;
            FileInfo = fileInfo;
            FileNameSearchResult = fileNameSearchResult;
            MatchedFileSize = matchedFileSize;
            FilePropertySearchResult = filePropertySearchResult;
            FileContentSearchResult = fileContentSearchResult;
            FinderSetting = finderSetting;
            AssociationFileSetting = associationFileSetting;
            NKitSetting = nkitSetting;
        }

        #region property

        public DirectoryInfo BaseDirectory { get; }
        public FileInfo FileInfo { get; }
        public TextSearchResult FileNameSearchResult { get; }
        public bool MatchedFileSize { get; }
        public FilePropertySearchResult FilePropertySearchResult { get; }
        public FileContentSearchResult FileContentSearchResult { get; }

        IReadOnlyFinderSetting FinderSetting { get; }
        IReadOnlyAssociationFileSetting AssociationFileSetting { get; }
        IReadOnlyNKitSetting NKitSetting { get; }

        public string RelativeDirectoryPath
        {
            get
            {
                var baseUri = new Uri(BaseDirectory.FullName);
                var parentDirectoryUri = new Uri(Path.GetDirectoryName(FileInfo.FullName));
                var relativeUri = baseUri.MakeRelativeUri(parentDirectoryUri);
                var relativeUriText = relativeUri.ToString();
                return Uri.UnescapeDataString(relativeUriText).Replace('/', Path.DirectorySeparatorChar);
            }
        }

        public FileTypeModel FileType => new FileTypeModel((FileInfo)FileInfo, NKitSetting);

        public FileHashModel FileHash => new FileHashModel((FileInfo)FileInfo);

        #endregion

        #region function
        public void OpenFile()
        {
            var opener = new FileOpener();
            opener.Open((FileInfo)FileInfo);
        }

        public void OpenDirectory()
        {
            var opener = new FileOpener();
            opener.OpenParentDirectory(FileInfo);
        }

        public void ShowProperty()
        {
            var opener = new FileOpener();
            opener.ShowProperty(FileInfo, IntPtr.Zero/*TODO: どうすんのよこれ*/);
        }

        public void CopyFileSize()
        {
            var co = new ClipboardOperator();
            var file = (FileInfo)FileInfo;
            co.CopyText(file.Length.ToString());
        }

        public void CopyFile()
        {
            var co = new ClipboardOperator();
            co.CopyFile(FileInfo);
        }

        public void CopyNameWithExtension()
        {
            var co = new ClipboardOperator();
            co.CopyText(FileInfo.Name);
        }

        public void CopyNameWithoutExtension()
        {
            var co = new ClipboardOperator();
            var s = Path.GetFileNameWithoutExtension(FileInfo.Name);
            co.CopyText(s);
        }

        public void CopyDirectory()
        {
            var co = new ClipboardOperator();
            var baseDirPath = Path.GetDirectoryName(FileInfo.FullName);
            co.CopyText(baseDirPath);
        }

        public Process OpenAssociationFile(AssociationFileKind associationFileKind, AssociationOpenParameter parameter)
        {
            var ao = new AssociationFileOpener(AssociationFileSetting);

            return ao.Open(associationFileKind, FileInfo, parameter);
        }

        BrowserKind GetBrowserKindForce(string lowDotExtension)
        {
            switch(lowDotExtension) {
                case ".xml":
                    return BrowserKind.Xml;

                default:
                    return BrowserKind.Unknown;
            }
        }

        BrowserKind GetBrowserKindFromSetting(string lowDotExtension)
        {
            var patternCreator = new SearchPatternCreator();

            var map = FinderUtility.CreateFileNameKinds(FinderSetting);

            if(map[FileNameKind.XmlHtml].IsMatch(lowDotExtension)) {
                return BrowserKind.Xml;
            }

            if(map[FileNameKind.Text].IsMatch(lowDotExtension)) {
                return BrowserKind.PlainText;
            }

            return BrowserKind.Unknown;
        }

        BrowserKind GetBrowserKind(string fileName)
        {
            var lowDotExtension = Path.GetExtension(fileName).ToLower();
            var force = GetBrowserKindForce(lowDotExtension);
            if(force != BrowserKind.Unknown) {
                return force;
            }

            return GetBrowserKindFromSetting(lowDotExtension);
        }

        public BrowserModel GetBrowser()
        {
            var browserKind = GetBrowserKind(FileInfo.Name);
            var encoding = Encoding.Default;
            if(FileContentSearchResult.Text.IsMatched) {
                encoding = FileContentSearchResult.Text.EncodingCheck.Encoding;
            }
            return new BrowserModel(browserKind, FileInfo, encoding);
        }

        #endregion
    }
}
