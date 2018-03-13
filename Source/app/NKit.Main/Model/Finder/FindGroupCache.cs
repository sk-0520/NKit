using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Setting.Finder;

namespace ContentTypeTextNet.NKit.Main.Model.Finder
{
    public interface IReadOnlyFindGroupCache
    {
        IReadOnlyFindGroupSetting Setting { get; }

        string RootDirectoryPath { get; }

        Regex FileName { get; }
        Regex FileContent { get; }

        IReadOnlyDictionary<FileNameKind, Regex> FileNameKinds { get; }
    }

    public class FindGroupCache: IReadOnlyFindGroupCache
    {
        #region IReadOnlyFindGroupCache

        public IReadOnlyFindGroupSetting Setting { get; set; }

        public string RootDirectoryPath { get; set; }

        public Regex FileName { get; set; }
        public Regex FileContent { get; set; }

        public IReadOnlyDictionary<FileNameKind, Regex> FileNameKinds { get; set; }
        #endregion
    }
}
