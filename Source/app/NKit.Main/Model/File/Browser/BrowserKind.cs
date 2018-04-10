using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Main.Model.File.Browser
{
    /// <summary>
    /// 表示ファイル種別。
    /// <para>内部的な切り分け等々あるため結構いっぱい定義する必要あり。</para>
    /// </summary>
    public enum BrowserKind
    {
        /// <summary>
        /// 不明。
        /// </summary>
        Unknown,

        PlainText,
        Ini,
        CSharp,

        Html,
        Xml,

        Png,
        Jpeg,
        Bmp,
        Gif,

        Application,
        Dll,
    }

    public static class BrowserKindExtension
    {
        /// <summary>
        /// テキストファイルとして有効か。
        /// </summary>
        /// <param name="browserKind"></param>
        /// <returns></returns>
        public static bool IsText(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.PlainText,
                BrowserKind.Ini,
                BrowserKind.CSharp,
            }.Any(bk => bk == browserKind);
        }

        /// <summary>
        /// XML/HTMLとして有効か。
        /// </summary>
        /// <param name="browserKind"></param>
        /// <returns></returns>
        public static bool IsXmlHtml(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.Html,
                BrowserKind.Xml,
            }.Any(bk => bk == browserKind);
        }

        /// <summary>
        /// 画像ファイルとして有効か。
        /// </summary>
        /// <param name="browserKind"></param>
        /// <returns></returns>
        public static bool IsImage(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.Bmp,
                BrowserKind.Jpeg,
                BrowserKind.Png,
                BrowserKind.Gif,
            }.Any(bk => bk == browserKind);
        }

        /// <summary>
        /// プログラムとして有効か。
        /// <para>動きそうなバイナリ。</para>
        /// </summary>
        /// <param name="browserKind"></param>
        /// <returns></returns>
        public static bool IsProgram(this BrowserKind browserKind)
        {
            return new[] {
                BrowserKind.Application,
                BrowserKind.Dll,
            }.Any(bk => bk == browserKind);
        }
    }

}
