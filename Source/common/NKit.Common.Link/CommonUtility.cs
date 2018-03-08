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
            return GetRootDirectory("..", "..");
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
            if(Environment.ProcessorCount == 64) {
                name = "busybox64.exe";
            }
            var path = Path.Combine(binaryDirectory.FullName, BinaryBusyboxDirectoryName, name);
            return new FileInfo(path);
        }

        public static FileInfo GetRocketApplication(DirectoryInfo applicationDirectory)
        {
            var path = Path.Combine(applicationDirectory.FullName, "NKit.Rocket.exe");
            return new FileInfo(path);
        }

#endregion
    }
}
