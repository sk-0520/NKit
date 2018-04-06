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

        Png,
        Jpeg,
        Bmp,

        Application,
        Dll,
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

        public static bool IsImage(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.Bmp,
                BrowserKind.Jpeg,
                BrowserKind.Png,
            }.Any(bk => bk == browserKind);
        }

        public static bool IsProgram(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.Application,
                BrowserKind.Dll,
            }.Any(bk => bk == browserKind);
        }
    }

}
