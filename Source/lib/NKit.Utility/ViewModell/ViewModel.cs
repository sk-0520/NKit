using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace ContentTypeTextNet.NKit.Utility.ViewModell
{
    public abstract class ViewModelBase : BindableBase, IDisposable
    {
        ~ViewModelBase()
        {
            Dispose(false);
        }

        #region function

        protected virtual bool SetPropertyValue<TValue>(object obj, TValue value, [CallerMemberName] string targetMemberName = "", [CallerMemberName] string notifyPropertyName = "")
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(targetMemberName);

            var nowValue = (TValue)propertyInfo.GetValue(obj);

            if(!IComparable<TValue>.Equals(nowValue, value)) {
                propertyInfo.SetValue(obj, value);
                OnPropertyChanged(new PropertyChangedEventArgs(notifyPropertyName));

                return true;
            }

            return false;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>時に呼び出されるイベント。
        /// <para>呼び出し時点では<see cref="IsDisposed"/>は偽のまま。</para>
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Disposing;

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>されたか。
        /// </summary>
        [IgnoreDataMember, XmlIgnore]
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>の内部処理。
        /// <para>継承先クラスでは本メソッドを呼び出す必要がある。</para>
        /// </summary>
        /// <param name="disposing">CLRの管理下か。</param>
        protected virtual void Dispose(bool disposing)
        {
            if(IsDisposed) {
                return;
            }

            if(Disposing != null) {
                Disposing(this, EventArgs.Empty);
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }

            IsDisposed = true;
        }

        /// <summary>
        /// 解放。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public abstract class SingleModelViewModelBase<TModel> : ViewModelBase
        where TModel : ModelBase
    {
        public SingleModelViewModelBase(TModel model)
        {
            Model = model;

            AttachModelEvents();
        }

        #region property

        protected TModel Model { get; private set; }

        #endregion

        #region function

        protected bool SetModelValue<T>(T value, [CallerMemberName] string targetMemberName = "", [CallerMemberName] string notifyPropertyName = "")
        {
            return SetPropertyValue(Model, value, targetMemberName, notifyPropertyName);
        }

        protected virtual void AttachModelEventsCore()
        { }

        protected void AttachModelEvents()
        {
            if(Model != null) {
                AttachModelEventsCore();
            }
        }

        protected virtual void DetachModelEventsCore()
        { }

        protected void DetachModelEvents()
        {
            if(Model != null) {
                DetachModelEventsCore();
            }
        }

        #endregion

        #region ViewModelBase

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                DetachModelEvents();
            }
            base.Dispose(disposing);
            Model = null;
        }

        #endregion
    }

    public abstract class RunnableViewModelBase<TModel, TExecuteResult> : SingleModelViewModelBase<TModel>
        where TModel : RunnableModelBase<TExecuteResult>
    {
        #region variable

        static string[] RunnableProperties =
        {
            nameof(RunnableModelBase<TExecuteResult>.RunState),
            nameof(RunnableModelBase<TExecuteResult>.StartTimestamp),
            nameof(RunnableModelBase<TExecuteResult>.EndTimestamp),
            nameof(RunnableModelBase<TExecuteResult>.PreparationSpan),
        };

        #endregion

        public RunnableViewModelBase(TModel model)
            : base(model)
        { }

        #region property

        public RunState RunState => Model.RunState;
        public DateTime StartTimestamp => Model.StartTimestamp;
        public DateTime EndTimestamp => Model.EndTimestamp;
        public TimeSpan PreparationSpan => Model.PreparationSpan;

        #endregion

        #region command

        protected virtual Task<TExecuteResult> ExecuteCore()
        {
            var task = Model.ExecuteAsync();
            task.ConfigureAwait(false);
            return task;
        }

        protected virtual bool GetCanExecute()
        {
            return true;
        }

        #endregion

        #region command

        public ICommand Execute => new DelegateCommand(
            () => ExecuteCore().ConfigureAwait(false),
            () => GetCanExecute()
        );

        #endregion

        #region SingleModelViewModelBase

        protected override void AttachModelEventsCore()
        {
            base.AttachModelEventsCore();

            Model.PropertyChanged += Model_PropertyChanged;
        }

        protected override void DetachModelEventsCore()
        {
            base.DetachModelEventsCore();

            Model.PropertyChanged -= Model_PropertyChanged;
        }
        protected virtual void ChangedModelPropertyCore(PropertyChangedEventArgs e)
        { }

        void ChangedModelProperty(PropertyChangedEventArgs e)
        {
            if(RunnableProperties.Any(s => s == e.PropertyName)) {
                RaisePropertyChanged(e.PropertyName);
            }

            ChangedModelPropertyCore(e);
        }

        #endregion

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangedModelProperty(e);
        }

    }
}
