using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting
{
    public class WindowSetting : SettingBase
    {
        #region property

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        #endregion

        #region function

        bool IsEnabledWindowValue(double value)
        {
            return !(double.IsNaN(value) || double.IsInfinity(value));
        }

        public bool Clamp(double left, double top, double width, double height)
        {
            bool isUpdated = false;

            if(!IsEnabledWindowValue(Left)) {
                Left = left;
                isUpdated = true;
            }
            if(!IsEnabledWindowValue(Top)) {
                Top = top;
                isUpdated = true;
            }

            if(!IsEnabledWindowValue(Width)) {
                Width = width;
                isUpdated = true;
            }
            if(!IsEnabledWindowValue(Height)) {
                Height = height;
                isUpdated = true;
            }

            return isUpdated;
        }

        #endregion
    }
}
