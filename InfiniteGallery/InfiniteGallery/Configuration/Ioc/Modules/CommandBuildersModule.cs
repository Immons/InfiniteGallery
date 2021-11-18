using System.Reflection;
using Autofac;
using InfiniteGallery.Commands.Base.Contracts;
using InfiniteGallery.ViewModels.Base;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
    public class CommandBuildersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(BaseViewModel).GetTypeInfo().Assembly)
                .AssignableTo<ICommandBuilder>()
                .AsImplementedInterfaces()
                .AsSelf()
                .OnActivating(
                    e =>
                    {
                        IoCHelper.InjectDependencies(e.Instance, e.Context);
                    });
        }
    }
}