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
using ContentTypeTextNet.NKit.Setting.Define;
using ContentTypeTextNet.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.Capture
{
    public class CaptureGroupModel: RunnableAsyncModel<None>
    {
        #region define

        const string CaptureSubDirectoryNamePrefix = "cap-";

        #endregion

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
        string SavedEventName { get; set; }

        Task SaveNoticePolling { get; set; }

        DirectoryInfo CurrentCaptureDirectory { get; set; }

        public ObservableCollection<CaptureImageModel> Items { get; } = new ObservableCollection<CaptureImageModel>();


        #endregion

        #region function

        CaptureImageModel CreateImageModel(FileInfo fileInfo)
        {
            var result = new CaptureImageModel(this, fileInfo);

            return result;
        }

        DirectoryInfo GetCaptureBaseDirectory()
        {
            var workspaceDirPath = Environment.ExpandEnvironmentVariables(StartupOptions.WorkspacePath);
            var dirPath = Path.Combine(workspaceDirPath, "capture", GroupSetting.Id.ToString());
            var dir = Directory.CreateDirectory(dirPath);

            return dir;
        }

        IEnumerable<FileInfo> GetCaptureFiles(DirectoryInfo directory)
        {
            return directory.EnumerateFiles($"*_{Constants.CaptureRawImageSuffix}.png", SearchOption.TopDirectoryOnly);
        }

        public void InitializeCaptureFiles()
        {
            //TODO: 例外対応
            var captureDirs = GetCaptureBaseDirectory()
                .EnumerateDirectories(CaptureSubDirectoryNamePrefix + "*")
                .OrderBy(d => d.Name)
            ;
            Items.Clear();
            foreach(var captureDir in captureDirs) {
                var files = GetCaptureFiles(captureDir)
                    .OrderBy(f => f.Name)
                ;
                foreach(var file in files) {
                    Items.Add(CreateImageModel(file));
                }
            }
        }

        void AddCaptureFiles()
        {
            var files = GetCaptureFiles(CurrentCaptureDirectory);
            var addFileItems = Items
                .Concat(files.Select(f => CreateImageModel(f)))
                .GroupBy(i => i.RawImageFile.Name)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .OrderBy(i => i.RawImageFile.Name)
                .ToList()
            ;
            foreach(var addFileItem in addFileItems) {
                Items.Add(addFileItem);
            }
        }

        public void RemoveAllCaptureFiles()
        {
            var dir = GetCaptureBaseDirectory();
            dir.Delete(true);
        }

        public void CancelCapture()
        {
            Manager.CancelCapture();
        }

        #endregion

        #region RunnableAsyncModel

        public override bool CanRun => !Manager.NowCapturing && base.CanRun;

        protected override Task<PreparaResult<None>> PreparateCoreAsync(CancellationToken cancelToken)
        {
            var baseDirectory = GetCaptureBaseDirectory();
            CurrentCaptureDirectory = baseDirectory.CreateSubdirectory(CaptureSubDirectoryNamePrefix + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

            var id = DateTime.UtcNow.ToFileTime().ToString();
            SavedEventName = $"cttn-nkit-capture-save-{id}";
            SaveNoticeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, SavedEventName);

            return base.PreparateCoreAsync(cancelToken);
        }

        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            var waitTime = TimeSpan.FromMinutes(1);

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

            return Manager.CaptureAsync(GroupSetting.CaptureTarget, GroupSetting.IsEnabledClipboard, GroupSetting.IsImmediateSelect, true, SavedEventName, CurrentCaptureDirectory, Constants.CaptureImageKind, Constants.CaptureThumbnailKind, Constants.CaptureThumbnailSize, scrollSetting, cancelToken).ContinueWith(_ => {
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
