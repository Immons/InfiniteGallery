using System.Linq;
using Autofac;
using InfiniteGallery.Configuration.Ioc.Contracts;

namespace InfiniteGallery.Configuration.Ioc
{
	public static class IoCHelper
	{
		public static void InjectDependencies(object instance, IComponentContext componentContext)
		{
			var injectors = instance.GetType()
				.GetInterfaces()
				.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInjector<>))
				.ToList();

			foreach (var injector in injectors)
			{
				var typeToInject = injector.GetGenericArguments().First();
				var instanceToInject = componentContext.Resolve(typeToInject);

				var methodInfo = typeof(IInjector<>)
					.MakeGenericType(typeToInject)
					.GetMethod(
						nameof(IInjector<object>.Inject),
						new[]
						{
							typeToInject
						});

				methodInfo?.Invoke(
					instance,
					new[]
					{
						instanceToInject
					});
			}
		}
	}
}