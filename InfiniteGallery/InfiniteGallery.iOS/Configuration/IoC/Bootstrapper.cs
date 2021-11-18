using Autofac;
using InfiniteGallery.Configuration;

namespace InfiniteGallery.iOS.Configuration.IoC
{
    public static class Bootstrapper
    {
        public static IContainer Init()
        {
            CoreBootstrapper.Init();
            var builder = new iOSContainerDependencyInjectionConfig().RegisterModules();
            return builder.Build();
        }
    }
}