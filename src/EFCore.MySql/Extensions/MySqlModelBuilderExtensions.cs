// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlModelBuilderExtensions
    {
        #region CharSet and delegation

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            DelegationModes? delegationModes = null)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(charSet, nameof(charSet));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            if (delegationModes is not null && !Enum.IsDefined(typeof(DelegationModes), delegationModes))
            {
                throw new ArgumentOutOfRangeException(nameof(delegationModes), delegationModes, null);
            }

            modelBuilder.Model.SetCharSet(charSet);
            modelBuilder.Model.SetCharSetDelegation(delegationModes);

            return modelBuilder;
        }

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="true"/>.
        /// </param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            bool explicitlyDelegateToChildren)
            => modelBuilder.HasCharSet(
                charSet,
                explicitlyDelegateToChildren == false
                    ? DelegationModes.ApplyToDatabases
                    : DelegationModes.ApplyToAll);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder HasCharSet(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            DelegationModes? delegationModes = null,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(charSet, nameof(charSet));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            if (modelBuilder.CanSetCharSet(charSet, fromDataAnnotation) &&
                modelBuilder.CanSetCharSetDelegation(delegationModes, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetCharSet(charSet, fromDataAnnotation);
                modelBuilder.Metadata.SetCharSetDelegation(delegationModes, fromDataAnnotation);

                return modelBuilder;
            }

            return null;
        }

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder HasCharSet(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            bool explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
            => modelBuilder.HasCharSet(
                charSet,
                explicitlyDelegateToChildren == false
                    ? DelegationModes.ApplyToDatabases
                    : DelegationModes.ApplyToAll,
                fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given character set can be set as default.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="charSet"> The character set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given character set can be set as default. </returns>
        public static bool CanSetCharSet(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(charSet, nameof(charSet));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.CharSet, charSet, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns a value indicating whether the given character set delegation modes can be set.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="delegationModes"> The character set delegation modes. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given character set delegation modes can be set as default. </returns>
        public static bool CanSetCharSetDelegation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] DelegationModes? delegationModes,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationModes, fromDataAnnotation);
        }

        #endregion CharSet and delegation

        #region Collation and delegation

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder UseCollation(
            [NotNull] this ModelBuilder modelBuilder,
            string collation,
            DelegationModes? delegationModes)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(collation, nameof(collation));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            modelBuilder.Model.SetCollation(collation);
            modelBuilder.Model.SetCollationDelegation(delegationModes);

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="true"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder UseCollation(
            [NotNull] this ModelBuilder modelBuilder,
            string collation,
            bool? explicitlyDelegateToChildren)
            => modelBuilder.UseCollation(
                collation,
                explicitlyDelegateToChildren == false
                    ? DelegationModes.ApplyToDatabases
                    : DelegationModes.ApplyToAll);

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="delegationModes">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationModes.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder UseCollation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            string collation,
            DelegationModes? delegationModes,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(collation, nameof(collation));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            if (modelBuilder.CanSetCollation(collation, fromDataAnnotation) &&
                modelBuilder.CanSetCollationDelegation(delegationModes, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetCollation(collation, fromDataAnnotation);
                modelBuilder.Metadata.SetCharSetDelegation(delegationModes, fromDataAnnotation);

                return modelBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder UseCollation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            string collation,
            bool? explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
            => modelBuilder.UseCollation(
                collation,
                explicitlyDelegateToChildren == false
                    ? DelegationModes.ApplyToDatabases
                    : DelegationModes.ApplyToAll,
                fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given collation delegation modes can be set.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="delegationModes"> The collation delegation modes. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given collation delegation modes can be set as default. </returns>
        public static bool CanSetCollationDelegation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] DelegationModes? delegationModes,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullOrEnumValue(delegationModes, nameof(delegationModes));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.CollationDelegation, delegationModes, fromDataAnnotation);
        }

        #endregion Collation and delegation

        #region GuidCollation

        /// <summary>
        ///     Configures the explicit default collation for char-based <see cref="Guid"/> columns, which will be used by all appropriate
        ///     columns without an explicit collation.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation">
        ///     The <see cref="Guid"/> default collation to apply.
        ///     An empty string means that no explicit collation will be applied, while <see langword="null"/> means that the default
        ///     collation `ascii_general_ci` will be applied.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder UseGuidCollation(
            [NotNull] this ModelBuilder modelBuilder,
            string collation)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            modelBuilder.Model.SetGuidCollation(collation);

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the explicit default collation for char-based <see cref="Guid"/> columns, which will be used by all appropriate
        ///     columns without an explicit collation.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation">
        ///     The <see cref="Guid"/> default collation to apply.
        ///     An empty string means that no explicit collation will be applied, while <see langword="null"/> means that the default
        ///     collation `ascii_general_ci` will be applied.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder UseGuidCollation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            string collation,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetGuidCollation(collation, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetGuidCollation(collation, fromDataAnnotation);
                return modelBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given <see cref="Guid"/> default collation setting can be set.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The <see cref="Guid"/> default collation setting. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given <see cref="Guid"/> default collation setting can be set. </returns>
        public static bool CanSetGuidCollation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string collation,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.GuidCollation, collation, fromDataAnnotation);
        }

        #endregion GuidCollation
    }
}
