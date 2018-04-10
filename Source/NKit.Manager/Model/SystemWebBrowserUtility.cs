using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using Microsoft.Win32;

namespace ContentTypeTextNet.NKit.Manager.Model
{
    public static class SystemWebBrowserUtility
    {
        #region define

        const string webbrowserEmulationPath = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
        const string webbrowserRenderingPath = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_DOCUMENT_COMPATIBLE_MODE";
        const int webbrowserDefaultVersion = 7000;
        const string internetExplorerPath = @"Software\Microsoft\Internet Explorer";
        const string internetExplorer10Key = "svcVersion";
        const string internetExplorer9Key = "Version";

        #endregion

        #region function

        static int ToIEVersion(Version version)
        {
            // TODO: 小数点以下は要調査
            var versionNumber = version.Major * 1000;
            return versionNumber;
        }

        /// <summary>
        /// .NET Frameworkで使用するIEバージョンを設定する。
        /// </summary>
        /// <param name="version">IEバージョン<para>https://msdn.microsoft.com/en-us/library/ee330730%28v=vs.85%29.aspx#browser_emulation</para></param>
        /// <param name="programName">対象とするプログラムのファイル名</param>
        public static void SetUsingBrowserVersion(int version, string programName)
        {
            if(version == webbrowserDefaultVersion) {
                ResetUsingBrowserVersion(programName);
            } else {
                using(var key = Registry.CurrentUser.CreateSubKey(webbrowserEmulationPath)) {
                    key.SetValue(programName, version, RegistryValueKind.DWord);
                }
                using(var key = Registry.CurrentUser.CreateSubKey(webbrowserRenderingPath)) {
                    key.SetValue(programName, version, RegistryValueKind.DWord);
                }
            }
        }
        public static void SetUsingBrowserVersion(Version version, string programName)
        {
            SetUsingBrowserVersion(ToIEVersion(version), programName);
        }

        static string GetExecutingAssemblyFileName()
        {
            var path = Assembly.GetEntryAssembly().Location;
            var name = Path.GetFileName(path);

            return name;
        }

#if DEBUG
        static string GetDebugName(string programName)
        {
            var ext = Path.GetExtension(programName);
            return Path.ChangeExtension(programName, "vshost" + ext);
        }
#endif
        static IEnumerable<string> GetBrowserControlUseProgram()
        {
            return new[] {
                GetExecutingAssemblyFileName(),
                CommonUtility.GetMainApplication(CommonUtility.GetApplicationDirectory()).Name,
                CommonUtility.GetJustLookingApplication(CommonUtility.GetApplicationDirectory()).Name,
            };
        }

        /// <summary>
        /// 現在実行中のアセンブリに対して.NET Frameworkで使用するIEバージョンを設定する。
        /// <para>デバッグバージョンであればvshostも対象とする</para>
        /// </summary>
        /// <param name="version">IEバージョン<para>https://msdn.microsoft.com/en-us/library/ee330730%28v=vs.85%29.aspx#browser_emulation</para></param>
        public static void SetUsingBrowserVersionForExecutingAssembly(int version)
        {
            foreach(var name in GetBrowserControlUseProgram()) {
                SetUsingBrowserVersion(version, name);
#if DEBUG
                SetUsingBrowserVersion(version, GetDebugName(name));
#endif

            }
        }
        public static void SetUsingBrowserVersionForExecutingAssembly(Version version)
        {
            SetUsingBrowserVersionForExecutingAssembly(ToIEVersion(version));
        }
        /// <summary>
        /// .NET Frameworkで使用するIEバージョンを初期状態に戻す。
        /// </summary>
        /// <param name="programName">対象とするプログラムのファイル名</param>
        public static void ResetUsingBrowserVersion(string programName)
        {
            using(var key = Registry.CurrentUser.OpenSubKey(webbrowserEmulationPath, true)) {
                if(key != null) {
                    key.DeleteValue(programName, false);
                }
            }
            using(var key = Registry.CurrentUser.OpenSubKey(webbrowserRenderingPath, true)) {
                if(key != null) {
                    key.DeleteValue(programName, false);
                }
            }
        }

        /// <summary>
        /// 現在実行中のアセンブリに対して.NET Frameworkで使用するIEバージョンを初期状態に戻す。
        /// </summary>
        public static void ResetUsingBrowserVersionForExecutingAssembly()
        {
            foreach(var name in GetBrowserControlUseProgram()) {
                ResetUsingBrowserVersion(name);
#if DEBUG
                ResetUsingBrowserVersion(GetDebugName(name));
#endif
            }
        }

        public static Version GetInternetExplorerVersion()
        {
            using(var key = Registry.LocalMachine.OpenSubKey(internetExplorerPath)) {
                if(key == null) {
                    return new Version(7, 0);
                } else {
                    var rawVersion = (string)(key.GetValue(internetExplorer10Key) ?? key.GetValue(internetExplorer9Key) ?? "7.0");
                    return new Version(rawVersion);
                }
            }
        }

        #endregion
    }
}
