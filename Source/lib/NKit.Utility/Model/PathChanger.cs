using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public class PathChanger
    {
        #region function

        public string GetUpDirectoryPath(string path)
        {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException(path);
            }

            var result = Path.GetDirectoryName(path);
            if(string.IsNullOrEmpty(result)) {
                return path;
            }

            return result;
        }

        #endregion
    }
}
