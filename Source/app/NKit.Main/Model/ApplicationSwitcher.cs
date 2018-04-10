using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model
{
    public class ApplicationSwitcher : ModelBase
    {
        #region define

        class LocalApplicationInfo
        {
            public NKitApplicationKind TargetApplication { get; set; }
            public string Arguments { get; set; }
            public string WorkingDirectoryPath { get; set; }
            public ActionCliApplicationExecutor Executor { get; set; }
        }

        #endregion

        public ApplicationSwitcher(Uri serviceUri)
        {
            if(serviceUri != null) {
                ApplicationClient = new NKitApplicationTalkerClient(NKitApplicationKind.Main, serviceUri);
            }
        }

        #region property

        NKitApplicationTalkerClient ApplicationClient { get; }
        NKitTalkerSwicher Swicther { get; } = new NKitTalkerSwicher();

        IDictionary<uint, LocalApplicationInfo> LocalApplicationInfos { get; } = new Dictionary<uint, LocalApplicationInfo>();

        #endregion

        #region function

        public void Initialize()
        {
            if(ApplicationClient != null) {
                ApplicationClient.Open();
            }
        }

        static string GetApplicationPath(NKitApplicationKind kind)
        {
            switch(kind) {
                case NKitApplicationKind.Main:
                    return CommonUtility.GetMainApplication(CommonUtility.GetApplicationDirectoryForApplication()).FullName;

                case NKitApplicationKind.Rocket:
                    return CommonUtility.GetRocketApplication(CommonUtility.GetApplicationDirectoryForApplication()).FullName;

                case NKitApplicationKind.Cameraman:
                    return CommonUtility.GetCameramanApplication(CommonUtility.GetApplicationDirectoryForApplication()).FullName;

                case NKitApplicationKind.JustLooking:
                    return CommonUtility.GetJustLookingApplication(CommonUtility.GetApplicationDirectoryForApplication()).FullName;

                default:
                    throw new NotImplementedException();
            }
        }

        public uint PreparateApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            var manageId = 0u;

            Swicther.DoSwitch(
                ApplicationClient,
                timestamp => {
                    manageId = ApplicationClient.PreparateApplication(targetApplication, arguments, workingDirectoryPath);
                },
                (timestamp, talkerException) => {
                    // あくまで起動準備させるだけで管理まではしない
                    // どうしようもないしね
                    if(talkerException != null) {
                        Logger.Error(talkerException);
                    }

                    var appPath = GetApplicationPath(targetApplication);
                    var executor = new ActionCliApplicationExecutor(appPath, arguments) {
                        ReceivedOutput = e => {
                            Log.Out.Debug(e.Data);
                        },
                        ReceivedError = e => {
                            Log.Out.Error(e.Data);
                        }
                    };
                    Logger.Information($"local execute: {targetApplication}, {arguments}");

                    var info = new LocalApplicationInfo() {
                        Arguments = arguments,
                        TargetApplication = targetApplication,
                        WorkingDirectoryPath = workingDirectoryPath,
                        Executor = executor,
                    };

                    var id = LocalApplicationInfos.Any()
                        ? LocalApplicationInfos.Max(p => p.Key) + 1
                        : 1
                    ;
                    LocalApplicationInfos[id] = info;

                    manageId = id;
                }
            );

            return manageId;
        }

        public bool WakeupApplication(uint manageId)
        {
            var result = false;

            Swicther.DoSwitch(
                ApplicationClient,
                timestamp => {
                    result = ApplicationClient.WakeupApplication(manageId);
                },
                (timestamp, talkerException) => {
                    // あくまで起動させるだけで管理まではしない
                    if(talkerException != null) {
                        Logger.Error(talkerException);
                    }

                    var info = LocalApplicationInfos[manageId];
                    info.Executor.RunAsync(CancellationToken.None).ContinueWith(t => {
                        Log.Out.Information($"local execute end: {info.TargetApplication}, {t.Result}");
                    });
                    result = true;
                }
            );

            return result;
        }

        public NKitApplicationStatus GetStatusApplication(uint manageId)
        {
            NKitApplicationStatus result = null;

            Swicther.DoSwitch(
                ApplicationClient,
                timestamp => {
                    result = ApplicationClient.GetStatus(manageId);
                },
                (timestamp, talkerException) => {
                    // あくまで起動させるだけで管理まではしない
                    if(talkerException != null) {
                        Logger.Error(talkerException);
                    }
                    // TODO: 内部的な監視がいるかも。。。だりぃ
                    // しらねーよ
                    result = new NKitApplicationStatus() {
                        IsEnabled = true,
                    };
                }
            );

            return result;
        }

        public bool ShutdownApplication(uint manageId, bool force)
        {
            bool result = false;

            Swicther.DoSwitch(
                ApplicationClient,
                timestamp => {
                    result = ApplicationClient.Shutdown(manageId, force);
                },
                (timestamp, talkerException) => {
                    // あくまで起動させるだけで管理まではしない
                    if(talkerException != null) {
                        Logger.Error(talkerException);
                    }
                    // TODO: 内部的な監視がいるかも。。。だりぃ
                    // しらねーよ
                    result = false;
                }
            );

            return result;
        }

        #endregion
    }
}
