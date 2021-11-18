using System.Reflection;
using Autofac;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;
using InfiniteGallery.ViewModels.Base;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
    public class ExceptionHandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(BaseViewModel).GetTypeInfo().Assembly)
                .AssignableTo<IExceptionHandler>()
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}