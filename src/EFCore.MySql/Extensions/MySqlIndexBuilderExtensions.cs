// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="IndexBuilder" />.
    /// </summary>
    public static class MySqlIndexBuilderExtensions
    {
        /// <summary>
        ///     Configures whether the index is clustered when targeting SQL Server.
        /// </summary>
        /// <param name="indexBuilder"> The builder for the index being configured. </param>
        /// <param name="clustered"> A value indicating whether the index is clustered. </param>
        /// <returns> A builder to further configure the index. </returns>
        public static IndexBuilder ForMySqlIsClustered([NotNull] this IndexBuilder indexBuilder, bool clustered = true)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));

            indexBuilder.Metadata.MySql().IsClustered = clustered;

            return indexBuilder;
        }
    }
}
