using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.Wapper;
using ContentTypeTextNet.NKit.Setting.NKit;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileTypeModel : RunnableAsyncModel<None>
    {
        #region variable

        string _information;

        #endregion

        public FileTypeModel(FileInfo fileInfo, IReadOnlyNKitSetting nkitSetting)
        {
            FileInfo = fileInfo;
            NKitSetting = nkitSetting;
        }

        #region property

        FileInfo FileInfo { get; }

        IReadOnlyNKitSetting NKitSetting { get; }

        public string Information
        {
            get { return this._information; }
            set { SetProperty(ref this._information, value); }
        }

        #endregion

        #region RunnableModelBase

        protected override Task<PreparaResult<None>> PreparateCoreAsync(CancellationToken cancelToken)
        {
            return base.PreparateCoreAsync(cancelToken);
        }

        /// <summary>
        /// 全く調べてなかったけど file コマンドないんですね！
        /// </summary>
        /// <returns></returns>
        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            Information = "";
            var executor = new BusyBoxExecutor(NKitSetting.UsePlatformBusyBox, "file", FileInfo.FullName);
            executor.ReceivedOutput = e => {
                Information += "[I]" + e.Data;
            };
            executor.ReceivedError = e => {
                Information += "[E]" + e.Data;
            };

            return executor.RunAsync(cancelToken).ContinueWith(t => None.Void);
        }

        #endregion
    }
}
