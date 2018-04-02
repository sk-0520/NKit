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
    public class DisplayConverterTest
    {
        [TestMethod]
        [DataRow("", new byte[] {  })]
        [DataRow("00", new byte[] { 0x00 })]
        [DataRow("0001", new byte[] { 0x00, 0x01 })]
        [DataRow("0f", new byte[] { 0x0f })]
        public void ToHexStringTest(string result, byte[] binary)
        {
            var dc = new DisplayConverter();
            Assert.IsTrue(dc.ToHexString(binary) == result);
        }
    }
}
