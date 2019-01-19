// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySql specific extension methods for <see cref="IndexBuilder" />.
    /// </summary>
    public static class MySqlIndexBuilderExtensions
    {
        /// <summary>
        ///     Configures whether the index is clustered when targeting MySql.
        /// </summary>
        /// <param name="indexBuilder"> The builder for the index being configured. </param>
        /// <param name="clustered"> A value indicating whether the index is clustered. </param>
        /// <returns> A builder to further configure the index. </returns>
        public static IndexBuilder ForMySqlIsClustered([NotNull] this IndexBuilder indexBuilder, bool clustered = true)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));

            indexBuilder.Metadata.MySql().IsClustered = clustered;

            return indexBuilder;
        }

        /// <summary>
        ///     Configures whether the index is clustered when targeting MySql.
        /// </summary>
        /// <param name="indexBuilder"> The builder for the index being configured. </param>
        /// <param name="clustered"> A value indicating whether the index is clustered. </param>
        /// <returns> A builder to further configure the index. </returns>
        public static IndexBuilder<TEntity> ForMySqlIsClustered<TEntity>([NotNull] this IndexBuilder<TEntity> indexBuilder, bool clustered = true)
            => (IndexBuilder<TEntity>)ForMySqlIsClustered((IndexBuilder)indexBuilder, clustered);

        /// <summary>
        ///     Configures index include properties when targeting MySql.
        /// </summary>
        /// <param name="indexBuilder"> The builder for the index being configured. </param>
        /// <param name="propertyNames"> An array of property names to be used in 'include' clause. </param>
        /// <returns> A builder to further configure the index. </returns>
        public static IndexBuilder ForMySqlInclude([NotNull] this IndexBuilder indexBuilder, [NotNull] params string[] propertyNames)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));
            Check.NotNull(propertyNames, nameof(propertyNames));

            indexBuilder.Metadata.MySql().IncludeProperties = propertyNames;

            return indexBuilder;
        }

        /// <summary>
        ///     Configures index include properties when targeting MySql.
        /// </summary>
        /// <param name="indexBuilder"> The builder for the index being configured. </param>
        /// <param name="includeExpression">
        ///     <para>
        ///         A lambda expression representing the property(s) to be included in the 'include' clause
        ///         (<c>blog => blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If multiple properties are to be included then specify an anonymous type including the
        ///         properties (<c>post => new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> A builder to further configure the index. </returns>
        public static IndexBuilder<TEntity> ForMySqlInclude<TEntity>([NotNull] this IndexBuilder<TEntity> indexBuilder, [NotNull] Expression<Func<TEntity, object>> includeExpression)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));
            Check.NotNull(includeExpression, nameof(includeExpression));

            ForMySqlInclude(indexBuilder, includeExpression.GetPropertyAccessList().Select(GetSimpleMemberName).ToArray());

            return indexBuilder;
        }

        private static string GetSimpleMemberName(MemberInfo member)
        {
            var name = member.Name;
            var index = name.LastIndexOf('.');
            return index >= 0 ? name.Substring(index + 1) : name;
        }
    }
}
