using Acr.UserDialogs;
using Autofac;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Exceptions.Base.Contracts;
using InfiniteGallery.Exceptions.Base.Factories;
using InfiniteGallery.Helpers;
using InfiniteGallery.Helpers.Contracts;
using InfiniteGallery.Services;
using InfiniteGallery.Services.Contracts;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
	public class PortableDepenedenciesModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder
                .RegisterType<ViewAndViewModelResolver>()
                .As<IViewAndViewModelResolver>()
                .SingleInstance();

			builder
                .RegisterType<FormsNavigationService>()
                .As<INavigationService>()
				.AsSelf()
                .SingleInstance();

            builder
				.RegisterType<ExceptionGuard>()
				.As<IExceptionGuard>()
				.SingleInstance();

            builder
                .Register(context => UserDialogs.Instance)
                .As<IUserDialogs>()
                .SingleInstance();

            builder
                .RegisterType<MainThreadInvoker>()
                .As<IMainThreadInvoker>()
                .SingleInstance()
                .AutoActivate()
                .OnActivated(args => MainThread.Instance = args.Instance);
        }
	}
}