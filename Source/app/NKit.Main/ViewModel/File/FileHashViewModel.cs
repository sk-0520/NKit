using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Main.Define;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.File;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Main.ViewModel.File
{
    public class FileHashViewModel : RunnableViewModelBase<FileHashModel, byte[]>
    {
        #region variable

        byte[] _hashValue;

        #endregion

        public FileHashViewModel(FileHashModel model)
            : base(model)
        { }

        #region property

        public byte[] HashValue
        {
            get { return this._hashValue; }
            set { SetProperty(ref this._hashValue, value); }
        }

        public HashType SelectedHashType
        {
            get { return Model.HashType; }
            set { SetModelValue(value, nameof(Model.HashType)); }
        }
        public HashType CurrentHashType { get; set; }

        public IEnumerable<HashType> HashTypeItems => EnumUtility.GetMembers<HashType>();


        #endregion

        #region command

        public ICommand CopyHashCommand => GetOrCreateCommand(() => new DelegateCommand<byte[]>(hash => Model.CopyHash(hash)));

        #endregion

        #region function



        #endregion

        #region RunnableViewModelBase

        protected override Task<byte[]> RunCoreAsync()
        {
            CurrentHashType = SelectedHashType;
            return base.RunCoreAsync().ContinueWith(t => HashValue = t.Result);
        }

        #endregion
    }
}
