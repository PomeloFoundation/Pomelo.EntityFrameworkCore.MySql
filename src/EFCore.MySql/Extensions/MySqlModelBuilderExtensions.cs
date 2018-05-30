// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySql specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class MySqlModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the model to use the MySql IDENTITY feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting MySql. This is the default
        ///     behavior when targeting MySql.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForMySqlUseIdentityColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var property = modelBuilder.Model;

            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.IdentityColumn;

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the model to use the MySql Computed feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAddOrUpdate" />, when targeting MySql. This is the default
        ///     behavior when targeting MySql.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForMySqlUseComputedColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var property = modelBuilder.Model;

            property.MySql().ValueGenerationStrategy = MySqlValueGenerationStrategy.ComputedColumn;

            return modelBuilder;
        }
    }
}
