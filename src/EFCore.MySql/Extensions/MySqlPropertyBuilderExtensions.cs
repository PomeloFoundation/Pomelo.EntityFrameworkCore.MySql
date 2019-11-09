// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="PropertyBuilder" />.
    /// </summary>
    public static class MySqlPropertyBuilderExtensions
    {
        /// <summary>
        ///     Configures the key property to use the MySQL IDENTITY feature to generate values for new entities,
        ///     when targeting MySQL. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder UseMySqlIdentityColumn(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn);
            
            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the MySQL IDENTITY feature to generate values for new entities,
        ///     when targeting MySQL. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> UseMySqlIdentityColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseMySqlIdentityColumn((PropertyBuilder)propertyBuilder);

        /// <summary>
        ///     Configures the key property to use the MySQL Computed feature to generate values for new entities,
        ///     when targeting MySQL. This method sets the property to be <see cref="ValueGenerated.OnAddOrUpdate" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder UseMySqlComputedColumn(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            
            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(MySqlValueGenerationStrategy.ComputedColumn);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the MySQL Computed feature to generate values for new entities,
        ///     when targeting MySQL. This method sets the property to be <see cref="ValueGenerated.OnAddOrUpdate" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> UseMySqlComputedColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseMySqlComputedColumn((PropertyBuilder)propertyBuilder);

        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder HasCharSet(
            [NotNull] this PropertyBuilder propertyBuilder,
            string charSet)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetCharSet(charSet);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The <see cref="CharSet"/> to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder HasCharSet(
            [NotNull] this PropertyBuilder propertyBuilder,
            CharSet charSet)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetCharSet(charSet?.Name);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the collation for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="collation">The name of the collation to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder HasCollation(
            [NotNull] this PropertyBuilder propertyBuilder,
            string collation)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetCollation(collation);

            return propertyBuilder;
        }
    }
}
