using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace ContentTypeTextNet.NKit.Utility.ViewModel
{
    public abstract class ViewModelBase : BindableBase, IDisposable
    {
        ~ViewModelBase()
        {
            Dispose(false);
        }

        #region property

        IDictionary<string, ICommand> CommandCache { get; } = new Dictionary<string, ICommand>();
        protected IEnumerable<ICommand> Commands => CommandCache.Values;

        #endregion

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

        protected TCommand GetOrCreateCommand<TCommand>(Func<TCommand> creator, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0)
            where TCommand: ICommand
        {
            var sb = new StringBuilder();
            sb.Append(GetType().FullName);
            sb.Append(':');
            sb.Append(callerMemberName);
            sb.Append(':');
            sb.Append(callerLineNumber);

            var key = sb.ToString();

            if(CommandCache.TryGetValue(key, out var cahceCommand)) {
                return (TCommand)cahceCommand;
            }

            var command = creator();
            CommandCache.Add(key, command);

            return command;
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
}
