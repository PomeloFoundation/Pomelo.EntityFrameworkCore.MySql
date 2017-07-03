// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SQL Server specific extension methods for metadata.
    /// </summary>
    public static class MySqlMetadataExtensions
    {
        /// <summary>
        ///     Gets the SQL Server specific metadata for a property.
        /// </summary>
        /// <param name="property"> The property to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the property. </returns>
        public static MySqlPropertyAnnotations MySql([NotNull] this IMutableProperty property)
            => (MySqlPropertyAnnotations)MySql((IProperty)property);

        /// <summary>
        ///     Gets the SQL Server specific metadata for a property.
        /// </summary>
        /// <param name="property"> The property to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the property. </returns>
        public static IMySqlPropertyAnnotations MySql([NotNull] this IProperty property)
            => new MySqlPropertyAnnotations(Check.NotNull(property, nameof(property)));

        /// <summary>
        ///     Gets the SQL Server specific metadata for an entity.
        /// </summary>
        /// <param name="entityType"> The entity to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the entity. </returns>
        public static MySqlEntityTypeAnnotations MySql([NotNull] this IMutableEntityType entityType)
            => (MySqlEntityTypeAnnotations)MySql((IEntityType)entityType);

        /// <summary>
        ///     Gets the SQL Server specific metadata for an entity.
        /// </summary>
        /// <param name="entityType"> The entity to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the entity. </returns>
        public static IMySqlEntityTypeAnnotations MySql([NotNull] this IEntityType entityType)
            => new MySqlEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));

        /// <summary>
        ///     Gets the SQL Server specific metadata for a key.
        /// </summary>
        /// <param name="key"> The key to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the key. </returns>
        public static MySqlKeyAnnotations MySql([NotNull] this IMutableKey key)
            => (MySqlKeyAnnotations)MySql((IKey)key);

        /// <summary>
        ///     Gets the SQL Server specific metadata for a key.
        /// </summary>
        /// <param name="key"> The key to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the key. </returns>
        public static IMySqlKeyAnnotations MySql([NotNull] this IKey key)
            => new MySqlKeyAnnotations(Check.NotNull(key, nameof(key)));

        /// <summary>
        ///     Gets the SQL Server specific metadata for an index.
        /// </summary>
        /// <param name="index"> The index to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the index. </returns>
        public static MySqlIndexAnnotations MySql([NotNull] this IMutableIndex index)
            => (MySqlIndexAnnotations)MySql((IIndex)index);

        /// <summary>
        ///     Gets the SQL Server specific metadata for an index.
        /// </summary>
        /// <param name="index"> The index to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the index. </returns>
        public static IMySqlIndexAnnotations MySql([NotNull] this IIndex index)
            => new MySqlIndexAnnotations(Check.NotNull(index, nameof(index)));

        /// <summary>
        ///     Gets the SQL Server specific metadata for a model.
        /// </summary>
        /// <param name="model"> The model to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the model. </returns>
        public static MySqlModelAnnotations MySql([NotNull] this IMutableModel model)
            => (MySqlModelAnnotations)MySql((IModel)model);

        /// <summary>
        ///     Gets the SQL Server specific metadata for a model.
        /// </summary>
        /// <param name="model"> The model to get metadata for. </param>
        /// <returns> The SQL Server specific metadata for the model. </returns>
        public static IMySqlModelAnnotations MySql([NotNull] this IModel model)
            => new MySqlModelAnnotations(Check.NotNull(model, nameof(model)));
    }
}
