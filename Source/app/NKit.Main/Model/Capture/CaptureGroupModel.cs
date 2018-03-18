using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Common;
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

        public ObservableCollection<FileInfo> FileItems { get; } = new ObservableCollection<FileInfo>();

        #endregion

        #region function

        DirectoryInfo GetCaptureDirectory()
        {
            var dirPath = Path.Combine(Environment.ExpandEnvironmentVariables(StartupOptions.WorkspacePath), "capture", GroupSetting.Id.ToString());
            var dir = Directory.CreateDirectory(dirPath);

            return dir;
        }

        IEnumerable<FileInfo> GetCaptureFiles()
        {
            var dir = GetCaptureDirectory();
            return dir.EnumerateFiles("*.png", SearchOption.TopDirectoryOnly);
        }

        void LoadCaptureFiles()
        {
            var files = GetCaptureFiles().OrderBy(f => f.Name);
            FileItems.Clear();
            foreach(var file in files) {
                FileItems.Add(file);
            }
        }

        void AddCaptureFiles()
        {
            var files = GetCaptureFiles();
            var addFileItems = FileItems
                .Concat(files)
                .GroupBy(f => f.Name)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .OrderBy(f => f.Name)
                .ToList()
            ;
            foreach(var addFileItem in addFileItems) {
                FileItems.Add(addFileItem);
            }

        }

        #endregion

        #region RunnableAsyncModel

        protected override PreparaResult<None> PreparateCore(CancellationToken cancelToken)
        {
            return base.PreparateCore(cancelToken);
        }

        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            var id = DateTime.Now.ToFileTime().ToString();
            var savedEventName = $"cttn-nkit-capture-save-{id}";
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
                        // これさぁ、監視した方が手っ取り早くないですかね
                        Logger.Information("saved image!");
                        AddCaptureFiles();
                    }
                    cancelToken.ThrowIfCancellationRequested();
                }
            }, saveNoticeCancelToken);


            var scrollSetting = GroupSetting.OverwriteScrollSetting
                ? GroupSetting.Scroll
                : CaptureSetting.Scroll
            ;
            var dir = GetCaptureDirectory();
            return Manager.CaptureAsync(GroupSetting.CaptureTarget, GroupSetting.IsEnabledClipboard, GroupSetting.IsImmediateSelect, true, savedEventName, dir, scrollSetting, cancelToken).ContinueWith(_ => {
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
