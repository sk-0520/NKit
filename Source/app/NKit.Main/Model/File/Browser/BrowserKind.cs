using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model.File.Browser
{
    public enum BrowserKind
    {
        Unknown,

        PlainText,
        Ini,
        CSharp,
        Html,
        Xml,

        Application,

        Png,
        Jpeg,
        Bmp,
    }

    public static class BrowserKindExtension
    {
        public static bool IsText(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.PlainText,
                BrowserKind.Ini,
                BrowserKind.CSharp,
                BrowserKind.Html,
                BrowserKind.Xml,
            }.Any(bk => bk == browserKind);
        }
    }

}
