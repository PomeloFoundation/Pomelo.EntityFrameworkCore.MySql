// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for <see cref="ComplexTypePropertyBuilder" />.
    /// </summary>
    public static class MySqlComplexTypePropertyBuilderExtensions
    {
        /// <summary>
        /// Configures the charset for the property's column.
        /// </summary>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ComplexTypePropertyBuilder HasCharSet(
            [NotNull] this ComplexTypePropertyBuilder propertyBuilder,
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
        /// <param name="charSet">The name of the charset to configure for the property's column.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ComplexTypePropertyBuilder<TProperty> HasCharSet<TProperty>(
            [NotNull] this ComplexTypePropertyBuilder<TProperty> propertyBuilder,
            string charSet)
            => (ComplexTypePropertyBuilder<TProperty>)HasCharSet((ComplexTypePropertyBuilder)propertyBuilder, charSet);
    }
}
