using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ContentTypeTextNet.NKit.Utility.ViewModel;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindMultiItemsViewModel : ViewModelBase
    {
        #region variable

        FindItemViewModel _selectedSingleItem;
        bool _isSelectedGeneral;
        bool _isSelectedSingleFile;

        #endregion

        public FindMultiItemsViewModel()
        {
            Items = GetInvokeUI(() => new ObservableCollection<FindItemViewModel>());
            Items.CollectionChanged += Items_CollectionChanged;
        }

        #region property

        public ObservableCollection<FindItemViewModel> Items { get; } 

        public FindItemViewModel SelectedSingleItem
        {
            get { return this._selectedSingleItem; }
            set
            {
                if(SetProperty(ref this._selectedSingleItem, value)) {
                    if(SelectedSingleItem != null) {
                        IsSelectedGeneral = false;
                        IsSelectedSingleFile = true;
                    } else {
                        IsSelectedGeneral = true;
                        IsSelectedSingleFile = false;
                    }
                }
            }
        }

        public bool IsSelectedGeneral
        {
            get { return this._isSelectedGeneral; }
            set { SetProperty(ref this._isSelectedGeneral, value); }
        }
        public bool IsSelectedSingleFile
        {
            get { return this._isSelectedSingleFile; }
            set { SetProperty(ref this._isSelectedSingleFile, value); }
        }

        public bool IsEnabled => 1 < Items.Count;

        public int Count => Items.Count;

        public long TotalSize => Items.Sum(i => i.FileSize);

        #endregion

        #region function

        void RaisePropertiesChanged()
        {
            RaisePropertyChanged(nameof(IsEnabled));
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged(nameof(TotalSize));
        }


        #endregion

        #region ViewModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                Items.CollectionChanged -= Items_CollectionChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove) {
                if(e.OldItems.Cast<FindItemViewModel>().Any(i => i == SelectedSingleItem)) {
                    SelectedSingleItem = null;
                }
            }

            RaisePropertiesChanged();
        }
    }
}
