using Autofac;
using InfiniteGallery.Configuration;

namespace InfiniteGallery.Droid.Configuration.Ioc
{
    public static class Bootstrapper
    {
        public static IContainer Init()
        {
            CoreBootstrapper.Init();
            var builder = new DroidContainerDependencyInjectionConfig().RegisterModules();
            return builder.Build();
        }
    }
}