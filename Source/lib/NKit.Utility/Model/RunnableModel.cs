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
        DateTime StartUtcTimestamp { get; }
        /// <summary>
        /// 処理終了時間。
        /// </summary>
        DateTime EndUtcTimestamp { get; }
        /// <summary>
        /// 準備にかかった時間。
        /// </summary>
        TimeSpan PreparateSpan { get; }
        /// <summary>
        /// 実行状態。
        /// </summary>
        RunState RunState { get; }
        /// <summary>
        /// キャンセル処理が可能か。
        /// </summary>
        bool IsCancelable { get; }
        /// <summary>
        /// 非同期実行が可能か。
        /// </summary>
        bool CanAsync { get; }

        /// <summary>
        /// 実行可能か。
        /// </summary>
        bool CanRun { get; }
        /// <summary>
        /// キャンセル可能か。
        /// <para><see cref="IsCancelable"/>と違って今の処理がキャンセル可能かを示す。</para>
        /// </summary>
        bool CanCancel { get; }

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

        public virtual DateTime StartUtcTimestamp
        {
            get { return this._startTimestamp; }
            private set { SetProperty(ref this._startTimestamp, value); }
        }
        public virtual DateTime EndUtcTimestamp
        {
            get { return this._endTimestamp; }
            private set { SetProperty(ref this._endTimestamp, value); }
        }
        public virtual TimeSpan PreparateSpan
        {
            get { return this._preparationSpan; }
            private set { SetProperty(ref this._preparationSpan, value); }
        }

        public virtual RunState RunState
        {
            get { return this._runState; }
            private set { SetProperty(ref this._runState, value); }
        }

        public virtual bool IsCancelable
        {
            get { return this._cancelable; }
            private set { SetProperty(ref this._cancelable, value); }
        }

        public virtual bool CanAsync
        {
            get { return this._canAsync; }
            private set { SetProperty(ref this._canAsync, value); }
        }

        public virtual bool CanRun
        {
            get
            {
                var canExecuteValues = new[] {
                    RunState.None,
                    RunState.Finished,
                    RunState.Error,
                    RunState.Cancel,
                };
                return canExecuteValues.Any(v => v == RunState);
            }
        }

        public virtual bool CanCancel
        {
            get
            {
                if(!IsCancelable) {
                    return false;
                }

                var canExecuteValues = new[] {
                    RunState.Preparate,
                    RunState.Running,
                };
                return canExecuteValues.Any(v => v == RunState);
            }
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
        protected virtual Task<PreparaResult<TRunResult>> PreparateCoreAsync(CancellationToken cancelToken) => GetDefaultPreparaValueAsync(true);

        protected abstract TRunResult RunCore(CancellationToken cancelToken);
        protected abstract Task<TRunResult> RunCoreAsync(CancellationToken cancelToken);

        public TRunResult Run(CancellationToken cancelToken)
        {
            EndUtcTimestamp = DateTime.MinValue;
            StartUtcTimestamp = DateTime.UtcNow;

            try {
                RunState = RunState.Preparate;

                var preResult = PreparateCore(cancelToken);

                PreparateSpan = DateTime.UtcNow - StartUtcTimestamp;

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
                EndUtcTimestamp = DateTime.UtcNow;
            }
        }

        public async Task<TRunResult> RunAsync(CancellationToken cancelToken)
        {
            EndUtcTimestamp = DateTime.MinValue;
            StartUtcTimestamp = DateTime.UtcNow;

            try {
                RunState = RunState.Preparate;

                var preResult = await PreparateCoreAsync(cancelToken);

                PreparateSpan = DateTime.UtcNow - StartUtcTimestamp;

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
                EndUtcTimestamp = DateTime.UtcNow;
            }
        }


        #endregion
    }


    public abstract class RunnableSyncModel<TRunResult>: RunnableModelBase<TRunResult>
    {
        #region RunnableModelBase

        public override bool CanAsync => false;

        protected override Task<PreparaResult<TRunResult>> PreparateCoreAsync(CancellationToken cancelToken)
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
