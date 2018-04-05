using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    /// <summary>
    /// <see cref="ObservableCollection"/> の変更通知を受け取ってなんかする人。
    /// <para>管理者が誰かもうワケわからんことになるのです。</para>
    /// </summary>
    public abstract class ObservableManager<T>: ModelBase
    {
        public ObservableManager(ObservableCollection<T> collection)
        {
            if(collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            Collection = collection;
            Collection.CollectionChanged += Collection_CollectionChanged;
        }

        #region property

        protected ObservableCollection<T> Collection { get; private set; }

        #endregion

        #region function

        protected abstract void AddItemsCore(IReadOnlyList<T> newItems);
        void AddItems(IReadOnlyList<T> newItems)
        {
            AddItemsCore(newItems);
        }

        protected abstract void RemoveItemsCore(IReadOnlyList<T> oldItems, int oldStartingIndex);
        void RemoveItems(IReadOnlyList<T> oldItems, int oldStartingIndex)
        {
            RemoveItemsCore(oldItems, oldStartingIndex);
        }

        protected abstract void ReplaceItemsCore(IReadOnlyList<T> newItems, IReadOnlyList<T> oldItems);
        void ReplaceItems(IReadOnlyList<T> newItems, IReadOnlyList<T> oldItems)
        {
            ReplaceItemsCore(newItems, oldItems);
        }

        protected abstract void MoveItemsCore(int newStartingIndex, int oldStartingIndex);
        void MoveItems(int newStartingIndex, int oldStartingIndex)
        {
            MoveItemsCore(newStartingIndex, oldStartingIndex);
        }

        protected abstract void ResetItemsCore();
        void ResetItems()
        {
            ResetItemsCore();
        }

        IReadOnlyList<T> ConvertList(IList list)
        {
            return list.Cast<T>().ToList();
        }

        protected virtual void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action) {
                case NotifyCollectionChangedAction.Add:
                    AddItems(ConvertList(e.NewItems));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(ConvertList(e.OldItems), e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    ReplaceItems(ConvertList(e.NewItems), ConvertList(e.OldItems));
                    break;

                case NotifyCollectionChangedAction.Move:
                    MoveItems(e.NewStartingIndex, e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetItems();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region ModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(disposing) {
                    Collection.CollectionChanged -= Collection_CollectionChanged;
                }
                Collection = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged(e);
        }

    }

}
