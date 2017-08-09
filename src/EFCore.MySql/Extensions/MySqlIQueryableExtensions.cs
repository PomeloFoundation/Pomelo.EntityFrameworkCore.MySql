using System;
using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlIQueryableExtensions
    {
        private static Random _random = null;

        public static IOrderedQueryable<T> OrderByRandom<T>(this IQueryable<T> self)
        {
            return self.OrderBy(x => _random.Next());
        }
    }
}
