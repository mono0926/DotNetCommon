using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mono.Framework.Common.Extensios
{
    public static class IEnumerableExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            var results = new ObservableCollection<T>();
            foreach (var i in items)
            {
                results.Add(i);
            }
            return results;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var i in items)
            {
                action(i);
            }
        }
    }
}