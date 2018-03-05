using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContentTypeTextNet.NKit.Utility.Define;

namespace ContentTypeTextNet.NKit.Utility.Model
{
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
        bool Cancelable { get; }

        #endregion
    }

    public abstract class RunnableModelBase<TRunResult> : ModelBase, IReadOnlyRunnableStatus
    {
        #region variable

        DateTime _startTimestamp;
        DateTime _endTimestamp;
        TimeSpan _preparationSpan;
        RunState _runState;
        bool _cancelable = true;

        #endregion

        #region property

        #endregion

        #region IReadOnlyRunnableStatus

        public virtual DateTime StartTimestamp
        {
            get { return this._startTimestamp; }
            private set { SetProperty(ref this._startTimestamp, value); }
        }
        public virtual DateTime EndTimestamp
        {
            get { return this._endTimestamp; }
            private set { SetProperty(ref this._endTimestamp, value); }
        }
        public virtual TimeSpan PreparationSpan
        {
            get { return this._preparationSpan; }
            private set { SetProperty(ref this._preparationSpan, value); }
        }


        public virtual RunState RunState
        {
            get { return this._runState; }
            private set { SetProperty(ref this._runState, value); }
        }

        public virtual bool Cancelable
        {
            get { return this._cancelable; }
            private set { SetProperty(ref this._cancelable, value); }
        }

        #endregion

        #region function

        protected PreparaResult<TRunResult> GetDefaultPreparaValue(bool value) => new PreparaResult<TRunResult>(value, default(TRunResult));
        protected Task<PreparaResult<TRunResult>> GetDefaultPreparaValueTask(bool value) => Task.FromResult(GetDefaultPreparaValue(value));

        /// <summary>
        /// 準備処理。
        /// </summary>
        /// <typeparam name="TPreparaValue"></typeparam>
        /// <returns>真なら処理を継続、偽なら<see cref="TRunResult"/>を返して未処理とする。</returns>
        protected virtual Task<PreparaResult<TRunResult>> PreparationCoreAsync(CancellationToken cancelToken) => GetDefaultPreparaValueTask(true);

        protected abstract Task<TRunResult> RunCoreAsync(CancellationToken cancelToken);

        public async Task<TRunResult> RunAsync(CancellationToken cancelToken)
        {
            EndTimestamp = DateTime.MinValue;
            StartTimestamp = DateTime.Now;

            try {
                RunState = RunState.Prepare;

                var preTask = PreparationCoreAsync(cancelToken);
                var preResult = await preTask;

                PreparationSpan = DateTime.Now - StartTimestamp;

                if(preResult.Success) {
                    RunState = RunState.Running;

                    var execTask = RunCoreAsync(cancelToken);
                    var result = await execTask;

                    RunState = RunState.Finished;
                    return result;
                } else {
                    RunState = RunState.None;
                    return preResult.Result;
                }
            } catch(OperationCanceledException) {
                RunState = RunState.Cancel;
                throw;
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

}
