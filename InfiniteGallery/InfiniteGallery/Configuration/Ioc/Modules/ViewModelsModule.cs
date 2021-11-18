using System.Reflection;
using Autofac;
using InfiniteGallery.ViewModels.Base;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
	public class ViewModelsModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterAssemblyTypes(typeof(IBaseViewModel).GetTypeInfo().Assembly)
				.AssignableTo<IBaseViewModel>()
				.AsSelf()
				.AsImplementedInterfaces()
				.OnActivating(
					e =>
					{
						IoCHelper.InjectDependencies(e.Instance, e.Context);
					});
		}
	}
}
