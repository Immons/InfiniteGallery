namespace InfiniteGallery.Commands.Base.Contracts
{
	public interface IAsyncGuardedDataContextCommandBuilder<TDataContext> : IAsyncGuardedCommandBuilder
	{
		IAsyncGuardedDataContextCommandBuilder<TDataContext> RegisterDataContext(TDataContext dataContext);
	}

	public interface
		IAsyncGuardedDataContextCommandBuilder<TDataContext, TCommandParameter> : IAsyncGuardedCommandBuilder<
			TCommandParameter>
	{
		IAsyncGuardedDataContextCommandBuilder<TDataContext, TCommandParameter> RegisterDataContext(
			TDataContext dataContext);
	}
}
