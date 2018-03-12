using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Manager
{
    partial class Constants
    {
        #region property

        public static string UpdateCheckBranchBaseUri => ConfigurationManager.AppSettings["update-check-branch-base-uri"];
        public static string UpdateCheckBranchTargetName => ConfigurationManager.AppSettings["update-check-branch-target-name"];
        public static string UpdateCheckBranchHashPattern => ConfigurationManager.AppSettings["update-check-branch-hash-pattern"];
        public static string UpdateCheckBranchHashPatternOptions => ConfigurationManager.AppSettings["update-check-branch-hash-pattern-options"];
        public static string UpdateCheckBranchHashPatternKey => ConfigurationManager.AppSettings["update-check-branch-hash-pattern-key"];
        public static string UpdateCheckVersionBaseUri => ConfigurationManager.AppSettings["update-check-version-base-uri"];
        public static string UpdateCheckVersionFilePath => ConfigurationManager.AppSettings["update-check-version-file-path"];
        public static string UpdateCheckVersionFilePattern => ConfigurationManager.AppSettings["update-check-version-file-pattern"];
        public static string UpdateCheckVersionFilePatternOptions => ConfigurationManager.AppSettings["update-check-version-file-pattern-options"];
        public static string UpdateCheckVersionFilePatternKey => ConfigurationManager.AppSettings["update-check-version-file-pattern-key"];
        public static string UpdateCheckDownloadBaseUri => ConfigurationManager.AppSettings["update-check-download-base-uri"];
        public static string UpdateCheckDownloadNameFormat => ConfigurationManager.AppSettings["update-check-download-name-format"];
        public static string UpdateCheckDownloadUriPattern => ConfigurationManager.AppSettings["update-check-download-uri-pattern"];
        public static string UpdateCheckDownloadUriPatternOptions => ConfigurationManager.AppSettings["update-check-download-uri-pattern-options"];
        public static string UpdateCheckDownloadUriPatternKey => ConfigurationManager.AppSettings["update-check-download-uri-pattern-key"];
        public static string UpdateReleaseNoteBaseUri => ConfigurationManager.AppSettings["update-release-note-base-uri"];
        public static string UpdateReleaseNoteFilePathFormat => ConfigurationManager.AppSettings["update-release-note-file-path-format"];

        #endregion
    }
}
