using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Main.Model.File
{
    public class FileHashModel : RunnableModelBase<byte[]>
    {
        #region define
        #endregion

        public FileHashModel(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        #region property

        public FileInfo FileInfo { get; }

        HashProvider HashProvider { get; set; }

        public HashType HashType { get; set; }
        FileStream FileStream { get; set; }

        #endregion

        #region function
        public void CopyHash(byte[] hash)
        {
            var cp = new ClipboardOperator();
            var dc = new DisplayConverter();

            var s = dc.ToHexString(hash);
            cp.CopyText(s);
        }

        #endregion

        #region RunnableModelBase

        protected override Task<PreparaResult<byte[]>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            HashProvider = new HashProvider(HashType);
            ;
            FileStream = FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return base.PreparationCoreAsync(cancelToken);
        }

        protected override Task<byte[]> RunCoreAsync(CancellationToken cancelToken)
        {
            return Task.Run(() => HashProvider.Execute(FileStream));
        }

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(FileStream != null) {
                    FileStream.Dispose();
                    FileStream = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

    }
}
