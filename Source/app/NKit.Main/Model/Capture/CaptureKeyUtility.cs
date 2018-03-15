using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Setting;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public static class CaptureKeyUtility
    {
        #region function

        public static bool CanSendKeySetting(IReadOnlyKeySetting keySetting)
        {
            return keySetting.Key != Key.None || keySetting.ModifierKeys != ModifierKeys.None;
        }

        public static string ToCameramanArgumentKey(IReadOnlyKeySetting keySetting)
        {
            // TODO: 処理系依存
            var result = new List<string>();

            if(keySetting.Key != Key.None) {
                result.Add(keySetting.Key.ToString());
            }

            if(keySetting.ModifierKeys != ModifierKeys.None) {
                result.Add(keySetting.ModifierKeys.ToString());
            }

            return string.Join(",", result);
        }

        #endregion
    }
}
