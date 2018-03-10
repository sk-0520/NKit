using System;
using System.Collections.Generic;
using System.Text;

namespace ContentTypeTextNet.NKit.Common
{
    public static class ProgramRelationUtility
    {
        #region function

        public static string EscapesequenceToArgument(string s)
        {
            if(s == null) {
                throw new ArgumentNullException(nameof(s));
            }

            if(s.IndexOf(' ') == -1) {
                return s;
            }

            // とりあえずスペースだけ対応。
            return "\"" + s + "\"";
        }


        #endregion
    }
}
