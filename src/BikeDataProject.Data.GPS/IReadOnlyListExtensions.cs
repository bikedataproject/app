using System;
using System.Collections;
using System.Collections.Generic;

namespace BikeDataProject.Data.GPS
{
    internal static class IReadOnlyListExtensions
    {
        public static IReadOnlyList<T> Map<T, S>(this IReadOnlyList<S> list, Func<S, T> map)
        {
            return new MappedList<T,S>(list, map);
        }

        private class MappedList<T, S> : IReadOnlyList<T>
        {
            private readonly IReadOnlyList<S> _list;
            private readonly Func<S, T> _map;

            public MappedList(IReadOnlyList<S> list, Func<S, T> map)
            {
                _map = map;
                _list = list;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < this.Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => _list.Count;

            public T this[int index] => _map(_list[index]);
        }
    }
}