using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Main.ViewModel.File;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public class FindItemGeneralDetailViewModel : FindItemDetailViewModelBase
    {
        public FindItemGeneralDetailViewModel(FindItemModel model)
            : base(model)
        {
            FileInfo = Model.FileInfo;
        }

        #region property

        FileInfo FileInfo { get; }

        public string FilePath => Model.FileInfo.FullName;
        public string FileName => Model.FileInfo.Name;
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);
        public string Extension => Path.GetExtension(FilePath).Replace(".", string.Empty);
        public string DirectoryPath => Path.GetDirectoryName(FilePath);
        public long FileSize => FileInfo.Length;
        public bool IsHiddenFile => FileInfo.Attributes.HasFlag(FileAttributes.Hidden);

        public FileTypeViewModel FileType => new FileTypeViewModel(Model.FileType);

        public FileHashViewModel FileHash => new FileHashViewModel(Model.FileHash);

        #endregion

        #region FindItemDetailViewModelBase

        public override string Header => Properties.Resources.String_ViewModel_Finder_FindItemDetail_General;

        public override bool Showable => true;

        #endregion
    }
}
