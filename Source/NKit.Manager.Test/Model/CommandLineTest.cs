using System;
using System.Linq;
using ContentTypeTextNet.NKit.Manager.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NKit.Manager.Test.Model
{
    [TestClass]
    public class CommandLineTest
    {
        const int DUMMY = -1;

        [TestMethod]
        [DataRow(new[] { "a" }, "a")]
        public void ToCommandLineArgumentsTest(string[] result, string arg)
        {
            Assert.IsTrue(CommandLine.ToCommandLineArguments(arg).SequenceEqual(result));
        }

        [TestMethod]
        [DataRow(1, DUMMY, new[] { "a" })]
        [DataRow(1, DUMMY, new[] { "/a" })]
        [DataRow(1, DUMMY, new[] { "/a=1" })]
        [DataRow(2, DUMMY, new[] { "/a=1", "/b" })]
        [DataRow(2, DUMMY, new[] { "/a=1", "/b=2" })]
        [DataRow(3, DUMMY, new[] { "/a=1", "/b=2", "/c" })]
        [DataRow(3, DUMMY, new[] { "/a=1", "/b=2", "/c=3" })]
        [DataRow(4, DUMMY, new[] { "/a=1", "/b=2", "/c=3", "4" })]
        [DataRow(3, DUMMY, new[] { "/a=1", "/b=2", "\"/c=3 4\"" })]
        public void LengthTest(int test, int _dummy_, string[] values)
        {
            var cl = new CommandLine(values.ToArray());
            Assert.IsTrue(test == cl.Length);
        }

        [TestMethod]
        [DataRow(true, "a", new[] { "/a" })]
        [DataRow(true, "a", new[] { "/a=val" })]
        public void HasOptionTest(bool result, string option, params string[] values)
        {
            var cl = new CommandLine(values);
            Assert.IsTrue(cl.HasOption(option) == result);
        }


        [TestMethod]
        [DataRow(false, "a", new[] { "/a" })]
        [DataRow(true, "a", new[] { "/a=" })]
        [DataRow(true, "a", new[] { "/a=val" })]
        [DataRow(false, "a", new[] { "/a", "/a=val" })]
        public void HasValueTest(bool result, string option, params string[] values)
        {
            var cl = new CommandLine(values);
            Assert.IsTrue(cl.HasValue(option) == result);
        }
        [TestMethod]
        [DataRow(false, "a", 0, new[] { "/a" })]
        [DataRow(true, "a", 0, new[] { "/a=" })]
        [DataRow(true, "a", 0, new[] { "/a=val" })]
        [DataRow(false, "a", 0, new[] { "/a", "/a=val" })]
        [DataRow(true, "a", 1, new[] { "/a", "/a=val" })]
        [DataRow(true, "a", 1, new[] { "/a", "/b", "/a=val" })]
        public void HasValueTest(bool result, string option, int index, params string[] values)
        {
            var cl = new CommandLine(values);
            Assert.IsTrue(cl.HasValue(option, index) == result);
        }


        [TestMethod]
        [DataRow("1", "a", new[] { "/a=1", "/b=2", "/c=3", "4", "d=5", "/d=50" })]
        [DataRow("2", "b", new[] { "/a=1", "/b=2", "/c=3", "4", "d=5", "/d=50" })]
        [DataRow("3", "c", new[] { "/a=1", "/b=2", "/c=3", "4", "d=5", "/d=50" })]
        [DataRow("50", "d", new[] { "/a=1", "/b=2", "/c=3", "4", "d=5", "/d=50" })]
        [DataRow("abc", "a", new[] { "/a=abc" })]
        [DataRow("abc", "a", new[] { "/a=\"abc\"" })]
        [DataRow("abc", "a", new[] { "/a=abc", "/a=def" })]
        public void GetValueTest(string result, string option, params string[] values)
        {
            var cl = new CommandLine(values.ToArray());
            Assert.IsTrue(cl.GetValue(option) == result);
        }

        [TestMethod]
        [DataRow("abc", "a", 0, new[] { "/a=abc", "/a=def" })]
        [DataRow("def", "a", 1, new[] { "/a=abc", "/a=def" })]
        public void GetValuesTest(string result, string option, int index, params string[] values)
        {
            var cl = new CommandLine(values.ToArray());
            Assert.IsTrue(cl.GetValues(option).ElementAt(index) == result);
        }
    }
}
