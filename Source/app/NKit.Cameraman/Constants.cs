using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Cameraman
{
    internal static partial class Constants
    {
        #region variable

        static ConfigurationCacher appConfig = new ConfigurationCacher();

        #endregion

        #region proeprty

        public static Color CameraBorderColor => appConfig.Get("camera-border-color", s => ColorTranslator.FromHtml(s));
        public static int CameraBorderWidth => appConfig.Get("camera-border-width", s => int.Parse(s));

        public static TimeSpan ScrollDelayTime => appConfig.Get("scroll-delay-time", s => TimeSpan.Parse(s));
        public static TimeSpan ScrollInternetExplorerInitializeTime => appConfig.Get("scroll-internet-explorer-initialize-time", s => TimeSpan.Parse(s));

        #endregion
    }
}
