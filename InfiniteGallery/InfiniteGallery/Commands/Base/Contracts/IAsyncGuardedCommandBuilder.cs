using System;
using Xamarin.CommunityToolkit.ObjectModel;

namespace InfiniteGallery.Commands.Base.Contracts
{
	public interface IAsyncGuardedCommandBuilder<TCommandParameter> : ICommandBuilder
	{
        IAsyncCommand<TCommandParameter> BuildCommand();

		IAsyncGuardedCommandBuilder<TCommandParameter> EnqueueAfterCommandExecuted(Action taskToInvoke);
		IAsyncGuardedCommandBuilder<TCommandParameter> EnqueueBeforeCommandExecuted(Action taskToInvoke);
	}

	public interface IAsyncGuardedCommandBuilder : ICommandBuilder
	{
        IAsyncCommand BuildCommand();

		IAsyncGuardedCommandBuilder EnqueueAfterCommandExecuted(Action taskToInvoke);
		IAsyncGuardedCommandBuilder EnqueueBeforeCommandExecuted(Action taskToInvoke);
	}
}