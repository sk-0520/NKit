using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Setting.Capture;
using ContentTypeTextNet.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureGroupModel: RunnableAsyncModel<None>
    {
        public CaptureGroupModel(CaptureManagerModel manager, CaptureGroupSetting groupSetting, IReadOnlyCaptureSetting captureSetting, IReadOnlyNKitSetting nkitSetting)
        {
            Manager = manager;
            GroupSetting = groupSetting;
            CaptureSetting = captureSetting;
            NKitSetting = nkitSetting;
        }

        #region property

        CaptureManagerModel Manager { get; }
        public CaptureGroupSetting GroupSetting { get; }
        public IReadOnlyCaptureSetting CaptureSetting { get; }
        public IReadOnlyNKitSetting NKitSetting { get; }

        EventWaitHandle SaveNoticeEvent { get; set; }
        Task SaveNoticePolling { get; set; }

        #endregion

        #region RunnableAsyncModel

        protected override PreparaResult<None> PreparateCore(CancellationToken cancelToken)
        {
            return base.PreparateCore(cancelToken);
        }

        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            var savedEventName = "TEST";
            var waitTime = TimeSpan.FromMinutes(1);
            SaveNoticeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, savedEventName);

            var saveNoticeCancel = new CancellationTokenSource();
            var saveNoticeCancelToken = saveNoticeCancel.Token;

            SaveNoticePolling = Task.Run(() => {
                while(true) {
                    var isSaved = SaveNoticeEvent.WaitOne(waitTime);
                    saveNoticeCancelToken.ThrowIfCancellationRequested();
                    if(isSaved) {
                        // 保存された画像をうんぬんかんぬん。
                        Logger.Information("saved image!");
                    }
                    cancelToken.ThrowIfCancellationRequested();
                }
            }, saveNoticeCancelToken);

            var dir = Directory.CreateDirectory(@"x:\nkit-screen2");
            return Manager.CaptureAsync(Setting.Define.CaptureMode.Control, true, true, true, savedEventName, dir, CaptureSetting.Scroll, cancelToken).ContinueWith(_ => {
                // 頭バグってきた
                saveNoticeCancel.Cancel();
                SaveNoticeEvent.Set();

                SaveNoticeEvent.Dispose();
                return None.Void;
            });
        }

        #endregion

    }
}
