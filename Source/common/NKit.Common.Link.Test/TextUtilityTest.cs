using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NKit.Common.Link.Test
{
    [TestClass]
    public class TextUtilityTest
    {
        [TestMethod]
        [DataRow("a", "a", new[] { "" })]
        [DataRow("a", "a", new[] { "b" })]
        [DataRow("a(2)", "a", new[] { "a" })]
        [DataRow("A", "A", new[] { "A(2)" })]
        [DataRow("a(3)", "a", new[] { "a(5)", "a(2)", "a(4)", "a" })]
        public void ToUniqueDefaultTest(string result, string src, params string[] list)
        {
            Assert.IsTrue(TextUtility.ToUniqueDefault(src, list, StringComparison.Ordinal) == result);
        }

        [TestMethod]
        [DataRow(0, "")]
        [DataRow(0, default(string))]
        [DataRow(1, "1")]
        [DataRow(2, "22")]
        [DataRow(1, "あ")]
        public void TextWidthTest(int result, string text)
        {
            Assert.IsTrue(TextUtility.TextWidth(text) == result);
        }
    }
}
