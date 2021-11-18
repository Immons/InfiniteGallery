using System.Collections.Generic;
using Autofac;
using InfiniteGallery.Configuration.Ioc;

namespace InfiniteGallery.Droid.Configuration.Ioc
{
    internal class DroidContainerDependencyInjectionConfig : ContainerDependencyModulesProvider
    {
        protected override IEnumerable<Module> GetPlatformSpecificModules()
        {
            yield return new PlatformSpecificDependenciesModule();
        }
    }
}