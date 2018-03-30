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
    public class UnitConverterTest
    {
        [TestMethod]
        public void GetNumberWidthTest()
        {
            var tests = new[] {
                new { Value = 0, Result = 1 },
                new { Value = 1, Result = 1 },
                new { Value = 10, Result = 2 },
                new { Value = 100, Result = 3 },
                new { Value = -1, Result = 1 },
                new { Value = -10, Result = 2 },
                new { Value = -100, Result = 3 },
            };

            var uc = new UnitConverter();
            foreach(var test in tests) {
                Assert.IsTrue(uc.GetNumberWidth(test.Value) == test.Result);
            }
        }
    }
}
