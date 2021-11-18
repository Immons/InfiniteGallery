using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfiniteGallery.Commands.Base.Contracts;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Exceptions.Base.Contracts;
using Xamarin.CommunityToolkit.ObjectModel;

namespace InfiniteGallery.Commands.Base
{
	public abstract class AsyncGuardedCommandBuilder : IAsyncGuardedCommandBuilder, IInjector<IExceptionGuard>
    {
        protected readonly Queue<Action> InvokeAfterCommandExecutedTaskQueue = new Queue<Action>();
        protected readonly Queue<Action> InvokeBeforeCommandExecutedTaskQueue = new Queue<Action>();
        protected bool CommandBeingExecuted;
        protected Func<bool> CanExecuteFunction;
        public IExceptionGuard ExceptionGuard { get; protected set; }

        public IAsyncGuardedCommandBuilder EnqueueAfterCommandExecuted(Action taskToInvoke)
        {
            InvokeAfterCommandExecutedTaskQueue.Enqueue(taskToInvoke);
            return this;
        }

        public IAsyncGuardedCommandBuilder EnqueueBeforeCommandExecuted(Action taskToInvoke)
        {
            InvokeBeforeCommandExecutedTaskQueue.Enqueue(taskToInvoke);
            return this;
        }

        public AsyncGuardedCommandBuilder ConfigureCanExecute(Func<bool> canExecute)
        {
            CanExecuteFunction = canExecute;
            return this;
        }

        public IAsyncCommand BuildCommand()
        {
            AsyncCommand command = null;

            // ReSharper disable once AccessToModifiedClosure
            command = new AsyncCommand(async () => await BuildCommandBody(command), () =>
            {
                var canExecute = CanExecute();
                if (canExecute)
                {
                    return !CommandBeingExecuted;
                }

                return false;
            });

            return command;
        }

        protected virtual async Task BuildCommandBody(AsyncCommand command)
        {
	        if (CommandBeingExecuted)
		        return;

            await ExceptionGuard.Guard(this,
                async () =>
                {
                    CommandBeingExecuted = true;

                    while (InvokeBeforeCommandExecutedTaskQueue.Any())
                    {
                        await Task.Run(InvokeBeforeCommandExecutedTaskQueue.Dequeue());
                    }

                    command.ChangeCanExecute();

                    AssignContextToExceptionHandlers(this);
                    await ExecuteCommandAction();
                },
                async () =>
                {
                    CommandBeingExecuted = false;
                    command.ChangeCanExecute();
                    Finally();

                    while (InvokeAfterCommandExecutedTaskQueue.Any())
                    {
                        await Task.Run(InvokeAfterCommandExecutedTaskQueue.Dequeue());
                    }
                });
        }

        protected virtual void Finally()
        {
        }

        protected abstract Task ExecuteCommandAction();

        protected virtual bool CanExecute()
        {
            return CanExecuteFunction == null || CanExecuteFunction.Invoke();
        }

        protected virtual void AssignContextToExceptionHandlers(object context)
        {
            ExceptionGuard.AssignContextToValidExceptionHandlers(context);
        }

        void IInjector<IExceptionGuard>.Inject(IExceptionGuard service) => ExceptionGuard = service;

        public virtual void Dispose()
        {
            ExceptionGuard = null;
        }
    }

    public abstract class AsyncGuardedCommandBuilder<TCommandParameter> : IAsyncGuardedCommandBuilder<TCommandParameter>, IInjector<IExceptionGuard>
    {
        protected readonly Queue<Action> InvokeAfterCommandExecutedTaskQueue = new Queue<Action>();
        protected readonly Queue<Action> InvokeBeforeCommandExecutedTaskQueue = new Queue<Action>();
        protected bool CommandBeingExecuted;
        protected Func<TCommandParameter, bool> CanExecuteFunction;
        public IExceptionGuard ExceptionGuard { get; protected set; }

        public IAsyncGuardedCommandBuilder<TCommandParameter> EnqueueAfterCommandExecuted(Action taskToInvoke)
        {
            InvokeAfterCommandExecutedTaskQueue.Enqueue(taskToInvoke);
            return this;
        }

        public IAsyncGuardedCommandBuilder<TCommandParameter> EnqueueBeforeCommandExecuted(Action taskToInvoke)
        {
            InvokeBeforeCommandExecutedTaskQueue.Enqueue(taskToInvoke);
            return this;
        }

        public AsyncGuardedCommandBuilder<TCommandParameter> ConfigureCanExecute(Func<TCommandParameter, bool> canExecute)
        {
            CanExecuteFunction = canExecute;
            return this;
        }

        public IAsyncCommand<TCommandParameter> BuildCommand()
        {
	        AsyncCommand<TCommandParameter> command = null;

            command = new AsyncCommand<TCommandParameter>(async model => await BuildCommandBody(command, model), i =>
            {
                var canExecute = CanExecute((TCommandParameter)i);
                if (canExecute)
                {
                    return !CommandBeingExecuted;
                }

                return false;
            }, allowsMultipleExecutions: false);

            return command;
        }

        protected virtual async Task BuildCommandBody(AsyncCommand<TCommandParameter> command, TCommandParameter model)
        {
	        if (CommandBeingExecuted)
		        return;

            await ExceptionGuard.Guard(this,
                async () =>
                {
                    CommandBeingExecuted = true;

                    while (InvokeBeforeCommandExecutedTaskQueue.Any())
                    {
                        await Task.Run(InvokeBeforeCommandExecutedTaskQueue.Dequeue());
                    }

                    command.ChangeCanExecute();

                    AssignContextToExceptionHandlers(this);
                    await ExecuteCommandAction(model);
                },
                async () =>
                {
                    CommandBeingExecuted = false;
                    command.ChangeCanExecute();
                    Finally();

                    while (InvokeAfterCommandExecutedTaskQueue.Any())
                    {
                        await Task.Run(InvokeAfterCommandExecutedTaskQueue.Dequeue());
                    }
                });
        }

        protected virtual void Finally()
        {
        }

        protected virtual void AssignContextToExceptionHandlers(object context)
        {
            ExceptionGuard.AssignContextToValidExceptionHandlers(context);
        }

        protected virtual bool CanExecute(TCommandParameter item)
        {
            return CanExecuteFunction == null || CanExecuteFunction.Invoke(item);
        }

        protected abstract Task ExecuteCommandAction(TCommandParameter item);

        void IInjector<IExceptionGuard>.Inject(IExceptionGuard service) => ExceptionGuard = service;

        public virtual void Dispose()
        {
            ExceptionGuard = null;
        }
    }
}