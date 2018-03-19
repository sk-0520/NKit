using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class FileTypeViewModel : RunnableViewModelBase<FileTypeModel, None>
    {
        public FileTypeViewModel(FileTypeModel model)
            : base(model)
        { }

        #region property

        public string Information => Model.Information;

        #endregion
    }
}
