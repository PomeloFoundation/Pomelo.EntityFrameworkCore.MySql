// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for metadata.
    /// </summary>
    public static class MySqlMetadataExtensions
    {
        /// <summary>
        ///     Gets the MySQL specific metadata for a property.
        /// </summary>
        /// <param name="property"> The property to get metadata for. </param>
        /// <returns> The MySQL specific metadata for the property. </returns>
        public static MySqlPropertyAnnotations MySql([NotNull] this IMutableProperty property)
            => (MySqlPropertyAnnotations)MySql((IProperty)property);

        /// <summary>
        ///     Gets the MySQL specific metadata for a property.
        /// </summary>
        /// <param name="property"> The property to get metadata for. </param>
        /// <returns> The MySQL specific metadata for the property. </returns>
        public static IMySqlPropertyAnnotations MySql([NotNull] this IProperty property)
            => new MySqlPropertyAnnotations(Check.NotNull(property, nameof(property)));

        /// <summary>
        ///     Gets the MySQL specific metadata for an index.
        /// </summary>
        /// <param name="index"> The index to get metadata for. </param>
        /// <returns> The MySQL specific metadata for the index. </returns>
        public static MySqlIndexAnnotations MySql([NotNull] this IMutableIndex index)
            => (MySqlIndexAnnotations)MySql((IIndex)index);

        /// <summary>
        ///     Gets the MySQL specific metadata for an index.
        /// </summary>
        /// <param name="index"> The index to get metadata for. </param>
        /// <returns> The MySQL specific metadata for the index. </returns>
        public static IMySqlIndexAnnotations MySql([NotNull] this IIndex index)
            => new MySqlIndexAnnotations(Check.NotNull(index, nameof(index)));
    }
}
