// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class MySqlEntityTypeBuilderExtensions
    {
        #region CharSet and delegation

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder HasCharSet(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string charSet,
            bool? explicitlyDelegateToChildren = null)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NullButNotEmpty(charSet, nameof(charSet));

            entityTypeBuilder.Metadata.SetCharSet(charSet);
            entityTypeBuilder.Metadata.SetCharSetDelegation(explicitlyDelegateToChildren);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder HasCharSet(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] CharSet charSet,
            bool? explicitlyDelegateToChildren = null)
            => entityTypeBuilder.HasCharSet(charSet?.Name, explicitlyDelegateToChildren);

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> HasCharSet<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string charSet,
            bool? explicitlyDelegateToChildren = null)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)HasCharSet((EntityTypeBuilder)entityTypeBuilder, charSet, explicitlyDelegateToChildren);

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> HasCharSet<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] CharSet charSet,
            bool? explicitlyDelegateToChildren = null)
            where TEntity : class
            => entityTypeBuilder.HasCharSet(charSet?.Name, explicitlyDelegateToChildren);

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static IConventionEntityTypeBuilder HasCharSet(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string charSet,
            bool? explicitlyDelegateToChildren = null,
            bool fromDataAnnotation = false)
        {
            if (entityTypeBuilder.CanSetCharSet(charSet, fromDataAnnotation) &&
                entityTypeBuilder.CanSetCharSetDelegation(explicitlyDelegateToChildren, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetCharSet(charSet, fromDataAnnotation);
                entityTypeBuilder.Metadata.SetCharSetDelegation(explicitlyDelegateToChildren, fromDataAnnotation);

                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses its default collation.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The character set. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static IConventionEntityTypeBuilder HasCharSet(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] CharSet charSet,
            bool? explicitlyDelegateToChildren = null,
            bool fromDataAnnotation = false)
            => entityTypeBuilder.HasCharSet(charSet?.Name, explicitlyDelegateToChildren, fromDataAnnotation);

        /// <summary>
        /// Returns a value indicating whether the MySQL character set can be set on the table associated with this entity.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The name of the character set. </param>
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

        /// <summary>
        /// Returns a value indicating whether the MySQL character set can be set on the table associated with this entity.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="charSet"> The character set. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true"/> if the mapped table can be configured with the collation.</returns>
        public static bool CanSetCharSet(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] CharSet charSet,
            bool fromDataAnnotation = false)
            => entityTypeBuilder.CanSetCharSet(charSet?.Name, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given character set delegation setting can be set.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="explicitlyDelegateToChildren"> The character set delegation setting. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given character set delegation setting can be set as default. </returns>
        public static bool CanSetCharSetDelegation(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] bool? explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(MySqlAnnotationNames.CharSetDelegation, explicitlyDelegateToChildren, fromDataAnnotation);
        }

        #endregion CharSet and delegation

        #region Collation and delegation

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder UseCollation(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string collation,
            bool? explicitlyDelegateToChildren = null)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetCollation(collation);
            entityTypeBuilder.Metadata.SetCollationDelegation(explicitlyDelegateToChildren);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> UseCollation<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] string collation,
            bool? explicitlyDelegateToChildren = null)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)UseCollation((EntityTypeBuilder)entityTypeBuilder, collation, explicitlyDelegateToChildren);

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you only specify the collation, MySQL implicitly sets
        /// the proper character set as well.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static IConventionEntityTypeBuilder UseCollation(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] string collation,
            bool fromDataAnnotation = false,
            bool? explicitlyDelegateToChildren = null)
        {
            if (entityTypeBuilder.CanSetCollation(collation, fromDataAnnotation) &&
                entityTypeBuilder.CanSetCollationDelegation(explicitlyDelegateToChildren, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetCollation(collation, fromDataAnnotation);
                entityTypeBuilder.Metadata.SetCollationDelegation(explicitlyDelegateToChildren, fromDataAnnotation);

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

        /// <summary>
        ///     Returns a value indicating whether the given collation delegation setting can be set.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="explicitlyDelegateToChildren"> The collation delegation setting. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns> <see langword="true" /> if the given collation delegation setting can be set as default. </returns>
        public static bool CanSetCollationDelegation(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? explicitlyDelegateToChildren = null,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(MySqlAnnotationNames.CollationDelegation, explicitlyDelegateToChildren, fromDataAnnotation);
        }

        #endregion Collation and delegation
    }
}
