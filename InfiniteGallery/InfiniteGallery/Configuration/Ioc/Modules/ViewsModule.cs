using System.Reflection;
using Autofac;
using InfiniteGallery.Views.Base.Contracts;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
	public class ViewsModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterAssemblyTypes(typeof(PortableDepenedenciesModule).GetTypeInfo().Assembly)
				.AssignableTo<IBasePage>()
				.AsSelf();
		}
	}
}