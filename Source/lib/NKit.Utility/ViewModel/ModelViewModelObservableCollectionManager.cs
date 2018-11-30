using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ContentTypeTextNet.NKit.Utility.Model;

namespace ContentTypeTextNet.NKit.Utility.ViewModel
{
    public enum ObservableCollectionKind
    {
        Before,
        After,
    }

    /// <summary>
    /// Model と ViewModel の一元的管理。
    /// <para>対になっている部分は内部で対応するがその前後処理までは面倒見ない。</para>
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ModelViewModelObservableCollectionManagerBase<TModel, TViewModel> : ObservableCollectionManagerBase<TModel>
        where TModel : ModelBase
        where TViewModel: ViewModelBase
    {
        public ModelViewModelObservableCollectionManagerBase(ObservableCollection<TModel> collection)
            : base(collection)
        {
            ViewModels = new ObservableCollection<TViewModel>(Collection.Select(m => ToViewModelCore(m)));
        }

        #region property

        public ObservableCollection<TViewModel> ViewModels { get; private set; }

        #endregion

        #region function

        protected abstract TViewModel ToViewModelCore(TModel model);

        protected abstract void AddItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels);
        protected abstract void RemoveItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels);
        protected abstract void ReplaceItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels);
        protected abstract void MoveItemsCore(ObservableCollectionKind kind, int newStartingIndex, int oldStartingIndex);
        protected abstract void ResetItemsCore(ObservableCollectionKind kind, IReadOnlyList<TViewModel> oldViewModels);

        #endregion

        #region ObservableManager

        protected override void AddItemsCore(IReadOnlyList<TModel> newItems)
        {
            var newViewModels = newItems
                .Cast<TModel>()
                .Select(m => ToViewModelCore(m))
                .ToList()
            ;

            AddItemsCore(ObservableCollectionKind.Before, newItems, newViewModels);

            ViewModels.AddRange(newViewModels);

            AddItemsCore(ObservableCollectionKind.After, newItems, newViewModels);
        }

        protected override void RemoveItemsCore(IReadOnlyList<TModel> oldItems, int oldStartingIndex)
        {
            var oldViewModels = ViewModels
                .Skip(oldStartingIndex)
                .Take(oldItems.Count)
                .ToList()
            ;

            RemoveItemsCore(ObservableCollectionKind.Before, oldItems, oldStartingIndex, oldViewModels);

            foreach(var counter in new Counter(oldViewModels.Count)) {
                ViewModels.RemoveAt(oldStartingIndex);
            }
            foreach(var oldViewModel in oldViewModels) {
                oldViewModel.Dispose();
            }

            RemoveItemsCore(ObservableCollectionKind.After, oldItems, oldStartingIndex, oldViewModels);
        }

        protected override void ReplaceItemsCore(IReadOnlyList<TModel> newItems, IReadOnlyList<TModel> oldItems)
        {
            // TODO: 正直こいつがいつ呼ばれるのか分かってない
            ReplaceItemsCore(ObservableCollectionKind.Before, newItems, oldItems, null, null);
            ReplaceItemsCore(ObservableCollectionKind.After, newItems, oldItems, null, null);
        }

        protected override void MoveItemsCore(int newStartingIndex, int oldStartingIndex)
        {
            MoveItemsCore(ObservableCollectionKind.Before, newStartingIndex, oldStartingIndex);

            ViewModels.Move(oldStartingIndex, newStartingIndex);

            MoveItemsCore(ObservableCollectionKind.After, newStartingIndex, oldStartingIndex);
        }

        protected override void ResetItemsCore()
        {
            var oldViewModels = ViewModels;

            ResetItemsCore(ObservableCollectionKind.Before, oldViewModels);

            ViewModels.Clear();
            foreach(var viewModel in oldViewModels) {
                viewModel.Dispose();
            }

            ResetItemsCore(ObservableCollectionKind.After, oldViewModels);
        }

        protected override void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => base.CollectionChanged(e)));
        }

        #endregion
    }


    public class ActionModeliewModelObservableCollectionManager<TModel, TViewModel> : ModelViewModelObservableCollectionManagerBase<TModel, TViewModel>
        where TModel : ModelBase
        where TViewModel : ViewModelBase
    {
        #region define

        public delegate TViewModel ToViewModelDelegate(TModel model);
        public delegate void AddItemsDelegate(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels);
        public delegate void RemoveItemsDelegate(ObservableCollectionKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels);
        public delegate void ReplaceItemsDelegate(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels);
        public delegate void MoveItemsDelegate(ObservableCollectionKind kind, int newStartingIndex, int oldStartingIndex);
        public delegate void ResetItemsDelegate(ObservableCollectionKind kind, IReadOnlyList<TViewModel> oldViewModels);

        #endregion

        public ActionModeliewModelObservableCollectionManager(ObservableCollection<TModel> collection)
            : base(collection)
        { }


        #region property

        public ToViewModelDelegate ToViewModel { get; set; }
        public AddItemsDelegate AddItems { get; set; }
        public RemoveItemsDelegate RemoveItems { get; set; }
        public ReplaceItemsDelegate ReplaceItems { get; set; }
        public MoveItemsDelegate MoveItems { get; set; }
        public ResetItemsDelegate ResetItems { get; set; }

        #endregion

        #region ViewViewModelObservableManagerBase

        protected override TViewModel ToViewModelCore(TModel model)
        {
            return ToViewModel(model);
        }

        protected override void AddItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels)
        {
            AddItems?.Invoke(kind, newModels, newViewModels);
        }

        protected override void RemoveItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels)
        {
            RemoveItems?.Invoke(kind, oldItems, oldStartingIndex, oldViewModels);
        }

        protected override void ReplaceItemsCore(ObservableCollectionKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels)
        {
            ReplaceItems?.Invoke(kind, newModels, oldModels, newViewModels, oldViewModels);
        }

        protected override void MoveItemsCore(ObservableCollectionKind kind, int newStartingIndex, int oldStartingIndex)
        {
            MoveItems?.Invoke(kind, newStartingIndex, oldStartingIndex);
        }

        protected override void ResetItemsCore(ObservableCollectionKind kind, IReadOnlyList<TViewModel> oldViewModels)
        {
            ResetItems?.Invoke(kind, oldViewModels);
        }

        protected override void Dispose(bool disposing)
        {
            ToViewModel = null;
            AddItems = null;
            RemoveItems = null;
            ReplaceItems = null;
            MoveItems = null;
            ResetItems = null;

            base.Dispose(disposing);
        }

        #endregion
    }
}
