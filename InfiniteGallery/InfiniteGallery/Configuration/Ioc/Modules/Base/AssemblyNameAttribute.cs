using System;

namespace InfiniteGallery.Configuration.Ioc.Modules.Base
{
    public class AssemblyNameAttribute : Attribute
    {
        public AssemblyNameAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}
