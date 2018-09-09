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
            nameof(IRunnableItem.StartUtcTimestamp),
            nameof(IRunnableItem.EndUtcTimestamp),
            nameof(IRunnableItem.PreparateSpan),
            nameof(IRunnableItem.IsCancelable),
            nameof(IRunnableItem.CanAsync),
            nameof(IRunnableItem.CanRun),
            nameof(IRunnableItem.CanCancel),
        };

        #endregion

        public RunnableViewModelBase(TModel model)
            : base(model)
        {
            RunCommand = GetInvokeUI(() => GetOrCreateCommand(() =>
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
            ));

            CancelRunCommand = GetInvokeUI(() => GetOrCreateCommand(() =>
                 new DelegateCommand(
                    () => CancelCore(),
                    () => CanCancel
                )
            ));
        }

        #region IReadOnlyRunnableStatus

        public RunState RunState => Model.RunState;
        public DateTime StartUtcTimestamp => Model.StartUtcTimestamp;
        public DateTime EndUtcTimestamp => Model.EndUtcTimestamp;
        public TimeSpan PreparateSpan => Model.PreparateSpan;
        public bool IsCancelable => Model.IsCancelable;
        public bool CanAsync => Model.CanAsync;
        public bool CanRun => Model.CanRun;
        public bool CanCancel => Model.CanCancel;

        #endregion

        #region property


        protected CancellationTokenSource RunningCancellationTokenSource { get; private set; }

        #endregion

        #region function

        protected virtual Task<TRunResult> RunCoreAsync()
        {
            if(RunningCancellationTokenSource != null) {
                RunningCancellationTokenSource.Dispose();
            }

            var token = CancellationToken.None;
            if(IsCancelable) {
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
            if(IsCancelable) {
                RunningCancellationTokenSource = new CancellationTokenSource();
                token = RunningCancellationTokenSource.Token;
            }
            return Model.Run(token);
        }

        protected virtual void CancelCore()
        {
            Debug.Assert(IsCancelable);
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
                switch(e.PropertyName) {
                    case nameof(Model.RunState):
                    case nameof(Model.CanRun):
                    case nameof(Model.CanCancel):
                        InvokeUI(() => {
                            //RaisePropertyChanged(nameof(CanRun));
                            //RaisePropertyChanged(nameof(CanCancel));
                            RunCommand.RaiseCanExecuteChanged();
                            CancelRunCommand.RaiseCanExecuteChanged();
                        });
                        break;

                    case nameof(Model.IsCancelable):
                        RaisePropertyChanged(nameof(CanCancel));
                        CancelRunCommand.RaiseCanExecuteChanged();
                        break;

                    default:
                        break;
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
