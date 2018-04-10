using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool IsText => BrowserKind.IsText();
        public bool IsXmlHtml => BrowserKind.IsXmlHtml();
        public bool IsImage => BrowserKind.IsImage();
        public bool IsProgram => BrowserKind.IsProgram();

        #endregion

        #region function

        static public BrowserKind GetBrowserKind(string fileName)
        {
            var ext = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();
            var items = new[] {
                // text
                new { Kind = BrowserKind.PlainText, Extensions = new [] { "txt" } },
                new { Kind = BrowserKind.CSharp, Extensions = new [] { "cs" } },
                // ex text
                new { Kind = BrowserKind.Xml, Extensions = new [] { "xml" } },
                new { Kind = BrowserKind.Html, Extensions = new [] { "html", "htm" } },
                // image
                new { Kind = BrowserKind.Bmp, Extensions = new [] { "bmp" } },
                new { Kind = BrowserKind.Png, Extensions = new [] { "png" } },
                new { Kind = BrowserKind.Jpeg, Extensions = new [] { "jpeg", "jpg" } },
                new { Kind = BrowserKind.Gif, Extensions = new [] { "gif" } },
                // program
                new { Kind = BrowserKind.Application, Extensions = new [] { "exe" } },
                new { Kind = BrowserKind.Dll, Extensions = new [] { "dll" } },
            };

            Debug.Assert(items.GroupBy(i => i.Kind).OrderByDescending(g => g.Count()).First().Count() == 1);

            var item = items.FirstOrDefault(i => i.Extensions.Any(e => e == ext));
            if(item != null) {
                return item.Kind;
            }

            return BrowserKind.Unknown;
        }

        public bool CanBrowse(BrowserKind browserKind)
        {
            if(browserKind.IsText()) {
                if(IsText || IsXmlHtml) {
                    return true;
                }
            }

            if(browserKind.IsXmlHtml()) {
                if(IsText || IsXmlHtml) {
                    return true;
                }
            }

            if(browserKind.IsImage()) {
                if(IsImage) {
                    return true;
                }
            }

            if(browserKind.IsProgram()) {
                if(IsProgram) {
                    return true;
                }
            }

            return false;
        }


        #endregion
    }
}
