using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ContentTypeTextNet.NKit.Common
{
    internal static class CommonUtility
    {
        #region property

        static string LibraryDirectoryName { get; } = "lib";
        static string BinaryDirectoryName { get; } = "bin";
        static string ApplicationDirectoryName { get; } = "app";
        static string EtcDirectoryName { get; } = "etc";

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


        #endregion
    }
}
