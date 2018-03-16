using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Cameraman
{
    partial class Constants
    {
        #region variable

        static ConfigurationCacher appConfig = new ConfigurationCacher();

        #endregion

        #region proeprty

        public static TimeSpan ShotDelayTime => appConfig.Get("shot-delay-time", s => TimeSpan.Parse(s));

        public static Color CameraBorderColor => appConfig.Get("camera-border-color", s => ColorTranslator.FromHtml(s));
        public static int CameraBorderWidth => appConfig.Get("camera-border-width", s => int.Parse(s));

        public static TimeSpan ScrollDelayTime => appConfig.Get("scroll-delay-time", s => TimeSpan.Parse(s));
        public static TimeSpan ScrollInternetExplorerInitializeTime => appConfig.Get("scroll-internet-explorer-initialize-time", s => TimeSpan.Parse(s));

        public static bool ScrollInternetExplorerDebug => appConfig.Get("scroll-internet-explorer-debug", s => bool.Parse(s));

        public static string HideHeaderTagClassItems => appConfig.Get("hide-header-tag-class-items");
        public static string HideFooterTagClassItems => appConfig.Get("hide-footer-tag-class-items");

        #endregion
    }
}
