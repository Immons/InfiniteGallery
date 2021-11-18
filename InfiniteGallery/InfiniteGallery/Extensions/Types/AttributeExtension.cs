using System;
using System.Reflection;

namespace InfiniteGallery.Extensions.Types
{
    public static class AttributeExtension
    {
        public static T GetAttribute<T>(this object obj)
        {
            return Attribute.GetCustomAttribute(obj.GetType(), typeof(T)) is T attr ? attr : default;
        }

        public static T GetAttribute<T>(this Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) is T attr ? attr : default;
        }

        public static T GetAttribute<T>(this Assembly assembly) where T : Attribute
        {
            return assembly.GetCustomAttribute<T>();
        }

        public static Type CreateType(this string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
    }
}