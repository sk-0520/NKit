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
        public void TextWidthTest()
        {
            var tests = new[] {
                new { Text = "", Result = 0 },
                new { Text = default(string), Result = 0 },
                new { Text = "1", Result = 1 },
                new { Text = "22", Result = 2 },
                new { Text = "あ", Result = 1 },
            };
            foreach(var test in tests) {
                Assert.IsTrue(TextUtility.TextWidth(test.Text) == test.Result);
            }
        }
    }
}
