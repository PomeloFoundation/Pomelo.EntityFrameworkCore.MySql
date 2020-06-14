// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Json specific extension methods for <see cref="MySqlDbContextOptionsBuilder" />.
    /// </summary>
    public static class MySqlJsonDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     Use Json to access MySQL spatial data.
        /// </summary>
        /// <param name="optionsBuilder"> The build being used to configure MySQL. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static MySqlDbContextOptionsBuilder UseMicrosoftJson(
            [NotNull] this MySqlDbContextOptionsBuilder optionsBuilder)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;

            var extension = coreOptionsBuilder.Options.FindExtension<MySqlJsonOptionsExtension>()
                ?? new MySqlJsonOptionsExtension();

            extension.Serializer = MicrosoftJsonSerializer.Instance;

            ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}
