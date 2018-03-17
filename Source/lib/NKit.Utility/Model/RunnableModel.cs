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

        /// <summary>
        /// 処理開始時間。
        /// </summary>
        DateTime StartTimestamp { get; }
        /// <summary>
        /// 処理終了時間。
        /// </summary>
        DateTime EndTimestamp { get; }
        /// <summary>
        /// 準備にかかった時間。
        /// </summary>
        TimeSpan PreparationSpan { get; }
        /// <summary>
        /// 実行状態。
        /// </summary>
        RunState RunState { get; }
        /// <summary>
        /// キャンセル処理が可能か。
        /// </summary>
        bool Cancelable { get; }
        /// <summary>
        /// 非同期実行が可能か。
        /// </summary>
        bool CanAsync { get; }

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
        bool _canAsync = true;
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

        public virtual bool CanAsync
        {
            get { return this._canAsync; }
            private set { SetProperty(ref this._canAsync, value); }
        }

        #endregion

        #region function

        protected PreparaResult<TRunResult> GetDefaultPreparaValue(bool value) => new PreparaResult<TRunResult>(value, default(TRunResult));
        protected Task<PreparaResult<TRunResult>> GetDefaultPreparaValueAsync(bool value) => Task.FromResult(GetDefaultPreparaValue(value));

        /// <summary>
        /// 準備処理。
        /// </summary>
        /// <typeparam name="TPreparaValue"></typeparam>
        /// <returns>真なら処理を継続、偽なら<see cref="TRunResult"/>を返して未処理とする。</returns>
        protected virtual PreparaResult<TRunResult> PreparateCore(CancellationToken cancelToken) => GetDefaultPreparaValue(true);

        /// <summary>
        /// 準備処理。
        /// </summary>
        /// <typeparam name="TPreparaValue"></typeparam>
        /// <returns>真なら処理を継続、偽なら<see cref="TRunResult"/>を返して未処理とする。</returns>
        protected virtual Task<PreparaResult<TRunResult>> PreparationCoreAsync(CancellationToken cancelToken) => GetDefaultPreparaValueAsync(true);

        protected abstract TRunResult RunCore(CancellationToken cancelToken);
        protected abstract Task<TRunResult> RunCoreAsync(CancellationToken cancelToken);

        public TRunResult Run(CancellationToken cancelToken)
        {
            EndTimestamp = DateTime.MinValue;
            StartTimestamp = DateTime.Now;

            try {
                RunState = RunState.Prepare;

                var preResult = PreparateCore(cancelToken);

                PreparationSpan = DateTime.Now - StartTimestamp;

                if(preResult.Success) {
                    RunState = RunState.Running;

                    var result = RunCore(cancelToken);

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
                Logger.Error(ex);
                RunState = RunState.Error;
                throw;
            } finally {
                EndTimestamp = DateTime.Now;
            }
        }

        public async Task<TRunResult> RunAsync(CancellationToken cancelToken)
        {
            EndTimestamp = DateTime.MinValue;
            StartTimestamp = DateTime.Now;

            try {
                RunState = RunState.Prepare;

                var preResult = await PreparationCoreAsync(cancelToken);

                PreparationSpan = DateTime.Now - StartTimestamp;

                if(preResult.Success) {
                    RunState = RunState.Running;

                    var result = await RunCoreAsync(cancelToken);

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
                Logger.Error(ex);
                RunState = RunState.Error;
                throw;
            } finally {
                EndTimestamp = DateTime.Now;
            }
        }


        #endregion
    }


    public abstract class RunnableSyncModel<TRunResult>: RunnableModelBase<TRunResult>
    {
        #region RunnableModelBase

        public override bool CanAsync => false;

        protected override Task<PreparaResult<TRunResult>> PreparationCoreAsync(CancellationToken cancelToken)
        {
            throw new NotSupportedException();
        }

        protected override Task<TRunResult> RunCoreAsync(CancellationToken cancelToken)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public abstract class RunnableAsyncModel<TRunResult> : RunnableModelBase<TRunResult>
    {
        #region RunnableModelBase

        public override bool CanAsync => true;

        protected override PreparaResult<TRunResult> PreparateCore(CancellationToken cancelToken)
        {
            throw new NotSupportedException();
        }

        protected override TRunResult RunCore(CancellationToken cancelToken)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
