using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Extensions
{
    public static class LinqExtensions
    {
        public static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    Debug.LogWarning("MinBy: Sequence contains no elements");
                    return default;
                }

                var min = enumerator.Current;
                var minKey = selector(min);

                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var currentKey = selector(current);

                    if (currentKey.CompareTo(minKey) < 0)
                    {
                        min = current;
                        minKey = currentKey;
                    }
                }

                return min;
            }
        }
    }
}