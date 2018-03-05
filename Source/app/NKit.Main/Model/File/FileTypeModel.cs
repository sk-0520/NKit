using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model.App;
using ContentTypeTextNet.NKit.Main.Model.Wapper;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileTypeModel : RunnableModelBase<None>
    {
        #region variable

        string _information;

        #endregion

        public FileTypeModel(FileInfo fileInfo, IReadOnlyAppSetting appSetting)
        {
            FileInfo = fileInfo;
            AppSetting = appSetting;
        }

        #region property

        FileInfo FileInfo { get; }

        IReadOnlyAppSetting AppSetting { get; }

        public string Information
        {
            get { return this._information; }
            set { SetProperty(ref this._information, value); }
        }

        #endregion

        #region RunnableModelBase

        protected override Task<PreparaResult<None>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            return base.PreparationCoreAsync(cancelToken);
        }

        /// <summary>
        /// 全く調べてなかったけど file コマンドないんですね！
        /// </summary>
        /// <returns></returns>
        protected override Task<None> RunCoreAsync(CancellationToken cancelToken)
        {
            Information = "";
            var executor = new BusyBoxExecutor(AppSetting.UsePlatformBusyBox, "file", FileInfo.FullName);
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
