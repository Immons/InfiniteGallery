using System.Collections.Generic;
using Autofac;
using InfiniteGallery.Configuration.Ioc;

namespace InfiniteGallery.iOS.Configuration.IoC
{
    internal class iOSContainerDependencyInjectionConfig : ContainerDependencyModulesProvider
    {
        protected override IEnumerable<Module> GetPlatformSpecificModules()
        {
            yield return new PlatformSpecificDependenciesModule();
        }
    }
}