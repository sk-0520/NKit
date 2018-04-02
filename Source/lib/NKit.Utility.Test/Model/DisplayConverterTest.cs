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
            var testResult = dc.ToHexString(binary);
            Assert.IsTrue(testResult == result, $"{testResult} != {result} <{nameof(result)}>");
        }

        [TestMethod]
        [DataRow("0.00 byte", 0)]
        [DataRow("1.00 byte", 1)]
        [DataRow("100.00 byte", 100)]
        [DataRow("1023.00 byte", 1023)]
        [DataRow("1.00 KB", 1024)]
        [DataRow("2.00 KB", 2048)]
        [DataRow("10.00 KB", 10240)]
        [DataRow("20.00 KB", 20480)]
        [DataRow("1.00 MB", 1048576)]
        public void ToHumanLikeByteTest(string result, long size)
        {
            var dc = new DisplayConverter();
            var testResult = dc.ToHumanLikeByte(size);
            Assert.IsTrue(testResult == result, $"{testResult} != {result} <{nameof(result)}>");
        }
    }
}
