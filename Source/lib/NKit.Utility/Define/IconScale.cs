using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ContentTypeTextNet.NKit.Utility.Define
{
    /// <summary>
    /// アイコンサイズ。
    /// </summary>
    [DataContract]
    public enum IconScale
    {
        /// <summary>
        /// 16px
        /// </summary>
        [EnumMember]
        Small = 16,
        /// <summary>
        /// 32px
        /// </summary>
        [EnumMember]
        Normal = 32,
        /// <summary>
        /// 48px
        /// </summary>
        [EnumMember]
        Big = 48,
        /// <summary>
        /// 256px
        /// </summary>
        [EnumMember]
        Large = 256,
    }

    public static class IconScaleExtensions
    {
        #region function

        public static Size ToSize(this IconScale iconScale)
        {
            var size = (int)iconScale;

            return new Size(size, size);
        }

        #endregion
    }

}
