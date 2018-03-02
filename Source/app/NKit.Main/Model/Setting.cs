using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.App;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class SettingBase : ModelBase, ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }

    public class WindowSetting : SettingBase
    {
        #region property

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        #endregion
    }

    public sealed class Setting : SettingBase
    {
        #region property

        public WindowSetting MainWindow { get; set; } = new WindowSetting();

        public FinderSetting Finder { get; set; } = new FinderSetting();

        public AppSetting Application { get; set; } = new AppSetting();

        #endregion

    }
}
