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

        public bool IsReadOnly { get; set; } = true;

        #endregion

        #region function

        static public BrowserKind GetBrowserKind(string fileName)
        {
            var ext = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();
            var items = new[] {
                // text
                new { Kind = BrowserKind.PlainText, Extensions = new [] { "txt" } },
                // ex text
                new { Kind = BrowserKind.Xml, Extensions = new [] { "xml" } },
                // image
                new { Kind = BrowserKind.Bmp, Extensions = new [] { "bmp" } },
                new { Kind = BrowserKind.Png, Extensions = new [] { "png" } },
                new { Kind = BrowserKind.Jpeg, Extensions = new [] { "jpeg", "jpg" } },
                // program
                new { Kind = BrowserKind.Application, Extensions = new [] { "exe" } },
                new { Kind = BrowserKind.Dll, Extensions = new [] { "dll" } },
            };
            var item = items.FirstOrDefault(i => i.Extensions.Any(e => e == ext));
            if(item != null) {
                return item.Kind;
            }

            return BrowserKind.Unknown;
        }


        #endregion
    }
}
