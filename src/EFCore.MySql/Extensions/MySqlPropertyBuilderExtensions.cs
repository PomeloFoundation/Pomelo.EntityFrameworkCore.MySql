// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="PropertyBuilder" />.
    /// </summary>
    public static class MySqlPropertyBuilderExtensions
    {

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder UseMySqlIdentityColumn(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            GetMySqlInternalBuilder(propertyBuilder).ValueGenerationStrategy(MySqlValueGenerationStrategy.IdentityColumn);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the SQL Server IDENTITY feature to generate values for new entities,
        ///     when targeting SQL Server. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property being configured. </typeparam>
        /// <param name="propertyBuilder"> The builder for the property being configured. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static PropertyBuilder<TProperty> UseMySqlIdentityColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseMySqlIdentityColumn((PropertyBuilder)propertyBuilder);

        private static MySqlPropertyBuilderAnnotations GetMySqlInternalBuilder(PropertyBuilder propertyBuilder)
            => propertyBuilder.GetInfrastructure<InternalPropertyBuilder>().MySql(ConfigurationSource.Explicit);
    }
}
