using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ContentTypeTextNet.NKit.Common
{
    internal static class CommonUtility
    {
        #region property

        static string LibraryDirectoryName { get; } = "lib";
        static string BinaryDirectoryName { get; } = "bin";
        static string ApplicationDirectoryName { get; } = "app";
        static string EtcDirectoryName { get; } = "etc";
        static string DocumentDirectoryName { get; } = "doc";

        static string BinaryBusyboxDirectoryName { get; } = "busybox";

        public static string BuildType
        {
            get
            {
                return
#if DEBUG
                    "DEBUG"
#elif BETA
                    "BETA"
#else
                    "RELEASE"
#endif
                ;
            }
        }

        public static string BuildTypeInformation
        {
            get
            {
                return
#if DEBUG
                    BuildType
#elif BETA
                    BuildType
#else
                    string.Empty
#endif
                ;
            }
        }

        public static string BuildTypeAppendValue
        {
            get
            {
                return
#if DEBUG
                    "-debug"
#elif BETA
                    "-beta"
#else
                    string.Empty
#endif
                ;
            }
        }

        public static string ProjectName => "NKit";

        public static string EnvironmentKeyUserDirectory => "NKIT_USER_DIR";
        public static string EnvironmentKeyDataDirectory => "NKIT_DATA_DIR";


        public static string AppAddress => "app";
        public static string LogAddress => "log";

        public static string WorkspaceSettingDirectoryName => "settings";
        public static string WorkspaceLogDirectoryName => "logs";
        public static string WorkspaceTemporaryDirectoryName => "temporary";

        public struct ManagedStartup
        {
            #region property

            /// <summary>
            /// 管理用フラグ。
            /// <para><seealso cref="Microsoft.Extensions.CommandLineUtils.CommandLineApplication"/>が謎パラメータ受け取ると解析終わってまうんすよ。</para>
            /// </summary>
            public static string ExecuteFlag => "--nkit_managed_execute";
            /// <summary>
            /// すぐ死んでほしい場合につけておくとすぐ死ぬ。
            /// </summary>
            public static string LetsDie => "--nkit_lets_die";
            /// <summary>
            /// マネージャのサービスエントリポイント。
            /// </summary>
            public static string ServiceUri => "--nkit_service_uri";
            /// <summary>
            /// マネージャのアプリケーションID(タスクバーボタン結合用)。
            /// </summary>
            public static string ApplicationId => "--nkit_application_id";
            /// <summary>
            /// 現在処理中のワークスペースディレクトリパス。
            /// </summary>
            public static string WorkspacePath => "--nkit_workspace";
            /// <summary>
            /// マネージャから送られてくる集団自殺示唆イベント。
            /// <para>このシグナルが立ったら問答無用で死ぬこと。</para>
            /// <para>ワークスペースの終了なんかに使用する。</para>
            /// </summary>
            public static string GroupSuicideEventName => "--nkit_group_suicide_event_name";
            /// <summary>
            /// マネージャから送られてくる単独自殺示唆イベント。
            /// <para>このシグナルが立ったら問答無用で死ぬこと。</para>
            /// <para>孤独にひっそりと死ぬ。</para>
            /// </summary>
            public static string AloneSuicideEventName => "--nkit_alone_suicide_event_name";

            #endregion
        }

        #endregion

        #region function

        static string GetOverwriteEnvironmentVariable(string customKey, string defaultValue)
        {
            var customValue = Environment.GetEnvironmentVariable(customKey);
            if(string.IsNullOrWhiteSpace(customValue)) {
                return defaultValue;
            }

            return customValue;
        }

        public static DirectoryInfo GetUserDirectory()
        {
            var envKey = GetOverwriteEnvironmentVariable(EnvironmentKeyUserDirectory, "%APPDATA%");
            var path = Path.Combine(envKey, ProjectName + BuildTypeAppendValue);
            var expandedPath = Environment.ExpandEnvironmentVariables(path);
            return Directory.CreateDirectory(expandedPath);
        }

        public static DirectoryInfo GetDataDirectory()
        {
            var envKey = GetOverwriteEnvironmentVariable(EnvironmentKeyDataDirectory, "%LOCALAPPDATA%");
            var path = Path.Combine(envKey, ProjectName + BuildTypeAppendValue);
            var expandedPath = Environment.ExpandEnvironmentVariables(path);
            return Directory.CreateDirectory(expandedPath);
        }

        public static DirectoryInfo GetRootDirectory(params string[] args)
        {
            var executeFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var paths = new List<string>();
            paths.Add(executeFilePath);
            paths.AddRange(args);
            var libDirPath = Path.Combine(paths.ToArray());
            var absPath = Path.GetFullPath(libDirPath);
            return new DirectoryInfo(absPath);
        }

        public static DirectoryInfo GetRootDirectoryForApplication()
        {
            return GetRootDirectory("..");
        }

        public static DirectoryInfo GetLibraryDirectory(params string[] args)
        {
            var libDirPath = Path.Combine(GetRootDirectory(args).FullName, LibraryDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetLibraryDirectoryForApplication()
        {
            var libDirPath = Path.Combine(GetRootDirectoryForApplication().FullName, LibraryDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetBinaryDirectory(params string[] args)
        {
            var libDirPath = Path.Combine(GetRootDirectory(args).FullName, BinaryDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetBinaryDirectoryForApplication()
        {
            var libDirPath = Path.Combine(GetRootDirectoryForApplication().FullName, BinaryDirectoryName);
            return new DirectoryInfo(libDirPath);
        }


        public static DirectoryInfo GetApplicationDirectory(params string[] args)
        {
            var libDirPath = Path.Combine(GetRootDirectory(args).FullName, ApplicationDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetApplicationDirectoryForApplication()
        {
            var libDirPath = Path.Combine(GetRootDirectoryForApplication().FullName, ApplicationDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetEtcDirectory(params string[] args)
        {
            var libDirPath = Path.Combine(GetRootDirectory(args).FullName, EtcDirectoryName);
            return new DirectoryInfo(libDirPath);
        }
        public static DirectoryInfo GetEtcDirectoryForApplication()
        {
            var libDirPath = Path.Combine(GetRootDirectoryForApplication().FullName, EtcDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static DirectoryInfo GetDocumentDirectory(params string[] args)
        {
            var libDirPath = Path.Combine(GetRootDirectory(args).FullName, DocumentDirectoryName);
            return new DirectoryInfo(libDirPath);
        }
        public static DirectoryInfo GetDocumentDirectoryForApplication()
        {
            var libDirPath = Path.Combine(GetRootDirectoryForApplication().FullName, DocumentDirectoryName);
            return new DirectoryInfo(libDirPath);
        }

        public static FileInfo GetBusyBox(bool usePlatformBusyBox, DirectoryInfo binaryDirectory)
        {
            var name = "busybox.exe";
            if(Environment.Is64BitOperatingSystem) {
                name = "busybox64.exe";
            }
            var path = Path.Combine(binaryDirectory.FullName, BinaryBusyboxDirectoryName, name);
            return new FileInfo(path);
        }

        public static FileInfo GetMainApplication(DirectoryInfo applicationDirectory)
        {
            var path = Path.Combine(applicationDirectory.FullName, "NKit.Main.exe");
            return new FileInfo(path);
        }

        public static FileInfo GetRocketApplication(DirectoryInfo applicationDirectory)
        {
            var path = Path.Combine(applicationDirectory.FullName, "NKit.Rocket.exe");
            return new FileInfo(path);
        }

        public static FileInfo GetCameramanApplication(DirectoryInfo applicationDirectory)
        {
            var path = Path.Combine(applicationDirectory.FullName, "NKit.Cameraman.exe");
            return new FileInfo(path);
        }



        static string ReplaceNKitTextCore(string source, DateTime utcTimestamp, IReadOnlyDictionary<string, string> customMap)
        {
            var localTimestamp = utcTimestamp.ToLocalTime();

            KeyValuePair<string, string> kvp(string k, string v) => new KeyValuePair<string, string>(k, v);
            IEnumerable<KeyValuePair<string, string>> CreateNKitTimestampFormatFromClrFromat(string nkitFormat, string clrFormat)
            {
                var local = localTimestamp.ToString(clrFormat);
                return new[] {
                    kvp(nkitFormat, local),
                    kvp(nkitFormat + ":" + "L", local),
                    kvp(nkitFormat + ":" + "U", utcTimestamp.ToString(clrFormat))
                };
            }

            var timstampFormats = new[] {
                new { NKit = "Y", Clr = "y" },
                new { NKit = "YY", Clr = "yy" },
                new { NKit = "YYY", Clr = "yyy" },
                new { NKit = "YYYY", Clr = "yyyy" },

                new { NKit = "M", Clr = "M" },
                new { NKit = "MM", Clr = "MM" },
                new { NKit = "MMM", Clr = "MMM" },
                new { NKit = "MMMM", Clr = "MMMM" },

                new { NKit = "D", Clr = "d" },
                new { NKit = "DD", Clr = "dd" },
                new { NKit = "DDD", Clr = "ddd" },

                new { NKit = "h12", Clr = "h" },
                new { NKit = "hh12", Clr = "hh" },
                new { NKit = "h24", Clr = "H" },
                new { NKit = "hh24", Clr = "HH" },

                new { NKit = "m", Clr = "m" },
                new { NKit = "mm", Clr = "mm" },

                new { NKit = "s", Clr = "s" },
                new { NKit = "ss", Clr = "ss" },

                new { NKit = "f", Clr = "f" },
                new { NKit = "ff", Clr = "ff" },
                new { NKit = "fff", Clr = "fff" },

                new { NKit = "F", Clr = "F" },
                new { NKit = "FF", Clr = "FF" },
                new { NKit = "FFF", Clr = "FFF" },
            }.SelectMany(i => CreateNKitTimestampFormatFromClrFromat(i.NKit, i.Clr))
            ;

            var map = timstampFormats.ToDictionary(
                kv => kv.Key,
                kv => kv.Value
            );

            if(customMap != null && customMap.Any()) {
                foreach(var pair in customMap) {
                    map[pair.Key] = pair.Value;
                }
            }

            var regex = new Regex(@"\${(\.+?)}");
            return regex.Replace(source, m => {
                if(!m.Success) {
                    return m.Value;
                }
                var key = m.Groups[0].Value;
                if(string.IsNullOrEmpty(key)) {
                    return m.Value;
                }

                if(map.TryGetValue(key, out var value)) {
                    return value;
                }

                return key;
            });
        }

        /// <summary>
        /// 共通的に使用する文字列置き換え処理。
        /// <para>やってることは <see cref="ContentTypeTextNet.NKit.Utility.Model.TextUtility.ReplaceFromDictionary"/> と一緒だけどマネージャがアセンブリ参照できない。</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customMap"></param>
        /// <param name="utcTimestamp"></param>
        /// <returns></returns>
        public static string ReplaceNKitText(string source, DateTime utcTimestamp, IReadOnlyDictionary<string, string> customMap = null)
        {
            if(source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if(utcTimestamp.Kind != DateTimeKind.Utc) {
                throw new ArgumentException($"{nameof(utcTimestamp)}.{nameof(utcTimestamp.Kind)} only {nameof(DateTimeKind)}.{nameof(DateTimeKind.Utc)}");
            }

            if(string.IsNullOrWhiteSpace(source)) {
                return source;
            }
            if(source.IndexOf('$') == -1) {
                return source;
            }

            return ReplaceNKitText(source, utcTimestamp, customMap);
        }

        #endregion
    }
}
