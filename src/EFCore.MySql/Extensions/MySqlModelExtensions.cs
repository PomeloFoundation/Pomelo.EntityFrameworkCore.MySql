// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlModelExtensions
    {
        #region ValueGenerationStrategy

        /// <summary>
        ///     Returns the <see cref="MySqlValueGenerationStrategy" /> to use for properties
        ///     of keys in the model, unless the property has a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default <see cref="MySqlValueGenerationStrategy" />. </returns>
        public static MySqlValueGenerationStrategy? GetValueGenerationStrategy([NotNull] this IModel model)
            => (MySqlValueGenerationStrategy?)model[MySqlAnnotationNames.ValueGenerationStrategy];

        /// <summary>
        ///     Attempts to set the <see cref="MySqlValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetValueGenerationStrategy([NotNull] this IMutableModel model, MySqlValueGenerationStrategy? value)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value);

        /// <summary>
        ///     Attempts to set the <see cref="MySqlValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetValueGenerationStrategy(
            [NotNull] this IConventionModel model, MySqlValueGenerationStrategy? value, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default <see cref="MySqlValueGenerationStrategy" />.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default <see cref="MySqlValueGenerationStrategy" />. </returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        #endregion ValueGenerationStrategy

        #region CharSet

        /// <summary>
        ///     Returns the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default character set. </returns>
        public static string GetCharSet([NotNull] this IModel model)
            => model[MySqlAnnotationNames.CharSet] as string;

        /// <summary>
        ///     Attempts to set the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="charSet"> The default character set. </param>
        public static void SetCharSet([NotNull] this IMutableModel model, string charSet)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet);

        /// <summary>
        ///     Attempts to set the character set to use as the default for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="charSet"> The default character set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetCharSet([NotNull] this IConventionModel model, string charSet, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSet, charSet, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default character set of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default character set. </returns>
        public static ConfigurationSource? GetCharSetConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.CharSet)?.GetConfigurationSource();

        #endregion CharSet

        #region CharSetDelegation

        /// <summary>
        ///     Returns the character set delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The character set delegation mode. </returns>
        public static DelegationMode? GetCharSetDelegation([NotNull] this IModel model)
            => ObjectToEnumConverter.GetEnumValue<DelegationMode>(model[MySqlAnnotationNames.CharSetDelegation]) ??
               (model[MySqlAnnotationNames.CharSetDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationMode.ApplyToAll
                       : DelegationMode.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the character set delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCharSetDelegation([NotNull] this IMutableModel model, DelegationMode? delegationMode)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationMode);

        /// <summary>
        ///     Attempts to set the character set delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the character set and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetCharSetDelegation([NotNull] this IConventionModel model, DelegationMode? delegationMode, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, delegationMode, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the character set delegation mode of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default character set delegation mode. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual character set delegation mode for the model/database.
        ///     Always returns a concrete value and never returns <see cref="DelegationMode.Default"/>.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The actual character set delegation mode. </returns>
        public static DelegationMode GetActualCharSetDelegation([NotNull] this IModel model)
        {
            var delegationMode = model.GetCharSetDelegation() ?? DelegationMode.Default;
            return delegationMode == DelegationMode.Default
                ? DelegationMode.ApplyToAll
                : delegationMode;
        }

        #endregion CharSetDelegation

        #region CollationDelegation

        /// <summary>
        ///     Returns the collation delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The collation delegation mode. </returns>
        public static DelegationMode? GetCollationDelegation([NotNull] this IModel model)
            => ObjectToEnumConverter.GetEnumValue<DelegationMode>(model[MySqlAnnotationNames.CollationDelegation]) ??
               (model[MySqlAnnotationNames.CollationDelegation] is bool explicitlyDelegateToChildren
                   ? explicitlyDelegateToChildren
                       ? DelegationMode.ApplyToAll
                       : DelegationMode.ApplyToDatabases
                   : null);

        /// <summary>
        ///     Attempts to set the collation delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        public static void SetCollationDelegation([NotNull] this IMutableModel model, DelegationMode? delegationMode)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationMode);

        /// <summary>
        ///     Attempts to set the collation delegation mode for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="delegationMode">
        /// Finely controls where to recursively apply the collation and where not (including this model/database).
        /// Implicitly uses <see cref="DelegationMode.ApplyToAll"/> if set to <see langword="null"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetCollationDelegation([NotNull] this IConventionModel model, DelegationMode? delegationMode, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, delegationMode, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the collation delegation mode of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default collation delegation mode. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the actual collation delegation mode for the model/database.
        ///     Always returns a concrete value and never returns <see cref="DelegationMode.Default"/>.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The actual collation delegation mode. </returns>
        public static DelegationMode GetActualCollationDelegation([NotNull] this IModel model)
        {
            var delegationMode = model.GetCollationDelegation() ?? DelegationMode.Default;
            return delegationMode == DelegationMode.Default
                ? DelegationMode.ApplyToAll
                : delegationMode;
        }

        #endregion CollationDelegation
    }
}
