using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace InfiniteGallery.Extensions.Types
{
	public static class TypeExtensions
	{
		public static IEnumerable<Type> ExceptionSafeGetTypes(this Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				Debug.WriteLine("ReflectionTypeLoadException masked during loading of {0} - error {1}",
					assembly.FullName, e);

				if (e.LoaderExceptions != null)
				{
					foreach (var excp in e.LoaderExceptions)
					{
						Debug.WriteLine(excp);
					}
				}

				if (Debugger.IsAttached)
					Debugger.Break();

				return new Type[0];
			}
		}

		public static IEnumerable<Type> CreatableTypes(this Assembly assembly)
		{
			return assembly
				.ExceptionSafeGetTypes()
				.Select(t => IntrospectionExtensions.GetTypeInfo(t))
				.Where(t => !t.IsAbstract)
				.Where(t => t.DeclaredConstructors.Any(c => !c.IsStatic && c.IsPublic))
				.Select(t => t.AsType());
		}

		public static IEnumerable<Type> EndingWith(this IEnumerable<Type> types, string endingWith)
		{
			return types.Where(x => x.Name.EndsWith(endingWith));
		}

		public static IEnumerable<Type> StartingWith(this IEnumerable<Type> types, string endingWith)
		{
			return types.Where(x => x.Name.StartsWith(endingWith));
		}

		public static IEnumerable<Type> Containing(this IEnumerable<Type> types, string containing)
		{
			return types.Where(x => x.Name.Contains(containing));
		}

		public static IEnumerable<Type> InNamespace(this IEnumerable<Type> types, string namespaceBase)
		{
			return types.Where(x => x.Namespace != null && x.Namespace.StartsWith(namespaceBase));
		}

		public static IEnumerable<Type> WithAttribute(this IEnumerable<Type> types, Type attributeType)
		{
			return types.Where(x => x.GetCustomAttributes(attributeType, true).Any());
		}

		public static IEnumerable<Type> WithAttribute<TAttribute>(this IEnumerable<Type> types)
			where TAttribute : Attribute
		{
			return types.WithAttribute(typeof(TAttribute));
		}

		public static IEnumerable<Type> Inherits(this IEnumerable<Type> types, Type baseType)
		{
			return types.Where(x => baseType.IsAssignableFrom(x));
		}

		public static IEnumerable<Type> Inherits<TBase>(this IEnumerable<Type> types)
		{
			return types.Inherits(typeof(TBase));
		}

		public static IEnumerable<Type> DoesNotInherit(this IEnumerable<Type> types, Type baseType)
		{
			return types.Where(x => !baseType.IsAssignableFrom(x));
		}

		public static IEnumerable<Type> DoesNotInherit<TBase>(this IEnumerable<Type> types)
			where TBase : Attribute
		{
			return types.DoesNotInherit(typeof(TBase));
		}

		public static IEnumerable<Type> Except(this IEnumerable<Type> types, params Type[] except)
		{
			if (except.Length >= 3)
			{
				var lookup = except.ToDictionary(x => x, x => true);
				return types.Where(x => !lookup.ContainsKey(x));
			}
			else
			{
				return types.Where(x => !except.Contains(x));
			}
		}

		public static bool IsGenericPartiallyClosed(this Type type) =>
			type.GetTypeInfo().IsGenericType
			&& type.GetTypeInfo().ContainsGenericParameters
			&& type.GetGenericTypeDefinition() != type;

		public class ServiceTypeAndImplementationTypePair
		{
			public ServiceTypeAndImplementationTypePair(List<Type> serviceTypes, Type implementationType)
			{
				ImplementationType = implementationType;
				ServiceTypes = serviceTypes;
			}

			public List<Type> ServiceTypes { get; private set; }
			public Type ImplementationType { get; private set; }
		}

		public static IEnumerable<ServiceTypeAndImplementationTypePair> AsTypes(this IEnumerable<Type> types)
		{
			return types.Select(t => new ServiceTypeAndImplementationTypePair(new List<Type>() { t }, t));
		}

		public static IEnumerable<ServiceTypeAndImplementationTypePair> AsInterfaces(this IEnumerable<Type> types) => types.Select(t => new ServiceTypeAndImplementationTypePair(t.GetInterfaces().ToList(), t));

		public static IEnumerable<ServiceTypeAndImplementationTypePair> AsInterfaces(this IEnumerable<Type> types, params Type[] interfaces)
		{
			// optimisation - if we have 3 or more interfaces, then use a dictionary
			if (interfaces.Length >= 3)
			{
				var lookup = interfaces.ToDictionary(x => x, x => true);
				return
					types.Select(
						t =>
							new ServiceTypeAndImplementationTypePair(
								t.GetInterfaces().Where(iface => lookup.ContainsKey(iface)).ToList(), t));
			}
			else
			{
				return
					types.Select(
						t =>
							new ServiceTypeAndImplementationTypePair(
								t.GetInterfaces().Where(iface => interfaces.Contains(iface)).ToList(), t));
			}
		}

		public static IEnumerable<ServiceTypeAndImplementationTypePair> ExcludeInterfaces(this IEnumerable<ServiceTypeAndImplementationTypePair> pairs, params Type[] toExclude)
		{
			foreach (var pair in pairs)
			{
				var excludedList = pair.ServiceTypes.Where(c => !toExclude.Contains(c)).ToList();
				if (excludedList.Any())
				{
					var newPair = new ServiceTypeAndImplementationTypePair(
						excludedList, pair.ImplementationType);
					yield return newPair;
				}
			}
		}

		public static object CreateDefault(this Type type)
		{
			if (type == null)
			{
				return null;
			}

			if (!type.GetTypeInfo().IsValueType)
			{
				return null;
			}

			if (Nullable.GetUnderlyingType(type) != null)
				return null;

			return Activator.CreateInstance(type);
		}

		public static ConstructorInfo FindApplicableConstructor(this Type type, IDictionary<string, object> arguments)
		{
			var constructors = type.GetConstructors();
			if (arguments == null || arguments.Count == 0)
			{
				return constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();
			}

			var unusedKeys = new List<string>(arguments.Keys);

			foreach (var constructor in constructors)
			{
				var parameters = constructor.GetParameters();
				foreach (var parameter in parameters)
				{
					if (unusedKeys.Contains(parameter.Name) && parameter.ParameterType.IsInstanceOfType(arguments[parameter.Name]))
					{
						unusedKeys.Remove(parameter.Name);
					}
				}

				if (unusedKeys.Count == 0)
				{
					return constructor;
				}
			}

			return null;
		}

		public static ConstructorInfo FindApplicableConstructor(this Type type, object[] arguments)
		{
			var constructors = type.GetConstructors();
			if (arguments == null || arguments.Length == 0)
			{
				return constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();
			}

			foreach (var constructor in constructors)
			{
				var parameterTypes = constructor.GetParameters().Select(p => p.ParameterType);
				var unusedArguments = arguments.ToList();

				foreach (var parameterType in parameterTypes)
				{
					var argumentMatch = unusedArguments.FirstOrDefault(arg => parameterType.IsInstanceOfType(arg));
					if (argumentMatch != null)
					{
						unusedArguments.Remove(argumentMatch);
					}
				}

				if (unusedArguments.Count == 0)
				{
					return constructor;
				}
			}

			return null;
		}
	}
}