using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector is null)
                throw new ArgumentNullException(nameof(keySelector));

            return _();

            IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>();
                foreach (var element in source)
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
            }
        }
    }
}