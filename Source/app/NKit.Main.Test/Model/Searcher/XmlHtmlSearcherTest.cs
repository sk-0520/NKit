using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Searcher;
using ContentTypeTextNet.NKit.Setting.Finder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentTypeTextNet.NKit.Main.Test.Model.Searcher
{
    [TestClass]
    public class XmlHtmlSearcherTest
    {
        Encoding Encoding { get; } = Encoding.Unicode;
        Stream ToStream(string text)
        {
            return new MemoryStream(Encoding.GetBytes(text));
        }

        [TestMethod]
        public void SearchXmlElementTest()
        {
            var setting = new FindXmlHtmlContentSetting() {
                Element = true,
                Text = false,
                AttributeKey = false,
                AttributeValue = false,
                Comment = false,
                IgnoreHtmlLinkValue = false,
            };
            var regex = new Regex("element");
            var searcher = new XmlHtmlSearcher();

            var xml1 = @"<root></root>";
            using(var stream = ToStream(xml1)) {
                var result = searcher.Search(stream, regex, Encoding, setting);
                Assert.IsFalse(result.IsMatched);
            }

            var xml2 = @"<root><element /></root>";
            using(var stream = ToStream(xml2)) {
                var result = searcher.Search(stream, regex, Encoding, setting);
                Assert.IsTrue(result.IsMatched);
            }

            var xml3 = @"<root><element></element></root>";
            using(var stream = ToStream(xml3)) {
                var result = searcher.Search(stream, regex, Encoding, setting);
                Assert.IsTrue(result.IsMatched);
            }

            var xml4 = @"<root>element</root>";
            using(var stream = ToStream(xml4)) {
                var result = searcher.Search(stream, regex, Encoding, setting);
                Assert.IsFalse(result.IsMatched);
            }

            var xml5 = @"<element>element</element>";
            using(var stream = ToStream(xml5)) {
                var result = searcher.Search(stream, regex, Encoding, setting);
                Assert.IsTrue(result.IsMatched);
            }
        }
    }
}
