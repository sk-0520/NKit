using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentTypeTextNet.NKit.Browser.ViewModel;
using ContentTypeTextNet.NKit.JustLooking.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.JustLooking.ViewModel
{
    public class JustLookingViewModel : SingleModelViewModelBase<JustLookingModel>
    {
        public JustLookingViewModel(JustLookingModel model)
            : base(model)
        { }

        #region property

        public BrowserViewModel Browser => new BrowserViewModel(Model.Browser);

        public string FilePath => Model.FilePath;
        public string DirectoryPath
        {
            get
            {
                var dirPath = Path.GetDirectoryName(FilePath);
                if(dirPath.Last() == Path.DirectorySeparatorChar) {
                    return dirPath;
                }

                return dirPath + Path.DirectorySeparatorChar;
            }
        }
        public string FileName => Path.GetFileName(FilePath);

        #endregion

        #region command

        public ICommand OpenDirectoryCommand => GetOrCreateCommand(() => new DelegateCommand(() => {
            Model.OpenDirectory();
        }));
        public ICommand OpenFileCommand => GetOrCreateCommand(() => new DelegateCommand(() => {
            Model.OpenFile();
        }));

        #endregion

        #region function
        #endregion
    }
}
