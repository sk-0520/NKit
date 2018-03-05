using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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

    public abstract class RunnableViewModelBase<TModel, TExecuteResult> : SingleModelViewModelBase<TModel>, IReadOnlyRunnableStatus
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
        {
            Execute = GetInvokeUI(() =>
                new DelegateCommand(
                    () => ExecuteCore().ConfigureAwait(false),
                    () => CanExecute
                )
            );
        }

        #region IReadOnlyRunnableStatus

        public RunState RunState => Model.RunState;
        public DateTime StartTimestamp => Model.StartTimestamp;
        public DateTime EndTimestamp => Model.EndTimestamp;
        public TimeSpan PreparationSpan => Model.PreparationSpan;

        #endregion

        #region property

        protected virtual bool CanExecute
        {
            get
            {
                var canExecuteValues = new[] {
                    RunState.None,
                    RunState.Finished,
                    RunState.Error,
                };
                return canExecuteValues.Any(v => v == RunState);
            }
        }

        protected CancellationTokenSource ExecuteCancellationTokenSource { get; } = new CancellationTokenSource();

        #endregion

        #region function

        protected virtual Task<TExecuteResult> ExecuteCore()
        {
            var task = Model.ExecuteAsync(ExecuteCancellationTokenSource.Token);
            task.ConfigureAwait(false);
            return task;
        }

        protected void InvokeUI(Action action)
        {
            if(Dispatcher.CurrentDispatcher == Application.Current.Dispatcher) {
                action();
            } else {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        protected T GetInvokeUI<T>(Func<T> func)
        {
            if(Dispatcher.CurrentDispatcher == Application.Current.Dispatcher) {
                return func();
            } else {
                var result = default(T);
                Application.Current.Dispatcher.Invoke(() => {
                    result = func();
                });
                return result;
            }
        }


        #endregion

        #region command

        public DelegateCommand Execute { get; private set; }

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
        protected virtual void OnChangedModelProperty(PropertyChangedEventArgs e)
        { }

        void ChangedModelProperty(PropertyChangedEventArgs e)
        {
            if(RunnableProperties.Any(s => s == e.PropertyName)) {
                RaisePropertyChanged(e.PropertyName);
                if(e.PropertyName == nameof(Model.RunState)) {
                    InvokeUI(() => {
                        RaisePropertyChanged(nameof(CanExecute));
                        Execute.RaiseCanExecuteChanged();
                    });
                }
            }

            OnChangedModelProperty(e);
        }

        #endregion

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangedModelProperty(e);
        }

    }
}
