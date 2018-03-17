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
    public class ApplicationSwitcher: ModelBase
    {
        public ApplicationSwitcher(Uri serviceUri)
        {
            if(serviceUri != null) {
                ApplicationClient = new NKitApplicationTalkerClient(NKitApplicationKind.Main, serviceUri);
            }
        }

        #region property

        NKitApplicationTalkerClient ApplicationClient { get; }
        NKitTakerSwicher Swicther { get; } = new NKitTakerSwicher();

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

                default:
                    throw new NotImplementedException();
            }
        }

        public void WakeupApplication(NKitApplicationKind targetApplication, string arguments, string workingDirectoryPath)
        {
            Swicther.DoSwitch(
                ApplicationClient,
                timestamp => {
                    ApplicationClient.WakeupApplication(targetApplication, arguments, workingDirectoryPath);
                },
                (timestamp, talkerException) => {
                    // あくまで起動させるだけで管理まではしない
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
                    Logger.Information($"local execute start: {targetApplication}, {arguments}");
                    executor.RunAsync(CancellationToken.None).ContinueWith(t => {
                        Log.Out.Information($"local execute end: {targetApplication}, {t.Result}");
                    });
                }
            );
        }

        #endregion
    }
}
