using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File.Browser
{
    public class BrowserModel: ModelBase
    {
        public BrowserModel(BrowserKind kind, FileInfo fileInfo)
            : this(kind, fileInfo, Encoding.UTF8)
        { }

        public BrowserModel(BrowserKind kind, FileInfo fileInfo, Encoding encoding)
        {
            BrowserKind = kind;
            FileInfo = fileInfo;
            Encoding = encoding;
        }

        #region property

        public BrowserKind BrowserKind { get; }

        public FileInfo FileInfo { get; }

        public Encoding Encoding { get; }

        #endregion

        #region function



        #endregion
    }
}
