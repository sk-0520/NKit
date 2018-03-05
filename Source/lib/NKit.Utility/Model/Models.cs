using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ContentTypeTextNet.NKit.Utility.Define;
using Prism.Mvvm;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public abstract class ModelBase : BindableBase, IDisposable
    {
        ~ModelBase()
        {
            Dispose(false);
        }

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

    public struct None
    {
        public static None Void { get; } = new None();
    }

    public struct PreparaResult<TResult>
    {
        public PreparaResult(bool success, TResult result)
        {
            Success = success;
            Result = result;
        }

        #region property

        public bool Success { get; }
        public TResult Result { get; }

        #endregion
    }

    public interface IReadOnlyRunnableStatus
    {
        #region property

        DateTime StartTimestamp { get; }
        DateTime EndTimestamp { get; }
        TimeSpan PreparationSpan { get; }
        RunState RunState { get; }
        bool IsRunningCancel { get; }

        #endregion
    }

    public abstract class RunnableModelBase<TExecuteResult> : ModelBase, IReadOnlyRunnableStatus
    {
        #region variable

        DateTime _startTimestamp;
        DateTime _endTimestamp;
        TimeSpan _preparationSpan;
        RunState _runState;

        bool _isRunningCancel;

        #endregion

        #region property

        #endregion

        #region IReadOnlyRunnableStatus

        public DateTime StartTimestamp
        {
            get { return this._startTimestamp; }
            private set { SetProperty(ref this._startTimestamp, value); }
        }
        public DateTime EndTimestamp
        {
            get { return this._endTimestamp; }
            private set { SetProperty(ref this._endTimestamp, value); }
        }
        public TimeSpan PreparationSpan
        {
            get { return this._preparationSpan; }
            private set { SetProperty(ref this._preparationSpan, value); }
        }


        public RunState RunState
        {
            get { return this._runState; }
            private set { SetProperty(ref this._runState, value); }
        }

        public bool IsRunningCancel
        {
            get { return this._isRunningCancel; }
            set { SetProperty(ref this._isRunningCancel, value); }
        }

        #endregion

        #region function

        protected PreparaResult<TExecuteResult> GetDefaultPreparaValue(bool value) => new PreparaResult<TExecuteResult>(value, default(TExecuteResult));
        protected Task<PreparaResult<TExecuteResult>> GetDefaultPreparaValueTask(bool value) => Task.FromResult(GetDefaultPreparaValue(value));

        /// <summary>
        /// 準備処理。
        /// </summary>
        /// <typeparam name="TPreparaValue"></typeparam>
        /// <returns>真なら処理を継続、偽なら<see cref="TExecuteResult"/>を返して未処理とする。</returns>
        protected virtual Task<PreparaResult<TExecuteResult>> PreparationCoreAsync(CancellationToken cancelToken) => GetDefaultPreparaValueTask(true);

        protected abstract Task<TExecuteResult> RunCoreAsync(CancellationToken cancelToken);

        public async Task<TExecuteResult> RunAsync(CancellationToken cancelToken)
        {
            IsRunningCancel = false;
            EndTimestamp = DateTime.MinValue;
            StartTimestamp = DateTime.Now;

            try {
                RunState = RunState.Prepare;

                var preTask = PreparationCoreAsync(cancelToken);
                var preResult = await preTask;
                IsRunningCancel = preTask.IsCanceled;

                PreparationSpan = DateTime.Now - StartTimestamp;

                if(preResult.Success && !IsRunningCancel) {
                    RunState = RunState.Running;

                    var execTask = RunCoreAsync(cancelToken);
                    var result = await execTask;
                    IsRunningCancel = execTask.IsCanceled;

                    RunState = RunState.Finished;
                    return result;
                } else {
                    RunState = RunState.None;
                    return preResult.Result;
                }
            } catch(Exception ex) {
                Debug.WriteLine(ex);
                RunState = RunState.Error;
                throw;
            } finally {
                EndTimestamp = DateTime.Now;
            }
        }

        #endregion
    }

    public class RawModel : ModelBase
    {
        public RawModel(object rawObject)
        {
            RawObject = rawObject;
        }

        #region property

        public object RawObject { get; }

        #endregion
    }

    public class RawModel<T> : RawModel
    {
        public RawModel(T rawObject)
            : base(rawObject)
        {
            Raw = rawObject;
        }

        #region property

        public T Raw { get; }

        #endregion
    }
}
