using System;
using ContentTypeTextNet.NKit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NKit.Common.Link.Test
{
    [TestClass]
    public class CommonUtilityTest
    {
        static DateTime UtcTestTimestamp { get; } = new DateTime(2018, 3, 1, 13, 8, 7, 12, DateTimeKind.Utc);
        static DateTime LocalTestTimestamp { get; } = UtcTestTimestamp.ToLocalTime();

        [TestMethod]
        [DataRow("", "")]
        [DataRow("${", "${")]
        [DataRow("${}", "${}")]
        [DataRow("${yyyy}", "${yyyy}")]
        [DataRow("2018", "${YYYY:U}")]
        [DataRow("03", "${MM:U}")]
        [DataRow("01", "${DD:U}")]
        [DataRow("01", "${hh12:U}")]
        [DataRow("13", "${hh24:U}")]
        [DataRow("08", "${mm:U}")]
        [DataRow("07", "${ss:U}")]
        [DataRow("01", "${ff:U}")]
        [DataRow("012", "${fff:U}")]
        [DataRow("01", "${FF:U}")]
        [DataRow("012", "${FFF:U}")]
        public void ReplaceNKitTextTest(string result, string source)
        {
            var testResult = CommonUtility.ReplaceNKitText(source, UtcTestTimestamp, null);
            Assert.IsTrue(testResult == result, $"<{nameof(result)}> {result} != {testResult}");
        }
    }
}
