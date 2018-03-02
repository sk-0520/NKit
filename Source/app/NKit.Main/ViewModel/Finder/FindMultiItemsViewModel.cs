using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.ViewModell;

namespace ContentTypeTextNet.NKit.Main.ViewModel.Finder
{
    public class FindMultiItemsViewModel : ViewModelBase
    {
        #region variable


        #endregion

        public FindMultiItemsViewModel()
        {
            Items.CollectionChanged += Items_CollectionChanged;
        }

        #region property

        public ObservableCollection<FindItemViewModel> Items { get; } = new ObservableCollection<FindItemViewModel>();

        public bool IsEnabled => 1 < Items.Count;

        public int Count => Items.Count;

        public long TotalSize => Items.Sum(i => i.FileSize);

        #endregion

        #region property

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

        private void Items_CollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertiesChanged();
        }
    }
}
