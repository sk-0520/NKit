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
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 10)]
        [DataRow(3, 100)]
        [DataRow(1, -1)]
        [DataRow(2, -10)]
        [DataRow(3, -100)]
        public void GetNumberWidthTest(int result, int value)
        {
            var uc = new UnitConverter();
            Assert.IsTrue(uc.GetNumberWidth(value) == result);
        }
    }
}
