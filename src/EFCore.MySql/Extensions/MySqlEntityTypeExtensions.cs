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
        ///     Gets the configuration source for the character set mode.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCharSetConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CharSet)?.GetConfigurationSource();

        #endregion CharSet

        #region CharSetDelegation

        /// <summary>
        ///     Returns the character set delegation mode for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The character set delegation mode. </returns>
        public static DelegationMode? GetCharSetDelegation([NotNull] this IEntityType entityType)
            => entityType[MySqlAnnotationNames.CharSetDelegation] as DelegationMode? ??
               (entityType[MySqlAnnotationNames.CharSetDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationMode.ApplyToAll
                       : DelegationMode.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the character set delegation mode for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCharSetDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] DelegationMode? delegationMode)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationMode);

        /// <summary>
        ///     Attempts to set the character set delegation mode for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static DelegationMode? SetCharSetDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] DelegationMode? delegationMode,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationMode, fromDataAnnotation);

            return delegationMode;
        }

        /// <summary>
        ///     Gets the configuration source for the character set delegation mode.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual character set delegation mode for the entity/table.
        ///     Always returns a concrete value and never returns <see cref="DelegationMode.Default"/>.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The actual character set delegation mode. </returns>
        public static DelegationMode GetActualCharSetDelegation([NotNull] this IEntityType entityType)
        {
            var delegationMode = entityType.GetCharSetDelegation() ?? DelegationMode.Default;
            return delegationMode == DelegationMode.Default
                ? DelegationMode.ApplyToAll
                : delegationMode;
        }

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
        ///     Gets the configuration source for the collation mode.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCollationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(RelationalAnnotationNames.Collation)?.GetConfigurationSource();

        #endregion Collation

        #region CollationDelegation

        /// <summary>
        ///     Returns the collation delegation mode for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The collation delegation mode. </returns>
        public static DelegationMode? GetCollationDelegation([NotNull] this IEntityType entityType)
            => entityType[MySqlAnnotationNames.CollationDelegation] as DelegationMode? ??
               (entityType[MySqlAnnotationNames.CollationDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationMode.ApplyToAll
                       : DelegationMode.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the collation delegation mode for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCollationDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] DelegationMode? delegationMode)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationMode);

        /// <summary>
        ///     Attempts to set the collation delegation mode for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static DelegationMode? SetCollationDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] DelegationMode? delegationMode,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationMode, fromDataAnnotation);

            return delegationMode;
        }

        /// <summary>
        ///     Gets the configuration source for the collation delegation mode.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual collation delegation mode for the entity/table.
        ///     Always returns a concrete value and never returns <see cref="DelegationMode.Default"/>.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The actual collation delegation mode. </returns>
        public static DelegationMode GetActualCollationDelegation([NotNull] this IEntityType entityType)
        {
            var delegationMode = entityType.GetCollationDelegation() ?? DelegationMode.Default;
            return delegationMode == DelegationMode.Default
                ? DelegationMode.ApplyToAll
                : delegationMode;
        }

        #endregion CollationDelegation
    }
}
