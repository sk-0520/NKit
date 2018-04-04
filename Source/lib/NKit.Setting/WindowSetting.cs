using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting
{
    public interface IReadOnlyWindowSetting
    {
        #region property

        [DataMember]
        double Left { get; }
        double Top { get; }
        double Width { get; }
        double Height { get; }

        #endregion
    }

    [Serializable, DataContract]
    public class WindowSetting : SettingBase, IReadOnlyWindowSetting
    {
        #region IReadOnlyWindowSetting

        [DataMember]
        public double Left { get; set; }
        [DataMember]
        public double Top { get; set; }
        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }

        #endregion

        #region function

        bool IsEnabledWindowValue(double value)
        {
            return !(double.IsNaN(value) || double.IsInfinity(value));
        }

        bool IsEnabledWindowLocationValue(double value)
        {
            return IsEnabledWindowValue(value);
        }

        bool IsEnabledWindowSizeValue(double value)
        {
            if(IsEnabledWindowValue(value)) {
                return 0 < value;
            }

            return false;
        }

        public bool Clamp(double left, double top, double width, double height)
        {
            bool isUpdated = false;

            if(!IsEnabledWindowLocationValue(Left)) {
                Left = left;
                isUpdated = true;
            }
            if(!IsEnabledWindowLocationValue(Top)) {
                Top = top;
                isUpdated = true;
            }

            if(!IsEnabledWindowSizeValue(Width)) {
                Width = width;
                isUpdated = true;
            }
            if(!IsEnabledWindowSizeValue(Height)) {
                Height = height;
                isUpdated = true;
            }

            return isUpdated;
        }

        #endregion
    }
}
