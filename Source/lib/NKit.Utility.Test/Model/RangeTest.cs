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
    public class RangeTest
    {
        [TestMethod]
        public void CompareTest()
        {
            var range = new Range<int>(5, 10);
            Assert.IsFalse(range.IsIn(4));
            Assert.IsTrue(range.IsIn(5));
            Assert.IsTrue(range.IsIn(8));
            Assert.IsTrue(range.IsIn(10));
            Assert.IsFalse(range.IsIn(11));
        }
    }
}
