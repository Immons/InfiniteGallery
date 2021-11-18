using InfiniteGallery.Configuration.Ioc.Modules.Base;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
    [AssemblyName("InfiniteGallery")]
    public class MobileAssemblyModule : AssemblyModule
    {
        protected override void RegisterAssemblies()
        {
            var assemblies = GetAssembliesStartingWith();

            foreach (var assembly in assemblies)
            {
                AddAssemblySweep(assembly, Service);
                AddAssemblySweep(assembly, Factory);
                AddAssemblySweep(assembly, Job);
            }
        }
    }
}