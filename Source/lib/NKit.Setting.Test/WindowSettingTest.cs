using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Setting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NKit.Setting.Test
{
    [TestClass]
    public class WindowSettingTest
    {
        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(1, double.NaN, 1)]
        [DataRow(1, 1, double.NaN)]
        public void Clamp_Left_Test(double result, double setting, double input)
        {
            var ws = new WindowSetting() {
                Left = setting,
            };
            ws.Clamp(input, 0, 0, 0);
            Assert.IsTrue(ws.Left == result);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(1, double.NaN, 1)]
        [DataRow(1, 1, double.NaN)]
        public void Clamp_Top_Test(double result, double setting, double input)
        {
            var ws = new WindowSetting() {
                Top = setting,
            };
            ws.Clamp(0, input, 0, 0);
            Assert.IsTrue(ws.Top == result);
        }


        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(1, double.NaN, 1)]
        [DataRow(1, 1, double.NaN)]
        public void Clamp_Width_Test(double result, double setting, double input)
        {
            var ws = new WindowSetting() {
                Width = setting,
            };
            ws.Clamp(0, 0, input, 0);
            Assert.IsTrue(ws.Width == result);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(1, double.NaN, 1)]
        [DataRow(1, 1, double.NaN)]
        public void Clamp_Height_Test(double result, double setting, double input)
        {
            var ws = new WindowSetting() {
                Height = setting,
            };
            ws.Clamp(0, 0, 0, input);
            Assert.IsTrue(ws.Height == result);
        }
    }
}
