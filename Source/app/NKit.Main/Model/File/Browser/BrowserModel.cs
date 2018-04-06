using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File.Browser
{
    public class BrowserModel : ModelBase
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

        public bool IsEditable { get; set; }

        #endregion

        #region function

        static public BrowserKind GetBrowserKind(string fileName)
        {
            var ext = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();
            var items = new[] {
                new { Kind = BrowserKind.Xml, Extensions = new [] { "xml" } },
            };
            var item = items.FirstOrDefault(i => i.Extensions.Any(e => e == ext));
            if(item == null) {
                return item.Kind;
            }

            return BrowserKind.Unknown;
        }


        #endregion
    }
}
