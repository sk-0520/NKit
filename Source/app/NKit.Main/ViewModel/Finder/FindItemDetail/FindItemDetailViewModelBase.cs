using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Main.Model;
using ContentTypeTextNet.NKit.Main.Model.Finder;
using ContentTypeTextNet.NKit.Utility.Model;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder.FindItemDetail
{
    public abstract class FindItemDetailViewModelBase : SingleModelViewModelBase<FindItemModel>, ISelectable
    {
        #region variable

        bool _isSelected;

        #endregion

        public FindItemDetailViewModelBase(FindItemModel model)
            : base(model)
        { }

        #region property

        public abstract string Header { get; }

        public abstract bool Showable { get; }

        public abstract bool IsEnabled { get; }

        #endregion

        #region ISelectable

        public bool IsSelected
        {
            get { return this._isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }

        #endregion
    }
}
