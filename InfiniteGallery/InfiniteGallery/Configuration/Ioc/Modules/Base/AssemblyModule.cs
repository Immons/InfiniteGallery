using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using InfiniteGallery.Extensions.Types;
using Module = Autofac.Module;

namespace InfiniteGallery.Configuration.Ioc.Modules.Base
{
    public abstract class AssemblyModule : Module
    {
        private readonly string _assemblyName;
        public const string Service = "Service";
        public const string Endpoint = "Endpoint";
        public const string Factory = "Factory";
        public const string Job = "Job";

        private readonly Dictionary<string, IEnumerable<Type>> _assembliesClasses =
            new Dictionary<string, IEnumerable<Type>>();

        protected ContainerBuilder ContainerBuilder;

        protected AssemblyModule()
        {
            if (GetType().GetCustomAttributes().FirstOrDefault(a => a is AssemblyNameAttribute) is AssemblyNameAttribute
                assemblyNameAttribute)
                _assemblyName = assemblyNameAttribute.AssemblyName;
            else
                throw new CiAssemblyNameNotSetException();
        }

        protected IEnumerable<Assembly> GetAssembliesStartingWith()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.GetName().Name.StartsWith(_assemblyName, StringComparison.InvariantCulture));
        }

        protected Assembly GetAssemblyByName()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == _assemblyName);
        }

        protected IList<Type> AddAssemblySweep(Assembly assembly, string kind)
        {
            var types = GetAssemblyClasses(assembly)
                .EndingWith(kind)
                .ToList();

            var automaticTypes = GetAutomaticRegistrationTypes(types);

            AddTypes(automaticTypes);
            return automaticTypes;
        }

        protected IList<Type> GetAutomaticRegistrationTypes(IList<Type> types)
        {
            return types.Where(t => t.GetCustomAttribute<ManualIoCRegistrationAttribute>() == null).ToList();
        }

        protected IList<SingleInstanceType> GetSingleInstanceRegistrationTypes(IList<Type> types)
        {
            return types
                .Where(t => t.GetCustomAttribute<SingleInstanceIoCRegistrationAttribute>() != null)
                .Select(t=> new SingleInstanceType(t, t.GetCustomAttribute<SingleInstanceIoCRegistrationAttribute>().AutoActivate))
                .ToList();
        }

        protected IList<Type> AddAssemblySweep(Assembly assembly, Type inheritedFrom)
        {
            var types = GetAssemblyClasses(assembly)
                .Inherits(inheritedFrom)
                .ToList();

            var automaticTypes = GetAutomaticRegistrationTypes(types);

            AddTypes(automaticTypes);
            return automaticTypes;
        }

        protected IEnumerable<Type> GetAssemblyClasses(Assembly assembly)
        {
            IEnumerable<Type> classes;
            if (_assembliesClasses.ContainsKey(assembly.FullName))
                classes = _assembliesClasses[assembly.FullName];
            else
            {
                classes = assembly.CreatableTypes();
                _assembliesClasses.Add(assembly.FullName, classes);
            }

            return classes;
        }

        protected sealed override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            ContainerBuilder = builder;
            RegisterAssemblies();
        }

        protected abstract void RegisterAssemblies();

        protected virtual void AddTypes(IList<Type> types)
        {
            var singleInstanceTypes = GetSingleInstanceRegistrationTypes(types);

            foreach (var type in types)
            {
                var registrationBuilder = ContainerBuilder.RegisterType(type)
                    .AsImplementedInterfaces()
                    .AsSelf();

                var singleInstanceType = singleInstanceTypes.FirstOrDefault(sit => sit.Type == type);

                if (singleInstanceType != null)
                {
                    registrationBuilder.SingleInstance();

                    if (singleInstanceType.AutoActivate)
                    {
                        registrationBuilder.AutoActivate();
                    }
                }

                registrationBuilder.OnActivating(e =>
                    IoCHelper.InjectDependencies(e.Instance, e.Context));
            }
        }
    }


}