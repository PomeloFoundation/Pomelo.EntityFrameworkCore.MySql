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
        ///     Returns the character set delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The character set delegation setting. </returns>
        public static bool? GetCharSetDelegation([NotNull] this IModel model)
            => model[MySqlAnnotationNames.CharSetDelegation] as bool?;

        /// <summary>
        ///     Attempts to set the character set delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        public static void SetCharSetDelegation([NotNull] this IMutableModel model, bool? explicitlyDelegateToChildren)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, explicitlyDelegateToChildren);

        /// <summary>
        ///     Attempts to set the character set delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the character set if set to <see langword="false"/>.
        /// They will explicitly inherit the character set if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetCharSetDelegation([NotNull] this IConventionModel model, bool? explicitlyDelegateToChildren, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CharSetDelegation, explicitlyDelegateToChildren, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the character set delegation setting of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default character set delegation setting. </returns>
        public static ConfigurationSource? GetCharSetDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.CharSetDelegation)?.GetConfigurationSource();

        #endregion CharSetDelegation

        #region CollationDelegation

        /// <summary>
        ///     Returns the collation delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The collation delegation setting. </returns>
        public static bool? GetCollationDelegation([NotNull] this IModel model)
            => model[MySqlAnnotationNames.CollationDelegation] as bool?;

        /// <summary>
        ///     Attempts to set the collation delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        public static void SetCollationDelegation([NotNull] this IMutableModel model, bool? explicitlyDelegateToChildren)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, explicitlyDelegateToChildren);

        /// <summary>
        ///     Attempts to set the collation delegation setting for the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="explicitlyDelegateToChildren">
        /// Entities/tables (and possibly properties/columns) don't explicitly inherit the collation if set to <see langword="false"/>.
        /// They will explicitly inherit the collation if set to <see langword="null"/> or <see langword="true"/>.
        /// </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static void SetCollationDelegation([NotNull] this IConventionModel model, bool? explicitlyDelegateToChildren, bool fromDataAnnotation = false)
            => model.SetOrRemoveAnnotation(MySqlAnnotationNames.CollationDelegation, explicitlyDelegateToChildren, fromDataAnnotation);

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the collation delegation setting of the model/database.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default collation delegation setting. </returns>
        public static ConfigurationSource? GetCollationDelegationConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(MySqlAnnotationNames.CollationDelegation)?.GetConfigurationSource();

        #endregion CollationDelegation
    }
}
