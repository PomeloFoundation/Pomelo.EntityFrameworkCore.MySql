// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class MySqlEntityTypeBuilderExtensions
    {
        #region CharSet

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the collation. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder HasCharSet(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string charSet)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetCharSet(charSet);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the collation. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> HasCharSet<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string charSet)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)HasCharSet((EntityTypeBuilder)entityTypeBuilder, charSet);

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the collation. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static IConventionEntityTypeBuilder HasCharSet(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string charSet,
            bool fromDataAnnotation = false)
        {
            if (CanSetCharSet(entityTypeBuilder, charSet, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetCharSet(charSet);

                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the MySQL character set can be set on the table associated with this entity.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the collation. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true"/> if the mapped table can be configured with the collation.</returns>
        public static bool CanSetCharSet(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string charSet,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(MySqlAnnotationNames.CharSet, charSet, fromDataAnnotation);
        }

        #endregion CharSet

        #region Collation

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder UseCollation(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string collation)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetCollation(collation);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> UseCollation<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string collation)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)UseCollation((EntityTypeBuilder)entityTypeBuilder, collation);

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static IConventionEntityTypeBuilder UseCollation(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string collation,
            bool fromDataAnnotation = false)
        {
            if (CanSetCollation(entityTypeBuilder, collation, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetCollation(collation);

                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the MySQL collation can be set on the table associated with this entity.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true"/> if the mapped table can be configured with the collation.</returns>
        public static bool CanSetCollation(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string collation,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(RelationalAnnotationNames.Collation, collation, fromDataAnnotation);
        }

        #endregion Collation
    }
}
