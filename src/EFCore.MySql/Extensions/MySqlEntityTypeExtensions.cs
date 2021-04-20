// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="IEntityType" />.
    /// </summary>
    public static class MySqlEntityTypeExtensions
    {
        #region CharSet

        /// <summary>
        /// Get the MySQL character set for the table associated with this entity.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The name of the character set. </returns>
        public static string GetCharSet([NotNull] this IEntityType entityType)
            => entityType[MySqlAnnotationNames.CharSet] as string;

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses the default collation.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="charSet"> The name of the character set. </param>
        public static void SetCharSet(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] string charSet)
        {
            Check.NullButNotEmpty(charSet, nameof(charSet));

            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet);
        }

        /// <summary>
        /// Sets the MySQL character set on the table associated with this entity. When you only specify the character set, MySQL implicitly
        /// uses the default collation.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="charSet"> The name of the character set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static string SetCharSet(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] string charSet,
            bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(charSet, nameof(charSet));

            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet, fromDataAnnotation);

            return charSet;
        }

        /// <summary>
        ///     Gets the configuration source for the character set setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCharSetConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CharSet)?.GetConfigurationSource();

        #endregion CharSet

        #region CharSetDelegation

        /// <summary>
        ///     Returns the character set delegation setting for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The character set delegation setting. </returns>
        public static bool? GetCharSetDelegation([NotNull] this IEntityType entityType)
            => entityType[MySqlAnnotationNames.CharSetDelegation] as bool?;

        /// <summary>
        ///     Attempts to set the character set delegation setting for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        public static void SetCharSetDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] bool? explicitlyDelegateToChildren)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, explicitlyDelegateToChildren);

        /// <summary>
        ///     Attempts to set the character set delegation setting for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static bool? SetCharSetDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] bool? explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, explicitlyDelegateToChildren, fromDataAnnotation);

            return explicitlyDelegateToChildren;
        }

        /// <summary>
        ///     Gets the configuration source for the character set delegation setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        #endregion CharSetDelegation

        #region Collation

        /// <summary>
        /// Get the MySQL collation for the table associated with this entity.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The name of the collation. </returns>
        public static string GetCollation([NotNull] this IEntityType entityType)
            => entityType[RelationalAnnotationNames.Collation] as string;

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you specify the collation, MySQL implicitly sets the
        /// proper character set as well.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="collation"> The name of the collation. </param>
        public static void SetCollation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] string collation)
        {
            Check.NullButNotEmpty(collation, nameof(collation));

            entityType.SetOrRemoveAnnotation(RelationalAnnotationNames.Collation, collation);
        }

        /// <summary>
        /// Sets the MySQL collation on the table associated with this entity. When you specify the collation, MySQL implicitly sets the
        /// proper character set as well.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="collation"> The name of the collation. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static string SetCollation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] string collation,
            bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(collation, nameof(collation));

            entityType.SetOrRemoveAnnotation(RelationalAnnotationNames.Collation, collation, fromDataAnnotation);

            return collation;
        }

        /// <summary>
        ///     Gets the configuration source for the collation setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCollationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(RelationalAnnotationNames.Collation)?.GetConfigurationSource();

        #endregion Collation

        #region CollationDelegation

        /// <summary>
        ///     Returns the collation delegation setting for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The collation delegation setting. </returns>
        public static bool? GetCollationDelegation([NotNull] this IEntityType entityType)
            => entityType[MySqlAnnotationNames.CollationDelegation] as bool?;

        /// <summary>
        ///     Attempts to set the collation delegation setting for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Properties/columns don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        public static void SetCollationDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] bool? explicitlyDelegateToChildren)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, explicitlyDelegateToChildren);

        /// <summary>
        ///     Attempts to set the collation delegation setting for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static bool? SetCollationDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] bool? explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, explicitlyDelegateToChildren, fromDataAnnotation);

            return explicitlyDelegateToChildren;
        }

        /// <summary>
        ///     Gets the configuration source for the collation delegation setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        #endregion CollationDelegation
    }
}
