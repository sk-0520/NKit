using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class DisplayConverter
    {
        #region property

        string[] Terms { get; set; } = new[] { "byte", "KB", "MB", "GB", "TB" };

        #endregion

        #region function

        public string ToHexString(byte[] binary)
        {
            if(binary == null) {
                return string.Empty;
            }

            return BitConverter.ToString(binary).ToLower().Replace("-", "");
        }

        public string ToHumanLikeByte(long byteSize, string sizeFormat, string[] terms)
        {
            double size = byteSize;
            int order = 0;
            while(size >= 1024 && ++order < terms.Length) {
                size = size / 1024;
            }

            return string.Format(sizeFormat, size, terms[order]);
        }

        public string ToHumanLikeByte(long byteSize, string[] terms)
        {
            return ToHumanLikeByte(byteSize, "{0:0.00} {1}", terms);
        }

        public string ToHumanLikeByte(long byteSize, string sizeFormat)
        {
            return ToHumanLikeByte(byteSize, sizeFormat, Terms);
        }

        public string ToHumanLikeByte(long byteSize)
        {
            return ToHumanLikeByte(byteSize, Terms);
        }

        #endregion
    }
}
