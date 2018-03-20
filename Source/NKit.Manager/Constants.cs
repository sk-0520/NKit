using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager
{
    public static partial class Constants
    {
        #region property

        public static string ArchiveDirectoryName { get; } = "archive";
        public static string ExtractDirectoryName { get; } = "extract";
        public static string BackupFileName { get; } = "backup.zip";
        public static string WorkspaceLockFile { get; } = "workspace.lock";

        #endregion
    }
}
