using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Utility.ViewModell;

namespace ContentTypeTextNet.NKit.Main.ViewModel
{
    public abstract class ManagerViewModelBase<TManager> : ViewModelBase
        where TManager : ManagerModelBase
    {
        public ManagerViewModelBase(TManager model)
        {
            Model = model;
        }

        #region property

        protected TManager Model { get; }

        #endregion
    }
}
