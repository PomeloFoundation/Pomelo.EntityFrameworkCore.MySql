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
    ///     MySQL specific extension methods for entity types.
    /// </summary>
    public static class MySqlEntityTypeExtensions
    {
        #region CharSet

        /// <summary>
        /// Get the MySQL character set for the table associated with this entity.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The name of the character set. </returns>
        public static string GetCharSet([NotNull] this IReadOnlyEntityType entityType)
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
        ///     Returns the character set delegation modes for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The character set delegation modes. </returns>
        public static DelegationModes? GetCharSetDelegation([NotNull] this IReadOnlyEntityType entityType)
            => ObjectToEnumConverter.GetEnumValue<DelegationModes>(entityType[MySqlAnnotationNames.CharSetDelegation]) ??
               (entityType[MySqlAnnotationNames.CharSetDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationModes.ApplyToAll
                       : DelegationModes.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the character set delegation modes for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCharSetDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] DelegationModes? delegationModes)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationModes);

        /// <summary>
        ///     Attempts to set the character set delegation modes for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static DelegationModes? SetCharSetDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] DelegationModes? delegationModes,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationModes, fromDataAnnotation);

            return delegationModes;
        }

        /// <summary>
        ///     Gets the configuration source for the character set delegation modes.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual character set delegation modes for the entity/table.
        ///     Always returns a concrete value and never returns <see cref="DelegationModes.Default"/>.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The actual character set delegation modes. </returns>
        public static DelegationModes GetActualCharSetDelegation([NotNull] this IReadOnlyEntityType entityType)
        {
            var delegationModes = entityType.GetCharSetDelegation() ?? DelegationModes.Default;
            return delegationModes == DelegationModes.Default
                ? DelegationModes.ApplyToAll
                : delegationModes;
        }

        #endregion CharSetDelegation

        #region Collation

        /// <summary>
        /// Get the MySQL collation for the table associated with this entity.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The name of the collation. </returns>
        public static string GetCollation([NotNull] this IReadOnlyEntityType entityType)
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
        ///     Returns the collation delegation modes for the entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The collation delegation modes. </returns>
        public static DelegationModes? GetCollationDelegation([NotNull] this IReadOnlyEntityType entityType)
            => ObjectToEnumConverter.GetEnumValue<DelegationModes>(entityType[MySqlAnnotationNames.CollationDelegation]) ??
               (entityType[MySqlAnnotationNames.CollationDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationModes.ApplyToAll
                       : DelegationModes.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the collation delegation modes for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCollationDelegation(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] DelegationModes? delegationModes)
            => entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationModes);

        /// <summary>
        ///     Attempts to set the collation delegation modes for entity/table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this entity/table).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static DelegationModes? SetCollationDelegation(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] DelegationModes? delegationModes,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationModes, fromDataAnnotation);

            return delegationModes;
        }

        /// <summary>
        ///     Gets the configuration source for the collation delegation modes.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(MySqlAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual collation delegation modes for the entity/table.
        ///     Always returns a concrete value and never returns <see cref="DelegationModes.Default"/>.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The actual collation delegation modes. </returns>
        public static DelegationModes GetActualCollationDelegation([NotNull] this IReadOnlyEntityType entityType)
        {
            var delegationModes = entityType.GetCollationDelegation() ?? DelegationModes.Default;
            return delegationModes == DelegationModes.Default
                ? DelegationModes.ApplyToAll
                : delegationModes;
        }

        #endregion CollationDelegation
    }
}
