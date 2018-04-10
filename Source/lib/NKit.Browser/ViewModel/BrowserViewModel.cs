using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ContentTypeTextNet.NKit.Browser.Model;
using ContentTypeTextNet.NKit.Browser.View;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Browser.ViewModel
{
    public class BrowserViewModel : SingleModelViewModelBase<BrowserModel>
    {
        #region define

        struct BuildState
        {
            #region property

            public bool Now { get; set; }
            public bool End { get; set; }

            #endregion
        }

        #endregion

        #region variable

        Lazy<Stream> _fileStream;

        #endregion

        public BrowserViewModel(BrowserModel model)
            : base(model)
        {
            this._fileStream = new Lazy<Stream>(() => Model.FileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        #region property

        public BrowserKind BrowserKind => Model?.BrowserKind ?? BrowserKind.Unknown;

        //public FileInfo FileInfo => Model.FileInfo;
        public Uri FileUri => new Uri(Model.FileInfo.FullName);

        public Encoding Encoding => Model.Encoding;

        public bool IsReadOnly => Model.IsReadOnly;

        public bool IsText => Model.IsText;
        public bool IsXmlHtml => Model.IsXmlHtml;
        public bool IsJson => Model.IsJson;
        public bool IsImage => Model.IsImage;
        public bool IsProgram => Model.IsProgram;

        IDictionary<IBrowserDetail, BuildState> BuildStates { get; } = new ConcurrentDictionary<IBrowserDetail, BuildState>();

        #endregion

        #region function

        public Stream GetSharedStream()
        {
            if(this._fileStream.IsValueCreated) {
                this._fileStream.Value.Position = 0;
            }
            return new KeepOpenStream(this._fileStream.Value);
        }

        public bool CanBrowse(BrowserKind browserKind)
        {
            return Model.CanBrowse(browserKind);
        }

        public bool CanBuild(IBrowserDetail browserDetail)
        {
            if(BuildStates.TryGetValue(browserDetail, out var value)) {
                return !value.End && !value.Now;
            }

            return true;
        }

        public void BeginBuild(IBrowserDetail browserDetail)
        {
            if(BuildStates.TryGetValue(browserDetail, out var state)) {
                state.Now = true;
                BuildStates[browserDetail] = state;
            } else {
                BuildStates[browserDetail] = new BuildState() {
                    Now = true,
                };
            }
        }

        public void EndBuild(IBrowserDetail browserDetail)
        {
            // あるはず！
            var state = BuildStates[browserDetail];
            state.End = true;
            BuildStates[browserDetail] = state;
        }



        #endregion

        #region SingleModelViewModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    if(this._fileStream != null) {
                        if(this._fileStream.IsValueCreated) {
                            this._fileStream.Value.Dispose();
                        }
                        this._fileStream = null;
                    }
                    if(BuildStates != null) {
                        BuildStates.Clear();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
