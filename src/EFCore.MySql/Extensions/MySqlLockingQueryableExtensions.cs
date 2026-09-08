// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for row-level locking (FOR UPDATE / FOR SHARE).
    /// </summary>
    public static class MySqlLockingQueryableExtensions
    {
        /// <summary>
        /// Appends a <c>FOR UPDATE</c> clause to the query, locking selected rows for update.
        /// Must be used within a transaction.
        /// </summary>
        public static IQueryable<T> ForUpdate<T>(this IQueryable<T> source)
            where T : class
            => source.TagWith("__efcore_for_update__");

        /// <summary>
        /// Appends a <c>FOR SHARE</c> clause to the query.
        /// Must be used within a transaction.
        /// </summary>
        public static IQueryable<T> ForShare<T>(this IQueryable<T> source)
            where T : class
            => source.TagWith("__efcore_for_share__");

        /// <summary>
        /// Appends a <c>FOR UPDATE NOWAIT</c> clause to the query.
        /// Returns an error immediately if the target row is locked.
        /// </summary>
        public static IQueryable<T> ForUpdateNoWait<T>(this IQueryable<T> source)
            where T : class
            => source.TagWith("__efcore_for_update_nowait__");

        /// <summary>
        /// Appends a <c>FOR UPDATE SKIP LOCKED</c> clause to the query.
        /// Skips rows that are already locked by another transaction.
        /// </summary>
        public static IQueryable<T> ForUpdateSkipLocked<T>(this IQueryable<T> source)
            where T : class
            => source.TagWith("__efcore_for_update_skiplocked__");
    }
}
