using System.Threading.Tasks;
using InfiniteGallery.Commands.Base.Contracts;
using InfiniteGallery.Exceptions;
using Xamarin.CommunityToolkit.ObjectModel;

namespace InfiniteGallery.Commands.Base
{
	public abstract class AsyncGuardedDataContextCommandBuilder<TDataContext> : AsyncGuardedCommandBuilder, IAsyncGuardedDataContextCommandBuilder<TDataContext> where TDataContext : class
	{
		protected TDataContext DataContext { get; private set; }

		public virtual IAsyncGuardedDataContextCommandBuilder<TDataContext> RegisterDataContext(TDataContext dataContext)
		{
			DataContext = dataContext;
			return this;
		}

		protected override Task BuildCommandBody(AsyncCommand command)
		{
			if (DataContext == null)
            {
                ExceptionGuard.Guard(this,
                    () => throw new DataContextNotRegisteredForCommandBuilderException(GetType(),
                        typeof(TDataContext)));
				return Task.CompletedTask;
			}

			return base.BuildCommandBody(command);
		}

		public override void Dispose()
		{
			base.Dispose();
			DataContext = null;
		}
	}

	public abstract class
		AsyncGuardedDataContextCommandBuilder<TDataContext, TCommandParameter> :
			AsyncGuardedCommandBuilder<TCommandParameter>,
			IAsyncGuardedDataContextCommandBuilder<TDataContext, TCommandParameter> where TDataContext : class
	{
		protected TDataContext DataContext { get; private set; }

		public virtual IAsyncGuardedDataContextCommandBuilder<TDataContext, TCommandParameter> RegisterDataContext(
			TDataContext dataContext)
		{
			DataContext = dataContext;
			return this;
		}

		protected override Task BuildCommandBody(AsyncCommand<TCommandParameter> command, TCommandParameter model)
		{
			if (DataContext == null && !DataContextCanBeNull)
			{
                ExceptionGuard.Guard(this,
                    () => throw new DataContextNotRegisteredForCommandBuilderException(GetType(),
                        typeof(TDataContext)));
				return Task.CompletedTask;
			}

			return base.BuildCommandBody(command, model);
		}

		public override void Dispose()
		{
			base.Dispose();
			DataContext = null;
		}

        public virtual bool DataContextCanBeNull => false;
    }
}