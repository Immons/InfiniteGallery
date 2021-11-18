using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Autofac;
using InfiniteGallery.Configuration.Api.Handlers;
using InfiniteGallery.Configuration.Ioc.Modules.Base;
using InfiniteGallery.Extensions.Types;
using Refit;

namespace InfiniteGallery.Configuration.Ioc.Modules
{
    [AssemblyName("InfiniteGallery")]
    public class EndpointsAssemblyModule : AssemblyModule
    {
        protected override void RegisterAssemblies()
        {
            var endpointsAssembly = GetAssembliesStartingWith();
            foreach (var assembly in endpointsAssembly)
            {
                var assemblyClasses = GetAssemblyClasses(assembly).ToList();
                var typesEndpoint =
                    assemblyClasses.EndingWith(Endpoint);

                var list = new List<Type>();
                list.AddRange(typesEndpoint);
                RegisterTypes(list);
            }
        }

        private void RegisterTypes(List<Type> types)
        {
            foreach (var type in types)
            {
                var implInterface = type.GetInterfaces().FirstOrDefault();
                if (implInterface != null)
                {
                    ContainerBuilder
                        .Register(context => RestService.For(implInterface, CreateClient(), new RefitSettings()
                        {
                            ContentSerializer = new SystemTextJsonContentSerializer()
                        }))
                        .As(implInterface)
                        .AsImplementedInterfaces()
                        .AutoActivate();
                }
            }
        }

        private static HttpClient CreateClient()
        {
            return new HttpClient(new DiagnosticsClientHandler(GetHttpHandler()))
            {
                BaseAddress = new Uri(AppConstants.BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        private static HttpClientHandler GetHttpHandler() =>
#if DEBUG
            new HttpClientHandler()
                {ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true};
#else
            new HttpClientHandler();
#endif
    }
}