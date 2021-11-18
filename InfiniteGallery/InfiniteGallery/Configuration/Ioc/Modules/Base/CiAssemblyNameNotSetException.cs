using System;

namespace InfiniteGallery.Configuration.Ioc.Modules.Base
{
    public class CiAssemblyNameNotSetException : Exception
    {
        public CiAssemblyNameNotSetException()
            : base(
            $"You must set {typeof(AssemblyNameAttribute)} for every {typeof(AssemblyModule)}")
        {
        }
    }
}
