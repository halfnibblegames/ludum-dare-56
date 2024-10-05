using System;
using System.Collections.Generic;

namespace HalfNibbleGame;

static class LinqExtensions
{
    public static IEnumerable<T> TakeWhileIncluding<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var element in source)
        {
            yield return element;
            if (!predicate(element))
            {
                yield break;
            }
        }
    }
}
