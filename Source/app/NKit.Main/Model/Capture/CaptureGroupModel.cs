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

        public ObservableCollection<CaptureImageModel> Images { get; } = new ObservableCollection<CaptureImageModel>();


        #endregion

        #region function

        bool IsEqualPath(string a, string b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        bool IsEqualImageData(string fileName, string directoryName, IReadOnlyCaptureImageSetting setting) => IsEqualPath(fileName, setting.FileName) && IsEqualPath(directoryName, setting.DirectoryName);

        CaptureImageModel CreateImageModel(FileInfo fileInfo)
        {

            var fileDirName = fileInfo.Directory.Name;
            var imageSetting = GroupSetting.Images
                .FirstOrDefault(i => IsEqualImageData(fileInfo.Name, fileDirName, i))
            ;
            var hasImageSetting = imageSetting != null;
            if(!hasImageSetting) {
                imageSetting = new CaptureImageSetting() {
                    DirectoryName = fileDirName,
                    FileName = fileInfo.Name,
                    Comment = string.Empty,
                };
                GroupSetting.Images.Add(imageSetting);
            }

            var result = new CaptureImageModel(imageSetting, this, fileInfo);
            if(!hasImageSetting) {
                result.RefreshSetting();
            }
            return result;
        }

        DirectoryInfo GetCaptureImageBaseDirectory()
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
            //TODO: ????????????
            var captureDirs = GetCaptureImageBaseDirectory()
                .EnumerateDirectories(CaptureSubDirectoryNamePrefix + "*")
                .OrderBy(d => d.Name)
            ;
            Images.Clear();
            foreach(var captureDir in captureDirs) {
                var files = GetCaptureFiles(captureDir)
                    .OrderBy(f => f.Name)
                ;
                foreach(var file in files) {
                    Images.Add(CreateImageModel(file));
                }
            }
        }

        void AddCaptureFiles()
        {
            var files = GetCaptureFiles(CurrentCaptureDirectory);
            var addFileItems = Images
                .Concat(files.Select(f => CreateImageModel(f)))
                .GroupBy(i => i.ImageFile.Name)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .OrderBy(i => i.ImageFile.Name)
                .ToList()
            ;
            foreach(var addFileItem in addFileItems) {
                Images.Add(addFileItem);
            }
        }

        public void RemoveAllCaptureFiles()
        {
            var dir = GetCaptureImageBaseDirectory();
            dir.Delete(true);
        }

        public void CancelCapture()
        {
            Manager.CancelCapture();
        }

        bool RemoveImage(CaptureImageModel image)
        {
            Images.Remove(image);

            var imageSetting = GroupSetting.Images
                .FirstOrDefault(i => IsEqualImageData(image.ImageFile.Name, image.ImageFile.Directory.Name, i))
            ;
            GroupSetting.Images.Remove(imageSetting);

            using(image) {
                try {
                    image.ImageFile.Delete();
                    var thumbnailImagePath = image.GetEnabledThumbnailImagePath();
                    if(!string.Equals(image.ImageFile.FullName, thumbnailImagePath, StringComparison.InvariantCultureIgnoreCase)) {
                        if(System.IO.File.Exists(thumbnailImagePath)) {
                            System.IO.File.Delete(thumbnailImagePath);
                        }
                    }
                    return true;
                } catch(IOException ex) {
                    Logger.Error(ex);
                }
                return false;
            }
        }

        public bool RemoveImageAt(int index)
        {
            var image = Images[index];
            return RemoveImage(image);
        }

        #endregion

        #region RunnableAsyncModel

        public override bool CanRun => !Manager.NowCapturing && base.CanRun;

        protected override Task<PreparaResult<None>> PreparateCoreAsync(CancellationToken cancelToken)
        {
            var baseDirectory = GetCaptureImageBaseDirectory();
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
                        // ???????????????????????????????????????????????????
                        // ?????????????????????????????????????????????????????????????????????
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
                // ?????????????????????
                saveNoticeCancel.Cancel();
                SaveNoticeEvent.Set();

                SaveNoticeEvent.Dispose();

                return None.Void;
            });
        }

        #endregion

    }
}
