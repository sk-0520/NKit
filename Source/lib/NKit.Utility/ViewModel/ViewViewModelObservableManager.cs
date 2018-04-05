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
    public enum ObservableCoreKind
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
    public abstract class ViewViewModelObservableManagerBase<TModel, TViewModel> : ObservableManager<TModel>
        where TModel : ModelBase
        where TViewModel: ViewModelBase
    {
        public ViewViewModelObservableManagerBase(ObservableCollection<TModel> collection)
            : base(collection)
        {
            ViewModels = new ObservableCollection<TViewModel>(Collection.Select(m => ToViewModelCore(m)));
        }

        #region property

        public ObservableCollection<TViewModel> ViewModels { get; private set; }

        #endregion

        #region function

        protected abstract TViewModel ToViewModelCore(TModel model);

        protected abstract void AddItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels);
        protected abstract void RemoveItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels);
        protected abstract void ReplaceItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels);
        protected abstract void MoveItemsCore(ObservableCoreKind kind, int newStartingIndex, int oldStartingIndex);
        protected abstract void ResetItemsCore(ObservableCoreKind kind, IReadOnlyList<TViewModel> oldViewModels);

        #endregion

        #region ObservableManager

        protected override void AddItemsCore(IReadOnlyList<TModel> newItems)
        {
            var newViewModels = newItems
                .Cast<TModel>()
                .Select(m => ToViewModelCore(m))
                .ToList()
            ;

            AddItemsCore(ObservableCoreKind.Before, newItems, newViewModels);

            ViewModels.AddRange(newViewModels);

            AddItemsCore(ObservableCoreKind.After, newItems, newViewModels);
        }

        protected override void RemoveItemsCore(IReadOnlyList<TModel> oldItems, int oldStartingIndex)
        {
            var oldViewModels = ViewModels
                .Skip(oldStartingIndex)
                .Take(oldItems.Count)
                .ToList()
            ;

            RemoveItemsCore(ObservableCoreKind.Before, oldItems, oldStartingIndex, oldViewModels);

            foreach(var counter in new Counter(oldViewModels.Count)) {
                ViewModels.RemoveAt(oldStartingIndex);
            }
            foreach(var oldViewModel in oldViewModels) {
                oldViewModel.Dispose();
            }

            RemoveItemsCore(ObservableCoreKind.After, oldItems, oldStartingIndex, oldViewModels);
        }

        protected override void ReplaceItemsCore(IReadOnlyList<TModel> newItems, IReadOnlyList<TModel> oldItems)
        {
            // TODO: 正直こいつがいつ呼ばれるのか分かってない
            ReplaceItemsCore(ObservableCoreKind.Before, newItems, oldItems, null, null);
            ReplaceItemsCore(ObservableCoreKind.After, newItems, oldItems, null, null);
        }

        protected override void MoveItemsCore(int newStartingIndex, int oldStartingIndex)
        {
            MoveItemsCore(ObservableCoreKind.Before, newStartingIndex, oldStartingIndex);

            ViewModels.Move(oldStartingIndex, newStartingIndex);

            MoveItemsCore(ObservableCoreKind.After, newStartingIndex, oldStartingIndex);
        }

        protected override void ResetItemsCore()
        {
            var oldViewModels = ViewModels;

            ResetItemsCore(ObservableCoreKind.Before, oldViewModels);

            ViewModels.Clear();
            foreach(var viewModel in oldViewModels) {
                viewModel.Dispose();
            }

            ResetItemsCore(ObservableCoreKind.After, oldViewModels);
        }

        protected override void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => base.CollectionChanged(e)));
        }

        #endregion
    }


    public class ActionViewViewModelObservableManager<TModel, TViewModel> : ViewViewModelObservableManagerBase<TModel, TViewModel>
        where TModel : ModelBase
        where TViewModel : ViewModelBase
    {
        #region define

        public delegate TViewModel ToViewModelDelegate(TModel model);
        public delegate void AddItemsDelegate(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels);
        public delegate void RemoveItemsDelegate(ObservableCoreKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels);
        public delegate void ReplaceItemsDelegate(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels);
        public delegate void MoveItemsDelegate(ObservableCoreKind kind, int newStartingIndex, int oldStartingIndex);
        public delegate void ResetItemsDelegate(ObservableCoreKind kind, IReadOnlyList<TViewModel> oldViewModels);

        #endregion

        public ActionViewViewModelObservableManager(ObservableCollection<TModel> collection)
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

        protected override void AddItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TViewModel> newViewModels)
        {
            AddItems?.Invoke(kind, newModels, newViewModels);
        }

        protected override void RemoveItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> oldItems, int oldStartingIndex, IReadOnlyList<TViewModel> oldViewModels)
        {
            RemoveItems?.Invoke(kind, oldItems, oldStartingIndex, oldViewModels);
        }

        protected override void ReplaceItemsCore(ObservableCoreKind kind, IReadOnlyList<TModel> newModels, IReadOnlyList<TModel> oldModels, IReadOnlyList<TViewModel> newViewModels, IReadOnlyList<TViewModel> oldViewModels)
        {
            ReplaceItems?.Invoke(kind, newModels, oldModels, newViewModels, oldViewModels);
        }

        protected override void MoveItemsCore(ObservableCoreKind kind, int newStartingIndex, int oldStartingIndex)
        {
            MoveItems?.Invoke(kind, newStartingIndex, oldStartingIndex);
        }

        protected override void ResetItemsCore(ObservableCoreKind kind, IReadOnlyList<TViewModel> oldViewModels)
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
