using System;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Setting.Finder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentTypeTextNet.NKit.Main.Test.Model.Finder
{
    [TestClass]
    public class FinderUtilityTest
    {
        [TestMethod]
        [DataRow(true, "a.txt", "*.txt")]
        [DataRow(false, "a.txt", "txt")]
        [DataRow(true, "a.txt", "*txt*")]
        [DataRow(true, "txt", "*txt*")]
        [DataRow(true, "txt", "txt")]
        [DataRow(false, "test.txt2", "*.txt")]
        [DataRow(true, "test.txt2", "*.txt|*.txt2")]
        public void CreateFileNameKinds_Text_Test(bool result, string input, string settingValue)
        {
            var setting = new FinderSetting() {
                TextFileNamePattern = settingValue,
            };

            var map = FinderUtility.CreateFileNameKinds(setting);
            var testResult = map[Define.FileNameKind.Text].IsMatch(input);
            Assert.IsTrue(testResult == result, $"<{nameof(input)}> {input}, {settingValue} <{nameof(settingValue)}>, {map[Define.FileNameKind.Text].ToString()}");
        }

        [TestMethod]
        [DataRow(true, "a.txt")]
        [DataRow(true, "a.ini")]
        [DataRow(false, "a.ini.bak")]
        [DataRow(false, "a.mp4")]
        public void CreateFileNameKinds_Default_Text_Test(bool result, string input)
        {
            var setting = new FinderSetting();

            var map = FinderUtility.CreateFileNameKinds(setting);
            var testResult = map[Define.FileNameKind.Text].IsMatch(input);
            Assert.IsTrue(testResult == result, $"<{nameof(input)}> {input}, {map[Define.FileNameKind.Text].ToString()}");

        }
    }
}
