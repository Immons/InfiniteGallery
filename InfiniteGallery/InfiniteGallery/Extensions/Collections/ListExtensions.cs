using System.Collections.Generic;

namespace InfiniteGallery.Extensions.Collections
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, params T[] elements)
        {
            foreach (var element in elements)
            {
                list.Add(element);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                list.Add(element);
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                list.Remove(element);
            }
        }

        public static void ReplaceWith<T>(this IList<T> list, IEnumerable<T> elements)
        {
            list.Clear();
            list.AddRange(elements);
        }
    }
}