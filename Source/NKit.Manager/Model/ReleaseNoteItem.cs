using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public class ReleaseNoteItem
    {
        #region property

        public Version Version { get; set; }
        public string Hash { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }

        #endregion
    }
}
