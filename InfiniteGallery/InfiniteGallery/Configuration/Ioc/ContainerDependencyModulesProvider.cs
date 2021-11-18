using System.Collections.Generic;
using Autofac;
using Autofac.Features.ResolveAnything;
using InfiniteGallery.Configuration.Ioc.Modules;

namespace InfiniteGallery.Configuration.Ioc
{
	public abstract class ContainerDependencyModulesProvider
	{
		protected abstract IEnumerable<Module> GetPlatformSpecificModules();

		public virtual ContainerBuilder RegisterModules()
		{
			var builder = new ContainerBuilder();
			builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

			builder.RegisterModule<PortableDepenedenciesModule>();
			builder.RegisterModule<PortableHttpDependenciesModule>();
			builder.RegisterModule<ViewModelsModule>();
			builder.RegisterModule<ViewsModule>();
			builder.RegisterModule<MobileAssemblyModule>();
			builder.RegisterModule<CommandBuildersModule>();
            builder.RegisterModule<ExceptionHandlersModule>();
            builder.RegisterModule<EndpointsAssemblyModule>();

			foreach (var platformSpecificModule in GetPlatformSpecificModules())
				builder.RegisterModule(platformSpecificModule);

			return builder;
		}
	}
}