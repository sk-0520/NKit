using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Common;

namespace ContentTypeTextNet.NKit.Setting
{
    public interface IReadOnlyKeySetting
    {
        /// <summary>
        /// 入力キー。
        /// </summary>
        Key Key { get; }
        /// <summary>
        /// 装飾子キー。
        /// </summary>
        ModifierKeys ModifierKeys { get; }
    }

    [Serializable, DataContract]
    public class KeySetting: SettingBase, IReadOnlyKeySetting
    {
        #region IReadOnlyKeySetting

        [DataMember]
        public Key Key { get; set; }
        [DataMember]
        public ModifierKeys ModifierKeys { get; set; }

        #endregion
    }
}
