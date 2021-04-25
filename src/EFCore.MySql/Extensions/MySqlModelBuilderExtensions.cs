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
    public static class MySqlModelBuilderExtensions
    {
        #region CharSet and delegation

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            DelegationMode? delegationMode = null)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(charSet, nameof(charSet));

            modelBuilder.Model.SetCharSet(charSet);
            modelBuilder.Model.SetCharSetDelegation(delegationMode);

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
                    ? DelegationMode.ApplyToDatabases
                    : DelegationMode.ApplyToAll);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The character set to use.</param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] CharSet charSet,
            DelegationMode? delegationMode = null)
            => modelBuilder.HasCharSet(charSet?.Name, delegationMode);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The character set to use.</param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="true"/>.
        /// </param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasCharSet(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] CharSet charSet,
            bool explicitlyDelegateToChildren)
            => modelBuilder.HasCharSet(
                charSet?.Name,
                explicitlyDelegateToChildren);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The name of the character set to use.</param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder HasCharSet(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string charSet,
            DelegationMode? delegationMode = null,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetCharSet(charSet, fromDataAnnotation) &&
                modelBuilder.CanSetCharSetDelegation(delegationMode, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetCharSet(charSet, fromDataAnnotation);
                modelBuilder.Metadata.SetCharSetDelegation(delegationMode, fromDataAnnotation);

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
                    ? DelegationMode.ApplyToDatabases
                    : DelegationMode.ApplyToAll,
                fromDataAnnotation);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The character set to use.</param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder HasCharSet(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] CharSet charSet,
            DelegationMode? delegationMode = null,
            bool fromDataAnnotation = false)
            => modelBuilder.HasCharSet(charSet?.Name, delegationMode, fromDataAnnotation);

        /// <summary>
        /// Sets the default character set to use for the model/database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="charSet">The character set to use.</param>
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
            [CanBeNull] CharSet charSet,
            bool explicitlyDelegateToChildren,
            bool fromDataAnnotation = false)
            => modelBuilder.HasCharSet(
                charSet?.Name,
                explicitlyDelegateToChildren,
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
        ///     Returns a value indicating whether the given character set delegation mode can be set.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="delegationMode"> The character set delegation mode. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given character set delegation mode can be set as default. </returns>
        public static bool CanSetCharSetDelegation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] DelegationMode? delegationMode,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationMode, fromDataAnnotation);
        }

        #endregion CharSet and delegation

        #region Collation and delegation

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder UseCollation(
            [NotNull] this ModelBuilder modelBuilder,
            string collation,
            DelegationMode? delegationMode)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(collation, nameof(collation));

            modelBuilder.Model.SetCollation(collation);
            modelBuilder.Model.SetCollationDelegation(delegationMode);

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
                    ? DelegationMode.ApplyToDatabases
                    : DelegationMode.ApplyToAll);

        /// <summary>
        ///     Configures the database collation, which will be used by all columns without an explicit collation. Also finely controls
        ///     where to recursively apply the collation and where not (including this model/database).
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="collation"> The collation. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.Default"/> (which translates to <see cref="DelegationMode.ApplyToAll"/>) if set to
        /// <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder UseCollation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            string collation,
            DelegationMode? delegationMode,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetCollation(collation, fromDataAnnotation) &&
                modelBuilder.CanSetCollationDelegation(delegationMode, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetCollation(collation, fromDataAnnotation);
                modelBuilder.Metadata.SetCharSetDelegation(delegationMode, fromDataAnnotation);

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
                    ? DelegationMode.ApplyToDatabases
                    : DelegationMode.ApplyToAll,
                fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given collation delegation mode can be set.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <param name="delegationMode"> The collation delegation mode. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the given collation delegation mode can be set as default. </returns>
        public static bool CanSetCollationDelegation(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] DelegationMode? delegationMode,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            return modelBuilder.CanSetAnnotation(MySqlAnnotationNames.CollationDelegation, delegationMode, fromDataAnnotation);
        }

        #endregion Collation and delegation
    }
}
