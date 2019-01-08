// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySql specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class MySqlEntityTypeBuilderExtensions
    {
        /// <summary>
        ///     Configures the table that the entity maps to when targeting MySql as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder ForMySqlIsMemoryOptimized(
            [NotNull] this EntityTypeBuilder entityTypeBuilder, bool memoryOptimized = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.MySql().IsMemoryOptimized = memoryOptimized;

            return entityTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting MySql as memory-optimized.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> ForMySqlIsMemoryOptimized<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder, bool memoryOptimized = true)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)ForMySqlIsMemoryOptimized((EntityTypeBuilder)entityTypeBuilder, memoryOptimized);

        /// <summary>
        ///     Configures an index on the specified properties. If there is an existing index on the given
        ///     set of properties, then the existing index will be returned for configuration.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="indexExpression">
        ///     <para>
        ///         A lambda expression representing the property(s) to be included in the index
        ///         (<c>blog => blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If the index is made up of multiple properties then specify an anonymous type including the
        ///         properties (<c>post => new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> An object that can be used to configure the index. </returns>
        public static IndexBuilder<TEntity> ForMySqlHasIndex<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder, [NotNull] Expression<Func<TEntity, object>> indexExpression)
            where TEntity : class
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotNull(indexExpression, nameof(indexExpression));

            var builder = ((IInfrastructure<InternalEntityTypeBuilder>)entityTypeBuilder).GetInfrastructure();

            return new IndexBuilder<TEntity>(
                builder.HasIndex(indexExpression.GetPropertyAccessList(), ConfigurationSource.Explicit));
        }
    }
}
