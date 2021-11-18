using Autofac;
using InfiniteGallery.Configuration.Api.Handlers;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
	public class PortableHttpDependenciesModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

        }
    }
}