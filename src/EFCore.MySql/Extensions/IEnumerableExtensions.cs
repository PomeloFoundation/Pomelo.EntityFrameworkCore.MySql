using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    // ReSharper disable once InconsistentNaming
    internal static class IEnumerableExtensions
    {
        internal static IEnumerable<T> AppendIfTrue<T>(this IEnumerable<T> source, bool condition, Func<T> elementGetter)
            => condition
                ? source.Append(elementGetter.Invoke())
                : source;
    }
}
