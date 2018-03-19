using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ContentTypeTextNet.NKit.Utility.Define;
using ContentTypeTextNet.NKit.Utility.Model;
using Prism.Commands;

namespace ContentTypeTextNet.NKit.Utility.ViewModel
{
    public interface IRunnableItem : IReadOnlyRunnableStatus
    {
        #region property
        ICommand RunCommand { get; }
        ICommand CancelRunCommand { get; }

        #endregion
    }

    public abstract class RunnableViewModelBase<TModel, TRunResult> : SingleModelViewModelBase<TModel>, IRunnableItem
        where TModel : RunnableModelBase<TRunResult>
    {
        #region variable

        static string[] RunnableProperties =
        {
            nameof(IRunnableItem.RunState),
            nameof(IRunnableItem.StartTimestamp),
            nameof(IRunnableItem.EndTimestamp),
            nameof(IRunnableItem.PreparationSpan),
            nameof(IRunnableItem.Cancelable),
            nameof(IRunnableItem.CanAsync),
        };

        #endregion

        public RunnableViewModelBase(TModel model)
            : base(model)
        {
            RunCommand = GetInvokeUI(() =>
                new DelegateCommand(
                    () => {
                        if(CanAsync) {
                            RunCoreAsync().ConfigureAwait(false);
                        } else {
                            RunCoreAsync();
                        }
                    },
                    () => CanRun
                )
            );

            CancelRunCommand = GetInvokeUI(() =>
                new DelegateCommand(
                    () => CancelCore(),
                    () => CanCancel
                )
            );
        }

        #region IReadOnlyRunnableStatus

        public RunState RunState => Model.RunState;
        public DateTime StartTimestamp => Model.StartTimestamp;
        public DateTime EndTimestamp => Model.EndTimestamp;
        public TimeSpan PreparationSpan => Model.PreparationSpan;
        public bool Cancelable => Model.Cancelable;
        public bool CanAsync => Model.CanAsync;

        #endregion

        #region property

        protected virtual bool CanRun
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

        protected virtual bool CanCancel
        {
            get
            {
                if(!Cancelable) {
                    return false;
                }

                var canExecuteValues = new[] {
                    RunState.Prepare,
                    RunState.Running,
                };
                return canExecuteValues.Any(v => v == RunState);
            }
        }

        protected CancellationTokenSource RunningCancellationTokenSource { get; private set; }

        #endregion

        #region function

        protected virtual Task<TRunResult> RunCoreAsync()
        {
            if(RunningCancellationTokenSource != null) {
                RunningCancellationTokenSource.Dispose();
            }

            var token = CancellationToken.None;
            if(Cancelable) {
                RunningCancellationTokenSource = new CancellationTokenSource();
                token = RunningCancellationTokenSource.Token;
            }
            var task = Model.RunAsync(token);
            task.ConfigureAwait(false);
            return task;
        }

        protected virtual TRunResult RunCore()
        {
            if(RunningCancellationTokenSource != null) {
                RunningCancellationTokenSource.Dispose();
            }

            var token = CancellationToken.None;
            if(Cancelable) {
                RunningCancellationTokenSource = new CancellationTokenSource();
                token = RunningCancellationTokenSource.Token;
            }
            return Model.Run(token);
        }

        protected virtual void CancelCore()
        {
            Debug.Assert(Cancelable);
            Debug.Assert(RunningCancellationTokenSource != null);
            RunningCancellationTokenSource.Cancel();
        }

        #endregion

        #region RunnableItem

        public DelegateCommand RunCommand { get; private set; }
        ICommand IRunnableItem.RunCommand => RunCommand;
        public DelegateCommand CancelRunCommand { get; private set; }
        ICommand IRunnableItem.CancelRunCommand => CancelRunCommand;


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
                        RaisePropertyChanged(nameof(CanRun));
                        RunCommand.RaiseCanExecuteChanged();
                        CancelRunCommand.RaiseCanExecuteChanged();
                    });
                }
                if(e.PropertyName == nameof(Model.Cancelable)) {
                    InvokeUI(() => {
                        RaisePropertyChanged(nameof(CanCancel));
                        CancelRunCommand.RaiseCanExecuteChanged();
                    });
                }
            }

            OnChangedModelProperty(e);
        }

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed) {
                if(RunningCancellationTokenSource != null) {
                    if(CanCancel) {
                        RunningCancellationTokenSource.Cancel();
                    }

                    RunningCancellationTokenSource.Dispose();

                    RunningCancellationTokenSource = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangedModelProperty(e);
        }

    }
}
