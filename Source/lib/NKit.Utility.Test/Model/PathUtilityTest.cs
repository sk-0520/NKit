using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentTypeTextNet.NKit.Utility.Test.Model
{
    [TestClass]
    public class PathUtilityTest
    {
        [DataRow("a.txt", "a", "txt")]
        [DataRow("a.txt.txt", "a.txt", "txt")]
        [DataRow("a..txt", "a.", "txt")]
        [DataRow("a..txt", "a", ".txt")]
        public void AppendExtensionTest(string result, string path, string ext)
        {
            Assert.AreEqual(result, PathUtility.AppendExtension(path, ext));
        }

        [DataRow("", "", "!")]
        [DataRow("", " ", "!")]
        [DataRow("a", "a", "!")]
        [DataRow("a!", "a?", "!")]
        [DataRow("a?", "a?", "?")]
        [DataRow("a@b@c@d", "a?b\\c*d", "@")]
        [DataRow("a<>b<>c<>d", "a?b\\c*d", "<>")]
        public void ToSafeNameTest(string result, string value, string c)
        {
            Assert.AreEqual(result, PathUtility.ToSafeName(value, v => c));
        }

        [DataRow("", "")]
        [DataRow("", " ")]
        [DataRow("a", "a")]
        [DataRow("a_", "a?")]
        [DataRow("a_", "a?")]
        [DataRow("a_b_c_d", "a?b\\c*d")]
        public void ToSafeNameDefaultTest(string result, string value)
        {
            Assert.AreEqual(result, PathUtility.ToSafeNameDefault(value));
        }

        [DataRow(false, "exe")]
        [DataRow(false, "dll")]
        [DataRow(true, ".exe")]
        [DataRow(true, ".dll")]
        [DataRow(false, ".ico")]
        [DataRow(true, "a.exe")]
        [DataRow(true, "a.dll")]
        [DataRow(false, "a.ico")]
        public void HasIconTest(bool result, string value)
        {
            Assert.AreEqual(result, PathUtility.HasIconPath(value));
        }
    }
}
